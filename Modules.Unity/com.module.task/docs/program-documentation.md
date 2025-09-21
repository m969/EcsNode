# （任务）模块程序设计文档

> 依据：design-documentation.md；遵循 program.instructions.md 规范；尽量复用现有 Achieve 模块。

## 1. 程序功能概述

- 在 Achieve（达成）模块之上实现任务系统，Task 仅负责“编排”：激活、状态流转、结算（奖励发放）、重置。
- 任务进度与完成性仅由 AchieveItem 决定（唯一真实来源，Single Source of Truth）。
- 状态机：Inactive → InProgress → Claimable → Claimed。
- 命令：激活 Activate、领取 Claim、重置 Reset；查询：列表/详情/进度/状态。
- 事件：TaskActivated / TaskProgressChanged / TaskCompleted / TaskRewardClaimed / TaskReset。
- 奖励由 RewardConfigId 指向外部奖励配置，由奖励系统解释并发放（任务系统不内嵌奖励逻辑）。

常用引用与命名空间示例：

```csharp
using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.TaskModule { /* ... */ }
```

## 2. 数据结构设计

说明：实体与组件仅承载属性数据，不实现方法逻辑；系统类只实现方法逻辑不持有属性。

### 2.1 配置接口

- ITaskConfig（配置接口以整型 Id 为唯一标识，字符串 Key 作为辅助名称标识）
	- int Id：任务配置 Id。
	- string Key：辅助名称 Key。
	- string Name：名称。
	- string Desc：描述。
	- int AchieveItemId：绑定的 AchieveItem（唯一进度来源）。
	- int RewardConfigId：奖励配置 Id（交由奖励系统解释与发放）。

### 2.2 实体（partial，继承 EcsEntity）

- TaskItem（运行时实例）
	- int Id：任务实例 Id（可与配置 Id 一致）。
	- int ConfigId：任务配置 Id。
	- int AchieveItemId：绑定 AchieveItem。
	- TaskState State：当前状态。
	- float Progress：进度（来源 AchieveItem，可缓存）。
	- bool RewardClaimed：是否已发放奖励（与 Claimed 冗余，用于校验）。
	- TaskActivateMode ActivateMode：激活方式（当前仅 Manual）。
	- long ActivatedAt：激活时间戳（可选）。
	- long CompletedAt：达成时间戳（可选）。
	- long ClaimedAt：领取时间戳（可选）。

命名空间：`ECSGame.TaskModule`；实体继承 `EcsEntity` 且标记为 partial，便于后续非侵入式扩展。

### 2.3 组件（继承 EcsComponent）

- TaskListComponent（任务索引与集合）
	- Dictionary<int, TaskItem> Id2Entities：taskId → TaskItem。
	- Dictionary<int, List<TaskItem>> ConfigId2Entities：configId → TaskItem[]（如一配多实例；单实例场景可退化）。
	- Dictionary<int, List<int>> AchieveItemId2TaskIds：achieveItemId → taskId[]（便于由 Achieve 事件反查任务）。
	- Queue<int> RecentChangedTaskIds（可选）：最近变更队列，便于前端增量刷新。

组件挂载位置：建议挂在模块根实体（如 Game/Domain Root）上，具体由集成方决定。

### 2.4 补充类型

- 枚举：
	- TaskState { Inactive = 0, InProgress = 1, Claimable = 2, Claimed = 3 }
	- TaskActivateMode { Manual = 0 }

## 3. 系统业务设计

系统类不持有属性，仅实现方法逻辑。业务方法一律为静态方法；只传实体与必要参数，组件在方法内获取。

### 3.1 实体系统：TaskItemSystem

继承：`AEntitySystem<TaskItem>`；实现生命周期接口：
- IAwake<TaskItem>, IInit<TaskItem>, IAfterInit<TaskItem>, IEnable<TaskItem>, IDisable<TaskItem>, IDestroy<TaskItem>

静态业务方法（摘要）：
- Create(EcsEntity parent, ITaskConfig config)
	- 创建 TaskItem 实体并与 AchieveItemId 绑定，初始化为 Inactive。
- Activate(TaskItem task)
	- 状态 Inactive → InProgress；派发 IOnTaskActivated。
- EvaluateState(TaskItem task, bool completed) : TaskState
	- 根据 Achieve 完成态与当前 State 计算新状态（无额外分支）。
- Claim(TaskItem task, Func<int, bool> tryGrantReward) : bool
	- 状态 Claimable → Claimed；调用外部奖励发放（传入 RewardConfigId），成功后派发 IOnTaskRewardClaimed。
- Reset(TaskItem task, bool resetToInactive = true)
	- 重置至 Inactive（默认）；不修改 Achieve 进度；派发 IOnTaskReset。
- GetProgress(TaskItem task) : float
	- 只读获取进度（由 Achieve 计算，可读取/同步缓存）。
- GetState(TaskItem task) : TaskState
	- 返回当前状态。

生命周期要点：
- Awake/Init：填充默认状态与时间戳；不触发外部副作用。
- Enable/Disable：用于订阅/退订 Achieve 相关事件（视集成方式）。
- Destroy：释放与 Task 相关的订阅关系。

### 3.2 列表组件系统：TaskListSystem

继承：`AComponentSystem<EcsEntity, TaskListComponent>`；实现生命周期接口：
- IAwake<EcsEntity, TaskListComponent>, IInit<...>, IAfterInit<...>, IEnable<...>, IDisable<...>, IDestroy<...>

静态业务方法（摘要）：
- Add(EcsEntity root, TaskItem task) : bool
	- 写入各字典索引；去重保护。
- Remove(EcsEntity root, int taskId) : bool
	- 从索引移除并返回是否成功。
- Get(EcsEntity root, int taskId) : TaskItem
- GetByAchieveItem(EcsEntity root, int achieveItemId) : IReadOnlyList<TaskItem>
- GetAll(EcsEntity root) : IReadOnlyList<TaskItem>
- OnAchieveProgressChanged(EcsEntity root, int achieveItemId, float progress, bool completed)
	- 反查关联 Task 并递进：更新各 TaskItem.Progress；当 completed 时使其状态 InProgress → Claimable，派发 IOnTaskProgressChanged 与 IOnTaskCompleted。
- Activate(EcsEntity root, int taskId) : bool
	- 便捷封装：查 Task 后调用 TaskItemSystem.Activate。
- Claim(EcsEntity root, int taskId, Func<int, bool> tryGrantReward) : bool
	- 便捷封装：查 Task 后调用 TaskItemSystem.Claim。
- Reset(EcsEntity root, int taskId, bool resetToInactive = true) : bool
	- 便捷封装：查 Task 后调用 TaskItemSystem.Reset。

说明：为满足“对外以 taskId 为主要入口”的易用性，这里提供 Id 封装，但核心逻辑仍集中在实体系统中，保持职责单一。

### 3.3 系统派发事件接口（继承 IDispatch）

为遵循开闭与依赖倒置，系统通过事件派发对外扩展：

- IOnTaskActivated：`void OnTaskActivated(EcsEntity entity, int taskId)`
	- 任务激活成功后触发。
- IOnTaskProgressChanged：`void OnTaskProgressChanged(EcsEntity entity, int taskId, float progress)`
	- 任务进度变化时触发（由 Achieve 事件驱动）。
- IOnTaskCompleted：`void OnTaskCompleted(EcsEntity entity, int taskId)`
	- 任务可领取（完成）时触发。
- IOnTaskRewardClaimed：`void OnTaskRewardClaimed(EcsEntity entity, int taskId)`
	- 奖励发放完成后触发。
- IOnTaskReset：`void OnTaskReset(EcsEntity entity, int taskId)`
	- 任务被重置时触发。

事件派发示例（伪代码）：
```csharp
taskEntity.Dispatch<IOnTaskActivated>(s => s.OnTaskActivated(taskEntity, taskEntity.Id));
```

## 4. 查询与命令接口（对外）

- 查询：
	- GetTask(EcsEntity root, int taskId) : TaskItem
	- GetProgress(EcsEntity root, int taskId) : float
	- GetState(EcsEntity root, int taskId) : TaskState
	- ListTasks(EcsEntity root) : IReadOnlyList<TaskItem>
- 命令：
	- Activate(EcsEntity root, int taskId) : bool
	- Claim(EcsEntity root, int taskId, Func<int, bool> tryGrantReward) : bool
	- Reset(EcsEntity root, int taskId, bool resetToInactive = true) : bool

## 5. 与 Achieve 模块的交互

- 只读依赖 Achieve 的进度与完成性：Task 不修改 Achieve 数据，避免双写与分叉。
- 事件绑定：
	- 至少订阅 Achieve 完成事件（示例模块提供 IOnAchieveCompleted）。
	- 若存在进度事件，可同步进度；否则可通过 AchieveConditionListComponent 聚合进度并按需轮询/触发。
- 状态变更规则（消除特殊情况）：
	- Inactive：未激活不派发进度；Activate 后进入 InProgress。
	- InProgress：当 Achieve 完成时转为 Claimable。
	- Claimable：Claim 成功后转为 Claimed，并发放奖励。
	- Claimed：终止态；Reset 可回到 Inactive（不影响 Achieve 进度）。

## 6. 设计原则与约束

- 单一职责：
	- TaskItemSystem 仅处理单个任务的状态流转与奖励结算调用。
	- TaskListSystem 仅处理集合索引与对外 Id 入口封装。
- 开闭原则：
	- 外部扩展通过 IDispatch 事件；不修改核心系统代码即可新增行为（UI 刷新、引导、日志等）。
- 依赖倒置：
	- 奖励发放通过回调/接口注入（tryGrantReward）；任务系统不直接依赖具体奖励实现。
- 创建约束：
	- 实体创建统一通过 `AddChild<T>()` 添加到父实体下。
	- 同类型组件在实体中唯一（TaskListComponent 仅挂一份）。

## 7. 时序与状态机（简要）

1) Activate(task) → State: Inactive → InProgress → IOnTaskActivated。
2) Achieve 完成 → OnAchieveProgressChanged(..., completed=true) → State: InProgress → Claimable → IOnTaskCompleted（期间持续派发 IOnTaskProgressChanged）。
3) Claim(task) → State: Claimable → Claimed → 发放奖励（基于 RewardConfigId）→ IOnTaskRewardClaimed。
4) Reset(task) → State: Claimed/Claimable/InProgress → Inactive → IOnTaskReset。

## 8. 备注与后续扩展

- 未来可扩展自动激活模式（TaskActivateMode 增补），在不破坏现有接口的前提下实现。
- 可选增加 Task 组/链路（依赖关系）作为上层编排，不改变 TaskItem 的最小状态集合。

— 完 —
