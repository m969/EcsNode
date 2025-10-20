# 追踪模块系统层 API 文档

## 核心系统

### `ChaseSystem`
- 用途：协调追踪流程的主系统，串联条件判定、候选管理、状态同步与事件派发。
- 静态方法：
	- `StartChase(EcsEntity entity, EcsEntity? target = null)`：先调用 `ConditionsSystem.ShouldStart`，未通过直接返回；重置暂停标记，按“传入目标 → 已锁定目标 → 自动挑选”顺序确定目标，更新状态为 `Following` 或 `Searching`，记录目标切换时间并派发 `IOnChaseStarted`。
	- `PauseChase(EcsEntity entity)`：若已处于暂停态将直接返回；否则设置 `IsPaused`、复位状态为 `Idle` 并派发 `IOnChasePaused`。
	- `ResumeChase(EcsEntity entity)`：仅在暂停态下生效，依据当前目标决定后续状态并派发 `IOnChaseResumed`。
	- `StopChase(EcsEntity entity, string reason = "manual")`：清空目标、重置暂停标记，归零距离与运动学数据，状态重置为 `Idle` 并派发 `IOnChaseStopped`。
	- `Tick(EcsEntity entity, float deltaTime)`：逐帧驱动逻辑，依次执行停止条件校验（命中时调用 `StopChase("condition")`）、区域检测及策略处理、暂停态短路、候选重选、运行时数据采样、距离/速度刷新以及进入/保持/丢失事件的派发。
	- `UpdateTargetCandidates(EcsEntity entity, List<EcsEntity> candidates)`：重建候选列表并评分，若当前目标缺席且配置开启自动重选，将主动尝试选择新目标。
	- `SelectTarget(EcsEntity entity)`：触发重评分并依据优先级选出最佳候选；成功时调用 `SetCurrentTarget` 并返回目标，失败则清空目标并将状态置为 `Searching`。
	- `GetState(EcsEntity entity)`：读取 `ChaseStateSystem` 中维护的当前状态。
	- `GetCurrentTarget(EcsEntity entity)`：通过 `IChaseTargetResolver` 解析 `CurrentTargetId` 并返回实体，解析失败时返回 `null`。
	- `HasActiveTarget(EcsEntity entity)`：判断当前是否存在有效目标 Id。
	- `GetCurrentDistance(EcsEntity entity)`：返回最新的追踪距离。
	- `GetVelocity(EcsEntity entity)`：返回 `ChaseStateSystem.GetKinematics(entity).Velocity`。
	- `SetStartConditions(EcsEntity entity, List<IStartConditionConfig> conditions)` / `SetStopConditions(...)`：委托 `ConditionsSystem` 以副本方式替换条件集合。
	- `SetCurrentTarget(EcsEntity entity, EcsEntity target)`：更新 `CurrentTargetId` 与 `LastTargetChangeTime`，派发 `IOnChaseTargetChanged` 并将状态切换为 `Following`。
- 事件触发：`IOnChaseStarted`、`IOnChasePaused`、`IOnChaseResumed`、`IOnChaseStopped`、`IOnChaseTargetChanged`、`IOnChaseEnterRadius`、`IOnChaseKeepDistanceReached`、`IOnChaseLostTarget`、`IOnChaseAreaViolated`。
- 外部依赖：需要实现 `IChaseRuntimeProvider`（位置与相对速度）、`IChaseTargetResolver`（Id 转实体）、`IChaseTimeProvider`（时间戳）、`IChaseMetricProvider`（评分指标）和 `IChaseConditionEvaluator`（扩展条件）。

### `ChaseStateSystem`
- 用途：维护追踪过程的状态机与运动学快照。
- 静态方法：
	- `GetState(EcsEntity entity)` / `SetState(...)`：读取或写入 `ChaseStateComponent.State`。
	- `GetCurrentDistance(EcsEntity entity)` / `SetCurrentDistance(...)`：同步 `ChaseStateComponent.CurrentDistance`。
	- `GetKinematics(EcsEntity entity)` / `SetKinematics(...)`：读取或写入 `ChaseKinematics`，支持 `in` 传参减少拷贝。

### `ChaseConfigSystem`
- 用途：集中管理 `ChaseConfigComponent` 中的运行配置。
- 静态方法：
	- `GetConfig(EcsEntity entity)`：返回绑定的 `IChaseConfig`（假定已完成写入）。
	- `SetConfig(EcsEntity entity, IChaseConfig config)`：写入配置实例，为后续系统提供访问入口。

### `ConditionsSystem`
- 用途：管理追踪的启动与停止条件并提供内置判定。
- 静态方法：
	- `SetStartConditions(EcsEntity entity, List<IStartConditionConfig> conditions)` / `SetStopConditions(...)`：以副本形式替换组件中的条件列表，避免外部引用被意外修改。
	- `ShouldStart(EcsEntity entity)`：无启动条件时默认允许；内置 `WithinEnterRadius`（支持 `radius` 参数覆盖默认进入半径）与 `TargetAvailable` 判定。
	- `ShouldStop(EcsEntity entity)`：无停止条件时默认继续；内置 `LeaveExitRadius`（支持 `radius` 参数覆盖默认退出半径）与 `TargetInvalid` 判定。
- 扩展支持：其他条件类型将转发给 `IChaseConditionEvaluator` 供外部扩展。

### `TargetCandidatesSystem`
- 用途：维护候选目标 Id、评分与选取策略。
- 静态方法：
	- `SetCandidates(EcsEntity entity, List<EcsEntity> candidates)`：清空旧数据，缓存配置中的 `PriorityRule`，并依据 `CandidateCapacity` 截断候选 Id。
	- `Score(EcsEntity entity)`：使用当前优先级规则为每个候选 Id 生成评分；`DistanceAsc` 通过取反距离实现“距离越近评分越高”，`Custom` 按权重与 `IChaseMetricProvider` 提供的指标累加。
	- `Pick(EcsEntity entity)`：依据评分及 `HigherIsBetter` 选择最佳目标，更新 `SelectedId`，并通过 `IChaseTargetResolver` 返回解析后的实体，若解析失败返回 `null`。
	- `Invalidate(EcsEntity entity, long targetId)`：移除失效目标；若该目标即当前选中目标且配置允许自动重选，将重新评分并尝试通过 `ChaseSystem.SetCurrentTarget` 回写新目标。
	- `HasCandidates(EcsEntity entity)` / `GetCandidateCount(...)` / `Contains(...)`：提供候选集合状态查询。
- 注意事项：所有评分缓存于 `Id2Score`，如需重新计算需显式调用 `Score`。

### `AreaLimitSystem`
- 用途：校验追踪实体是否越界并执行策略。
- 静态方法：
	- `CheckInside(EcsEntity entity, Vector3 position)`：依据区域类型检测位置是否在限制内，同时更新 `AreaLimitComponent.IsOutOfArea` 并返回检测结果。
	- `HandleOutOfArea(EcsEntity entity)`：当越界标记为真时派发 `IOnChaseAreaViolated`；`None` 策略会清除越界标记，`Rollback` 交由外部依据 `RollbackStep` 执行回退，`StopChase` 会直接调用 `ChaseSystem.StopChase("area_violation")`。

## 事件派发接口
- `IOnChaseStarted`：追踪启动回调，携带判定后的初始目标。
- `IOnChasePaused`：追踪进入暂停态。
- `IOnChaseResumed`：追踪从暂停态恢复。
- `IOnChaseStopped`：追踪结束，附带停止原因。
- `IOnChaseTargetChanged`：当前目标变更时通知旧/新目标，若无目标则传入 `null`。
- `IOnChaseEnterRadius`：目标进入配置的进入半径。
- `IOnChaseKeepDistanceReached`：达到保持距离阈值。
- `IOnChaseLostTarget`：目标失效或距离达到丢失阈值时触发。
- `IOnChaseAreaViolated`：触发区域越界策略通知。

## 扩展接口
- `IChaseRuntimeProvider`：提供追踪实体与目标的空间位置与相对速度，供 `Tick` 计算距离与方向。
- `IChaseTargetResolver`：基于 Id 解析实际 `EcsEntity`，影响目标锁定与候选解析。
- `IChaseTimeProvider`：提供时间戳，用于写入 `LastTargetChangeTime` 等运行时信息。
- `IChaseMetricProvider`：提供距离及自定义评分指标，参与候选评分与优先级计算。
- `IChaseConditionEvaluator`：扩展启动/停止条件判定逻辑，实现业务自定义条件。
