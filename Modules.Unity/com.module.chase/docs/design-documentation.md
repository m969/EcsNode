# （追踪）模块程序设计文档

基于EcsNode框架，为追踪（Chase）行为提供可配置、可扩展、可派发节点事件的模块能力。命名空间：`ECSGame.ChaseModule`。

适配需求要点：
- 启动/暂停/恢复/停止追踪；支持启动/停止条件。
- 范围与阈值：进入/退出半径、保持距离、丢失距离，进入/退出触发回调。
- 多对象管理：候选对象列表、优先级规则、对象失效淘汰与自动重选。
- 事件派发：开始、对象变更、进入半径、达到保持距离、丢失、停止等。
- 区域限制：多边形/圆形边界，越界回退或停止策略。
- 状态暴露：空闲/搜索/追随/拦截/丢失，关键数据（当前对象、距离、速度向量）。

常用引用（代码示例中默认使用）：
```
using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
```

命名规则：
- 模块命名空间：`ECSGame.ChaseModule`。
- 配置接口名以 Config 结尾；系统接口名以 System 结尾；流程节点派发接口以 On 开头并继承 `IDispatch`。


## 配置接口设计

### IChaseConfig
- 用途：控制单个追踪体（ChaseComponent）的阈值、条件与行为策略。
- 字段：
	- string ConfigId：配置标识。
	- float EnterRadius：进入判定半径（进入该半径触发进入事件）。
	- float ExitRadius：退出判定半径（离开该半径触发退出事件）。
	- float KeepDistance：保持距离阈值（达到时触发保持事件）。
	- float LostDistance：丢失距离阈值（超过视为目标丢失）。
	- bool AutoReselectOnInvalid：目标失效时是否自动重选。
	- int CandidateCapacity：候选目标容量上限（用于内存/性能控制）。
	- IChaseAreaConfig AreaLimit：活动区域限制（可为空表示不限制）。
	- List<IStartConditionConfig> StartConditions：启动条件集合。
	- List<IStopConditionConfig> StopConditions：停止条件集合。
	- IPriorityRuleConfig PriorityRule：目标优先级规则配置。

说明：EnterRadius、ExitRadius 与 KeepDistance、LostDistance 相互独立，系统以“最严格有效原则”进行状态切换（例如达到 KeepDistance 不必满足 EnterRadius 才触发保持事件）。

### IChaseAreaConfig
- 用途：限定追踪行为的活动区域与越界策略。
- 字段：
	- AreaType Type：区域类型（Circle/Polygon）。
	- Vector3 Center：区域中心（Circle 时必填）。
	- float Radius：半径（Circle 时必填）。
	- List<Vector3> Points：顶点序列（Polygon 时必填，按顺时针）。
	- OutOfAreaStrategy Strategy：越界策略（None/Rollback/StopChase）。
	- float RollbackStep：回退步长（仅当 Strategy 为 Rollback 时生效）。

### IPriorityRuleConfig
- 用途：定义候选目标选择的加权规则。
- 字段：
	- PriorityPolicy Policy：优先级策略（DistanceAsc/Custom）。
	- Dictionary<string, float> Weights：当 Policy 为 Custom 时，键为特征名（如"distance"/"threat"/"angle"），值为权重。
	- bool HigherIsBetter：分数极性定义（true 表示分数越高优先级越高）。

### IStartConditionConfig / IStopConditionConfig
- 用途：声明式配置可复用的启动/停止条件。
- 字段：
	- string ConditionType：条件类型标识（如 "WithinEnterRadius"、"TargetAvailable"、"LeaveExitRadius"、"TargetInvalid"）。
	- Dictionary<string, string> Params：参数键值表（具体由实现解析）。


## 其他类型补充

### 流程节点派发接口（继承 IDispatch）
- IOnChaseStarted：void OnChaseStarted(EcsEntity entity, EcsEntity target)
- IOnChasePaused：void OnChasePaused(EcsEntity entity)
- IOnChaseResumed：void OnChaseResumed(EcsEntity entity)
- IOnChaseStopped：void OnChaseStopped(EcsEntity entity, string reason)
- IOnChaseTargetChanged：void OnChaseTargetChanged(EcsEntity entity, EcsEntity oldTarget, EcsEntity newTarget)
- IOnChaseEnterRadius：void OnChaseEnterRadius(EcsEntity entity, EcsEntity target, float radius)
- IOnChaseKeepDistanceReached：void OnChaseKeepDistanceReached(EcsEntity entity, EcsEntity target, float keepDistance)
- IOnChaseLostTarget：void OnChaseLostTarget(EcsEntity entity, EcsEntity lastTarget)
- IOnChaseAreaViolated：void OnChaseAreaViolated(EcsEntity entity, OutOfAreaStrategy strategy)

说明：节点派发通过实体调用 `Dispatch<T>` 完成，例如：
```
entity.Dispatch<IOnChaseStarted>(s => s.OnChaseStarted(entity, target));
```

### 基础数据类型补充
- struct ChaseKinematics
	- Vector3 Direction
	- float Speed
	- Vector3 Velocity => Direction.normalized * Speed

- struct TargetScore
	- long TargetId
	- float Score

### 枚举补充
- enum ChaseState { Idle, Searching, Following, Intercepting, Lost }
- enum AreaType { None, Circle, Polygon }
- enum OutOfAreaStrategy { None, Rollback, StopChase }
- enum PriorityPolicy { DistanceAsc, Custom }


## 实体设计

- 任何需要追踪能力的实体通过挂载 `ChaseComponent` 及其配套组件即可获得完整功能。

## 组件设计

### ChaseComponent
- 用途：作为宿主实体追踪逻辑的核心组件，维护配置绑定与当前目标引用。
- 字段：
	- string ConfigId：绑定的追踪配置标识（用于从配置系统加载 IChaseConfig）。
	- long CurrentTargetId：当前锁定目标实体 Id（若无目标则为 -1 或 0）。
	- bool IsPaused：记录当前是否处于暂停状态，便于 Resume/Stop 流程判定。
	- float LastTargetChangeTime：最近一次目标切换的时间戳。

#### ChaseSystem
- 功能接口：
	- void StartChase(EcsEntity entity, EcsEntity target = null)
	- void PauseChase(EcsEntity entity)
	- void ResumeChase(EcsEntity entity)
	- void StopChase(EcsEntity entity, string reason = "manual")
	- void Tick(EcsEntity entity, float deltaTime)
	- void UpdateTargetCandidates(EcsEntity entity, List<EcsEntity> candidates)
	- EcsEntity SelectTarget(EcsEntity entity)
	- ChaseState GetState(EcsEntity entity)
	- EcsEntity GetCurrentTarget(EcsEntity entity)
	- float GetCurrentDistance(EcsEntity entity)
	- Vector3 GetVelocity(EcsEntity entity)
	- void SetStartConditions(EcsEntity entity, List<IStartConditionConfig> conditions)
	- void SetStopConditions(EcsEntity entity, List<IStopConditionConfig> conditions)
	- void SetCurrentTarget(EcsEntity entity, EcsEntity target)

状态切换与事件派发：
- StartChase 成功 -> IOnChaseStarted
- Pause/Resume -> IOnChasePaused/IOnChaseResumed
- StopChase -> IOnChaseStopped
- 目标变更 -> IOnChaseTargetChanged
- 进入半径 -> IOnChaseEnterRadius
- 达到保持距离 -> IOnChaseKeepDistanceReached
- 丢失对象 -> IOnChaseLostTarget
- 区域越界 -> IOnChaseAreaViolated

### ChaseConfigComponent
- 用途：缓存并暴露 IChaseConfig，供系统高频读取。
- 字段：
	- IChaseConfig Config

#### ChaseConfigSystem
- 功能接口：
	- IChaseConfig GetConfig(EcsEntity entity)
	- void SetConfig(EcsEntity entity, IChaseConfig config)

### ChaseStateComponent（运行时状态）
- 用途：追踪运行态数据与对外状态暴露。
- 字段：
	- ChaseState State
	- float CurrentDistance
	- ChaseKinematics Kinematics

#### ChaseStateSystem
- 功能接口：
	- ChaseState GetState(EcsEntity entity)
	- void SetState(EcsEntity entity, ChaseState state)
	- float GetCurrentDistance(EcsEntity entity)
	- void SetCurrentDistance(EcsEntity entity, float distance)
	- void SetKinematics(EcsEntity entity, in ChaseKinematics kin)

### TargetCandidatesComponent（候选/优先级）
- 用途：维护候选目标、评分与选中目标逻辑。
- 字段：
	- List<long> CandidateIds
	- Dictionary<long, float> Id2Score
	- long SelectedId
	- IPriorityRuleConfig Rule

#### TargetCandidatesSystem
- 功能接口：
	- void SetCandidates(EcsEntity entity, List<EcsEntity> candidates)
	- void Score(EcsEntity entity)
	- EcsEntity Pick(EcsEntity entity)
	- void Invalidate(EcsEntity entity, long targetId)

### AreaLimitComponent（区域限制）
- 用途：限定追踪活动区域并处理越界策略。
- 字段：
	- IChaseAreaConfig Area
	- bool IsOutOfArea

#### AreaLimitSystem
- 功能接口：
	- bool CheckInside(EcsEntity entity, Vector3 pos)
	- void HandleOutOfArea(EcsEntity entity)

### ConditionsComponent（启动/停止条件）
- 用途：以数据形式持有当前生效的启动与停止条件。
- 字段：
	- List<IStartConditionConfig> StartConditions
	- List<IStopConditionConfig> StopConditions

#### ConditionsSystem
- 功能接口：
	- bool ShouldStart(EcsEntity entity)
	- bool ShouldStop(EcsEntity entity)


## 行为流程（概述）
1) ChaseSystem.StartChase -> 校验 StartConditions -> 设置 State=Searching 并派发 IOnChaseStarted。
2) 更新候选 -> Score/SelectTarget -> 目标变更则派发 IOnChaseTargetChanged 并同步 ChaseComponent.CurrentTargetId。
3) Tick：计算距离与速度向量 -> 触发 Enter/Keep/Lost 节点事件；若 StopConditions 满足或越界策略为 StopChase 则 Stop。
4) 目标失效 -> Invalidate -> 根据 AutoReselectOnInvalid 自动重选；若无可用目标，State=Lost 并派发 IOnChaseLostTarget。


## 对外暴露（查询）
- 通过 ChaseSystem/ChaseStateSystem 提供：
	- GetState、GetCurrentTarget、GetCurrentDistance、GetVelocity（或 Kinematics）。


## 备注
- 实体仅承载轻量基础属性；可插拔的复杂功能由组件实现与系统驱动。
- 同类型组件在实体中唯一，实体创建通过父实体 AddChild<T>() 完成。
- 所有流程节点采用 IDispatch 扩展，符合开闭与依赖倒置原则。

