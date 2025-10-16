# 追踪模块系统层 API 文档

## 核心系统

### `ChaseSystem`
- 用途：协调追踪流程的主系统，串联候选管理、条件判定、状态同步与事件派发。
- 静态方法：
	- `StartChase(EcsEntity entity, EcsEntity? target = null)`：校验启动条件后启动追踪，自动选择或沿用目标，并派发 `IOnChaseStarted`。
	- `PauseChase(EcsEntity entity)`：切换为暂停态，重置状态为 `Idle` 并派发 `IOnChasePaused`。
	- `ResumeChase(EcsEntity entity)`：从暂停态恢复，按当前目标决定状态并派发 `IOnChaseResumed`。
	- `StopChase(EcsEntity entity, string reason = "manual")`：清空目标与运动数据，状态归零并派发 `IOnChaseStopped`。
	- `Tick(EcsEntity entity, float deltaTime)`：逐帧更新追踪；包含停止条件校验、越界检测、目标重选、距离与速度更新，以及触发进入/保持/丢失事件。
	- `UpdateTargetCandidates(EcsEntity entity, List<EcsEntity> candidates)`：写入候选列表、重算评分并在需要时触发自动重选。
	- `SelectTarget(EcsEntity entity)`：根据评分挑选最佳候选，若失败则进入 `Searching` 状态。
	- `GetState(EcsEntity entity)`：读取当前追踪状态。
	- `GetCurrentTarget(EcsEntity entity)`：返回当前目标实体，内部通过 `IChaseTargetResolver` 解析 Id。
	- `HasActiveTarget(EcsEntity entity)`：判断是否存在有效目标。
	- `GetCurrentDistance(EcsEntity entity)`：返回最新追踪距离。
	- `GetVelocity(EcsEntity entity)`：返回追踪实体的速度向量。
	- `SetStartConditions(EcsEntity entity, List<IStartConditionConfig> conditions)`：转调 `ConditionsSystem` 更新启动条件。
	- `SetStopConditions(EcsEntity entity, List<IStopConditionConfig> conditions)`：转调 `ConditionsSystem` 更新停止条件。
	- `SetCurrentTarget(EcsEntity entity, EcsEntity target)`：强制切换目标并派发 `IOnChaseTargetChanged`。
- 事件触发：`IOnChaseStarted`、`IOnChasePaused`、`IOnChaseResumed`、`IOnChaseStopped`、`IOnChaseTargetChanged`、`IOnChaseEnterRadius`、`IOnChaseKeepDistanceReached`、`IOnChaseLostTarget`、`IOnChaseAreaViolated`。
- 依赖扩展：需要外部实现 `IChaseRuntimeProvider`、`IChaseTargetResolver`、`IChaseTimeProvider`、`IChaseMetricProvider` 和 `IChaseConditionEvaluator` 以提供运行态数据、目标解析、时间与自定义条件/指标。

### `ChaseStateSystem`
- 用途：维护追踪实体的状态机与运动学数据。
- 静态方法：
	- `GetState` / `SetState`：读取或写入 `ChaseStateComponent.State`。
	- `GetCurrentDistance` / `SetCurrentDistance`：维护当前与目标的距离。
	- `GetKinematics` / `SetKinematics`：同步 `ChaseKinematics`（含方向与速度）。

### `ChaseConfigSystem`
- 用途：集中读写 `ChaseConfigComponent` 中的运行配置。
- 静态方法：
	- `GetConfig(EcsEntity entity)`：读取绑定的 `IChaseConfig`。
	- `SetConfig(EcsEntity entity, IChaseConfig config)`：写入配置实例。

### `ConditionsSystem`
- 用途：管理追踪的启动与停止条件并提供判定逻辑。
- 静态方法：
	- `SetStartConditions` / `SetStopConditions`：以副本形式替换条件集合。
	- `ShouldStart(EcsEntity entity)`：全部启动条件通过时返回 `true`，为空集时默认为通过。
	- `ShouldStop(EcsEntity entity)`：任意停止条件满足时返回 `true`，为空集时默认继续。
- 内置条件：支持 `WithinEnterRadius`、`TargetAvailable`、`LeaveExitRadius`、`TargetInvalid`，其余条件转发给 `IChaseConditionEvaluator` 扩展。

### `TargetCandidatesSystem`
- 用途：维护候选目标 Id、评分及自动挑选逻辑。
- 静态方法：
	- `SetCandidates(EcsEntity entity, List<EcsEntity> candidates)`：重置候选列表，按配置容量截断，并缓存优先级规则。
	- `Score(EcsEntity entity)`：基于当前 `IPriorityRuleConfig` 重算所有候选评分。
	- `Pick(EcsEntity entity)`：依据评分与升降序策略选出最佳目标并返回解析后的实体。
	- `Invalidate(EcsEntity entity, long targetId)`：移除失效目标并视配置决定是否重选、回写到 `ChaseSystem`。
	- `HasCandidates` / `GetCandidateCount` / `Contains`：提供候选集合状态查询。
- 评分策略：内置支持 `PriorityPolicy.DistanceAsc` 与 `PriorityPolicy.Custom`，额外指标通过 `IChaseMetricProvider` 获取。

### `AreaLimitSystem`
- 用途：校验追踪实体是否越界并执行策略。
- 静态方法：
	- `CheckInside(EcsEntity entity, Vector3 position)`：根据区域类型检测是否越界，并更新 `AreaLimitComponent.IsOutOfArea`。
	- `HandleOutOfArea(EcsEntity entity)`：在越界时派发 `IOnChaseAreaViolated`，依据策略保持、回退或调用 `ChaseSystem.StopChase`。

## 事件派发接口
- `IOnChaseStarted`：追踪启动回调，携带初始目标。
- `IOnChasePaused`：追踪进入暂停态。
- `IOnChaseResumed`：追踪从暂停态恢复。
- `IOnChaseStopped`：追踪结束，附带停止原因。
- `IOnChaseTargetChanged`：当前目标变更时通知旧/新目标。
- `IOnChaseEnterRadius`：目标进入配置的进入半径。
- `IOnChaseKeepDistanceReached`：达到保持距离阈值。
- `IOnChaseLostTarget`：目标失效或丢失。
- `IOnChaseAreaViolated`：触发区域越界策略通知。

## 扩展接口
- `IChaseRuntimeProvider`：提供追踪实体与目标的空间位置与相对速度。
- `IChaseTargetResolver`：基于 Id 解析实际 `EcsEntity`。
- `IChaseTimeProvider`：提供追踪流程使用的时间戳。
- `IChaseMetricProvider`：提供距离及自定义评分指标。
- `IChaseConditionEvaluator`：扩展启动/停止条件判定逻辑。
