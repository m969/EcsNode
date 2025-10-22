# 追踪模块数据层 API 文档

## 组件

### `AreaLimitComponent`
- 用途：保存追踪区域配置以及越界状态标记。
- 属性：
	- `IChaseAreaConfig? Area`：当前生效的区域限制配置，通常由配置系统或外部业务写入。
	- `bool IsOutOfArea`：是否已被 `AreaLimitSystem.CheckInside` 判定为越界。

### `ChaseComponent`
- 用途：追踪主体的核心数据入口，关联配置、目标与暂停态。
- 属性：
	- `string ConfigId`：绑定的追踪配置标识，供外部查表使用。
	- `long CurrentTargetId`：当前锁定的目标实体 Id，`0` 表示无目标。
	- `bool IsPaused`：是否处于暂停态。
	- `float LastTargetChangeTime`：最近一次目标切换的时间戳，由 `ChaseSystem` 通过 `IChaseTimeProvider` 写入。

### `ChaseConfigComponent`
- 用途：缓存追踪配置对象，便于系统层快速读取。
- 属性：
	- `IChaseConfig? Config`：对应的追踪配置实例，`ChaseConfigSystem` 使用时假定已写入非空值。

### `ChaseStateComponent`
- 用途：暴露追踪流程的实时状态与运动学数据。
- 属性：
	- `ChaseState State`：当前追踪状态枚举值，默认 `Idle`。
	- `float CurrentDistance`：与目标的当前距离。
	- `ChaseKinematics Kinematics`：追踪实体的运动学数据，包含方向与速度。

### `ConditionsComponent`
- 用途：集中管理追踪的启动与停止条件。
- 属性：
	- `List<IStartConditionConfig> StartConditions`：当前生效的启动条件集合，由 `ConditionsSystem` 以副本写入。
	- `List<IStopConditionConfig> StopConditions`：当前生效的停止条件集合，由 `ConditionsSystem` 以副本写入。

### `TargetCandidatesComponent`
- 用途：维护候选目标及其评分信息。
- 属性：
	- `List<long> CandidateIds`：候选目标实体 Id 列表，由 `TargetCandidatesSystem.SetCandidates` 填充。
	- `Dictionary<long, float> Id2Score`：候选 Id 对应的最新评分缓存。
	- `long SelectedId`：当前选中的目标 Id，`0` 表示尚未选中。
	- `IPriorityRuleConfig? Rule`：评分所使用的优先级规则，通常来源于配置。

## 配置接口

### `IChaseConfig`
- 描述追踪行为的完整配置。
- 关键成员：`ConfigId`（唯一标识）、`EnterRadius`、`ExitRadius`、`KeepDistance`、`LostDistance`、`AutoReselectOnInvalid`（失效目标是否自动重选）、`CandidateCapacity`（小于等于 0 表示不限）、`IChaseAreaConfig AreaLimit`、`List<IStartConditionConfig> StartConditions`、`List<IStopConditionConfig> StopConditions`、`IPriorityRuleConfig PriorityRule`。

### `IChaseAreaConfig`
- 定义活动区域与越界策略。
- 关键成员：`AreaType Type`、`Vector3 Center`、`float Radius`、`List<Vector3> Points`（多边形顶点）、`OutOfAreaStrategy Strategy`、`float RollbackStep`（回退策略使用的步长）。

### `IStartConditionConfig`
- 描述启动追踪的条件。
- 关键成员：`string ConditionType`（内置：`WithinEnterRadius`、`TargetAvailable` 等）、`Dictionary<string, string> Params`（用于覆盖半径、最小候选数量等参数）。

### `IStopConditionConfig`
- 描述停止追踪的条件。
- 关键成员：`string ConditionType`（内置：`LeaveExitRadius`、`TargetInvalid` 等）、`Dictionary<string, string> Params`（用于覆盖阈值等参数）。

### `IPriorityRuleConfig`
- 制定候选目标的评分策略。
- 关键成员：`PriorityPolicy Policy`、`Dictionary<string, float> Weights`（`Custom` 策略下的指标权重）、`bool HigherIsBetter`（评分越高是否越优）。

## 数据定义

### 枚举
- `AreaType`：表示区域限制的形状（`None`、`Circle`、`Polygon`）。
- `ChaseState`：追踪流程阶段（`Idle`、`Searching`、`Following`、`Intercepting`、`Lost`）。
- `OutOfAreaStrategy`：越界后的处理方式（`None`、`Rollback`、`StopChase`）。
- `PriorityPolicy`：候选评分策略类型（`DistanceAsc`、`Custom`）。

### 结构体
- `ChaseKinematics`：包含方向向量与速度标量，`Velocity` 属性会对方向归一化后乘以速度得到最终速度向量。
- `TargetScore`：封装候选目标 Id 与对应评分，可用于外部缓存或调试评分结果。
