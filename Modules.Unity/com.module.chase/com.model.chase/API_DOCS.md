# 追踪模块数据层 API 文档

## 组件

### `AreaLimitComponent`
- 用途：保存追踪区域配置以及越界状态。
- 属性：
	- `IChaseAreaConfig? Area`：当前使用的活动区域限制。
	- `bool IsOutOfArea`：是否已经越界。

### `ChaseComponent`
- 用途：追踪主体的核心数据入口，关联配置与目标状态。
- 属性：
	- `string ConfigId`：绑定的追踪配置标识。
	- `long CurrentTargetId`：当前锁定的目标实体 Id。
	- `bool IsPaused`：是否处于暂停态。
	- `float LastTargetChangeTime`：最近一次目标切换时间。

### `ChaseConfigComponent`
- 用途：缓存追踪配置对象，便于系统层快速读取。
- 属性：
	- `IChaseConfig? Config`：对应的追踪配置实例。

### `ChaseStateComponent`
- 用途：暴露追踪流程的实时状态与运动学数据。
- 属性：
	- `ChaseState State`：当前追踪状态枚举值。
	- `float CurrentDistance`：与目标的当前距离。
	- `ChaseKinematics Kinematics`：追踪实体的运动学数据。

### `ConditionsComponent`
- 用途：集中管理追踪的启动与停止条件。
- 属性：
	- `List<IStartConditionConfig> StartConditions`：当前生效的启动条件集合。
	- `List<IStopConditionConfig> StopConditions`：当前生效的停止条件集合。

### `TargetCandidatesComponent`
- 用途：维护候选目标及其评分信息。
- 属性：
	- `List<long> CandidateIds`：候选目标实体 Id 列表。
	- `Dictionary<long, float> Id2Score`：候选 Id 对应的评分映射。
	- `long SelectedId`：当前选中的目标 Id。
	- `IPriorityRuleConfig? Rule`：评分所使用的优先级规则。

## 配置接口

### `IChaseConfig`
- 描述追踪行为的完整配置。
- 关键成员：`ConfigId`、`EnterRadius`、`ExitRadius`、`KeepDistance`、`LostDistance`、`AutoReselectOnInvalid`、`CandidateCapacity`、`IChaseAreaConfig AreaLimit`、`List<IStartConditionConfig> StartConditions`、`List<IStopConditionConfig> StopConditions`、`IPriorityRuleConfig PriorityRule`。

### `IChaseAreaConfig`
- 定义活动区域与越界策略。
- 关键成员：`AreaType Type`、`Vector3 Center`、`float Radius`、`List<Vector3> Points`、`OutOfAreaStrategy Strategy`、`float RollbackStep`。

### `IStartConditionConfig`
- 描述启动追踪的条件。
- 关键成员：`string ConditionType`、`Dictionary<string, string> Params`。

### `IStopConditionConfig`
- 描述停止追踪的条件。
- 关键成员：`string ConditionType`、`Dictionary<string, string> Params`。

### `IPriorityRuleConfig`
- 制定候选目标的评分策略。
- 关键成员：`PriorityPolicy Policy`、`Dictionary<string, float> Weights`、`bool HigherIsBetter`。

## 数据定义

### 枚举
- `AreaType`：表示区域限制的形状（`None`、`Circle`、`Polygon`）。
- `ChaseState`：追踪流程阶段（`Idle`、`Searching`、`Following`、`Intercepting`、`Lost`）。
- `OutOfAreaStrategy`：越界后的处理方式（`None`、`Rollback`、`StopChase`）。
- `PriorityPolicy`：候选评分策略类型（`DistanceAsc`、`Custom`）。

### 结构体
- `ChaseKinematics`：包含方向、速度标量，并提供 `Velocity` 只读向量。
- `TargetScore`：封装候选目标 Id 与对应评分。
