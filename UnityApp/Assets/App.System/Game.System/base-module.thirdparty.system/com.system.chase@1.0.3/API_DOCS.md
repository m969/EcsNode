# 追踪模块系统层 API 文档

## 核心系统

### `ChaseSystem`
- 用途：提供追踪主流程的状态管理与事件派发，围绕 `ChaseComponent`、`ChaseStateComponent` 协调暂停、目标与距离数据。
- 静态方法：
  - `StartChase(EcsEntity entity, EcsEntity? target = null)`：清除暂停标记并尝试锁定传入或已有目标；若未解析出有效目标则清空锁定并将状态设置为 `Searching`，最终派发 `IOnChaseStarted`。
  - `PauseChase(EcsEntity entity)`：若已暂停则直接返回；否则写入 `IsPaused`，状态置为 `Idle`，随后派发 `IOnChasePaused`。
  - `ResumeChase(EcsEntity entity)`：仅在暂停态下生效，恢复 `IsPaused`，依据当前目标选择 `Following` 或 `Searching` 状态，并派发 `IOnChaseResumed`。
  - `StopChase(EcsEntity entity, string reason = "manual")`：清除目标与暂停标记，将状态恢复为 `Idle`，距离归零、运动学重置为静止，最后派发 `IOnChaseStopped`。
  - `Tick(EcsEntity entity, float deltaTime)`：跳过暂停态；无有效目标时写入 `Searching` 状态并重置距离；存在目标时采样 `RuntimeSnapshot`，解析失败触发 `HandleLostTarget`，成功则更新距离、速度和状态，按阈值派发 `IOnChaseEnterRadius`、`IOnChaseKeepDistanceReached`，超出丢失距离时触发 `IOnChaseLostTarget`。
  - `GetState(EcsEntity entity)`：返回 `ChaseStateSystem.GetState(entity)`。
  - `GetCurrentTarget(EcsEntity entity)`：通过 `ActorListSystem.GetActor` 解析当前锁定 Id，解析失败返回 `null`。
  - `HasActiveTarget(EcsEntity entity)`：判断 `CurrentTargetId` 是否大于 0（不保证实体仍然存在）。
  - `GetCurrentDistance(EcsEntity entity)`：返回 `ChaseStateSystem.GetCurrentDistance(entity)`。
  - `GetVelocity(EcsEntity entity)`：返回 `ChaseStateSystem.GetKinematics(entity).Velocity`。
  - `SetCurrentTarget(EcsEntity entity, EcsEntity target)`：更新锁定 Id，刷新 `LastTargetChangeTime`（通过 `IChaseTimeProvider` 获取），派发 `IOnChaseTargetChanged` 并将状态改为 `Following`。
- 事件触发：`IOnChaseStarted`、`IOnChasePaused`、`IOnChaseResumed`、`IOnChaseStopped`、`IOnChaseTargetChanged`、`IOnChaseEnterRadius`、`IOnChaseKeepDistanceReached`、`IOnChaseLostTarget`。
- 外部依赖：依赖 `ChaseConfigSystem`、`ChaseStateSystem`，并通过 `ActorListSystem`、`TransformSystem` 获取世界数据；需要实现 `IChaseTimeProvider` 以提供时间戳。

### `ChaseStateSystem`
- 用途：对 `ChaseStateComponent` 提供读写封装，维护状态机、距离与运动学数据。
- 静态方法：
  - `GetState(EcsEntity entity)` / `SetState(EcsEntity entity, ChaseState state)`：读写当前追踪状态。
  - `GetCurrentDistance(EcsEntity entity)` / `SetCurrentDistance(EcsEntity entity, float distance)`：读写当前追踪距离。
  - `SetKinematics(EcsEntity entity, in ChaseKinematics kinematics)` / `GetKinematics(EcsEntity entity)`：读写当前运动学快照。

### `ChaseConfigSystem`
- 用途：集中维护 `ChaseConfigComponent` 中的运行参数，供追踪主流程使用。
- 静态方法：
  - `GetConfig(EcsEntity entity)`：返回配置组件实例，以读取配置标识及半径、距离阈值。
  - `SetConfig(EcsEntity entity, string configId, float enterRadius, float exitRadius, float keepDistance, float lostDistance)`：批量写入配置标识和进入/退出/保持/丢失距离阈值。

## 事件派发接口
- `IOnChaseStarted`：追踪启动时触发，携带初始化目标（可能为 `null`）。
- `IOnChasePaused`：追踪暂停时触发。
- `IOnChaseResumed`：追踪从暂停状态恢复时触发。
- `IOnChaseStopped`：追踪停止时触发，附带停止原因。
- `IOnChaseTargetChanged`：当前锁定目标发生变更时触发，提供旧目标与新目标引用。
- `IOnChaseEnterRadius`：当前目标进入配置的进入半径时触发。
- `IOnChaseKeepDistanceReached`：达到保持距离阈值时触发。
- `IOnChaseLostTarget`：解析不到目标或距离超出丢失阈值时触发，携带丢失前的目标。

## 扩展接口
- `IChaseTimeProvider`：提供时间戳，由 `ChaseSystem` 在目标切换时调用以刷新 `LastTargetChangeTime`。
