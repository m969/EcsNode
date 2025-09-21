# （任务）模块功能设计文档

> 依据：require-documentation.md；尽量复用现有 Achieve 模块（game.thirdparty.module/com.system.achieve 与 com.model.achieve）。

## 模块功能概述

- 在 Achieve（达成）模块之上实现任务系统，Task 仅负责“编排”：激活、状态流转、结算（奖励发放）、重置。
- 任务进度与完成性仅由 AchieveItem 决定（唯一真实来源，Single Source of Truth）。
- 状态机：未激活(Inactive) → 进行中(InProgress) → 可领取(Claimable) → 已领取(Claimed)。
- 操作：激活、领取、重置；查询：列表/详情/进度；事件：TaskActivated/TaskProgressChanged/TaskCompleted/TaskRewardClaimed/TaskReset。
- 奖励不内嵌在任务配置中，改为使用 RewardConfigId 引用外部奖励配置，由奖励系统解释并发放。

## 配置接口设计

### ITaskConfig
- 用途：定义任务的静态配置，绑定唯一 AchieveItem，通过奖励配置 Id 引用奖励。
- 字段：
	- int Id：任务配置 Id。
	- string Name：名称。
	- string Desc：描述。
	- int AchieveItemId：绑定的 AchieveItem（唯一进度来源）。
	- int RewardConfigId：奖励配置 Id（由外部奖励系统解析与发放）。

## 其他类型补充

### 流程节点派发接口（一个节点一个接口）
- ITaskActivated：void OnTaskActivated(int taskId)
- ITaskProgressChanged：void OnTaskProgressChanged(int taskId, float progress)
- ITaskCompleted：void OnTaskCompleted(int taskId)
- ITaskRewardClaimed：void OnTaskRewardClaimed(int taskId)
- ITaskReset：void OnTaskReset(int taskId)

### 基础数据类型补充
- 无；奖励由 RewardConfigId 指向外部奖励配置（由奖励系统解释与发放）。

### 枚举补充
- enum TaskState { Inactive = 0, InProgress = 1, Claimable = 2, Claimed = 3 }
- enum TaskActivateMode { Manual = 0 } // 当前需求仅手动激活

## 实体设计

### 实体：TaskItem（运行时实例）
- 用途：描述一个任务的运行时状态，与 AchieveItem 建立绑定。
- 字段：
	- int Id：任务实例 Id（与配置 Id 一致或运行时分配，按项目规范）。
	- int ConfigId：任务配置 Id（等于 Id 时可冗余）。
	- int AchieveItemId：绑定 AchieveItem。
	- TaskState State：当前状态。
	- float Progress：进度（来源 AchieveItem，读取时计算/缓存）。
	- bool RewardClaimed：是否已发放奖励（与 State=Claimed 一致冗余，便于校验）。
	- TaskActivateMode ActivateMode：激活方式（当前仅 Manual）。
	- long ActivatedAt：激活时间戳（可选）。
	- long CompletedAt：达成时间戳（可选）。
	- long ClaimedAt：领取时间戳（可选）。

### 实体系统：TaskItemSystem
- 功能接口：
	- bool Activate(int taskId)：未激活 → 进行中；派发 ITaskActivatedEvent。
	- bool Claim(int taskId)：可领取 → 已领取；派发 ITaskRewardClaimedEvent；依据 RewardConfigId 调用外部奖励系统发放奖励。
	- bool Reset(int taskId, bool resetToInactive = true)：重置任务状态（默认回到未激活）；派发 ITaskResetEvent。
	- void OnAchieveProgressChanged(int achieveItemId, float progress, bool completed)：由 Achieve 事件回调驱动，更新 TaskItem 进度与状态，并在完成时派发 ITaskCompletedEvent，同时持续派发 ITaskProgressChangedEvent。
	- TaskState EvaluateState(TaskItem task, bool completed)：根据 Achieve 完成态与当前 State 计算新状态（无额外特殊分支）。

## 列表组件设计

### TaskListComponent（用于存储和管理任务实体）
- Id2Entities：Dictionary<int, TaskItem>
- ConfigId2Entities：Dictionary<int, List<TaskItem>>（若一配多实例，否则可留空/退化为直指向）
- AchieveItemId2TaskIds：Dictionary<int, List<int>> 用于由 Achieve 事件高效反查关联任务。
- 其他：可加入最近变更的环形缓冲，用于前端增量刷新（可选）。

### 列表组件系统：TaskListSystem
- 功能接口：
	- TaskItem Get(int taskId)
	- IReadOnlyList<TaskItem> GetByAchieveItem(int achieveItemId)
	- IReadOnlyList<TaskItem> GetAll()
	- bool Add(TaskItem task)
	- bool Remove(int taskId)

## 组件设计（当前阶段暂无额外运行时组件，留空）

### 与 Achieve 模块的交互（关键路径）
- 只读依赖 Achieve 的进度：
	- 通过 achieveItemId 监听 AchieveEvents（如 AchieveItem 进度/完成事件）。
	- Task 不直接修改 Achieve 数据，避免双写与状态分叉。
- 状态变更规则（消除特殊情况）：
	- Inactive：未激活不派发进度；Activate 后立即进入 InProgress。
	- InProgress：当 Achieve 完成时变为 Claimable。
	- Claimable：Claim 后变为 Claimed，同时发放奖励。
	- Claimed：终止态；Reset 可回到 Inactive（不影响 Achieve 进度，除非外部显式重置 Achieve）。

## 查询与命令接口（供外部系统调用）
- 查询：
	- TaskItem GetTask(int taskId)
	- float GetProgress(int taskId)
	- TaskState GetState(int taskId)
	- IReadOnlyList<TaskItem> ListTasks()
- 命令：
	- bool Activate(int taskId)
	- bool Claim(int taskId)
	- bool Reset(int taskId)

## 事件对齐与扩展点
- 统一由 TaskItemSystem 派发上述事件接口，外部可实现订阅以解耦奖励系统、UI 刷新、引导等。
- 与 AchieveEvents 的绑定在系统初始化时注入（依赖注入），便于测试与替换。

## 数据与复杂度说明（设计取舍）
- 数据结构：TaskItem 最小化持有状态，只读引用 AchieveItem；去除“判定逻辑”重复，避免分叉来源。
- 消除特殊情况：用状态机直转移替代多层 if/else；完成性只看 Achieve 完成态。
- 向后兼容：不修改 Achieve 的对外语义；Task 仅订阅事件与读取。
- 实用性：当前仅支持手动激活，后续可在不破坏现有接口前提下扩展自动激活模式。

## 简要时序
1) Activate(taskId) → State: Inactive → InProgress → ITaskActivatedEvent。
2) Achieve 完成 → OnAchieveProgressChanged(..., completed=true) → State: InProgress → Claimable → ITaskCompletedEvent。
3) Claim(taskId) → State: Claimable → Claimed → 发放 Rewards → ITaskRewardClaimedEvent。
	改为：Claim(taskId) → State: Claimable → Claimed → 发放奖励（基于 RewardConfigId）→ ITaskRewardClaimedEvent。
4) Reset(taskId) → State: Claimed/Claimable/InProgress → Inactive → ITaskResetEvent（默认不影响 Achieve 数据）。

## 与第三方模块目录约定
- 第三方模块目录：game.thirdparty.module
- 依赖模块：Achieve（com.model.achieve / com.system.achieve），主要依赖 AchieveItem 与 AchieveEvents。

