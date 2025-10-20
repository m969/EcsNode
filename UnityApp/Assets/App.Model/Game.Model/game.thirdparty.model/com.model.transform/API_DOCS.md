TransformComponent API 文档

命名空间：`ECSGame`

基类：`EcsComponent`

依赖：
- `ECS`（实体组件系统框架）
- `TrueSync`（确定性数学库）：`TSVector`、`TSQuaternion`

概览：
- `TransformComponent` 用于描述实体的世界空间位置、旋转与朝向，并提供可选的“预测位置”。
- 该组件不包含更新逻辑，仅存储与派生（例如通过旋转派生 Forward）。

属性
- Position: `TSVector`
	- 读写。
	- 描述：实体在世界空间中的位置。
	- 备注：未显式赋值时为类型默认值（依赖 `TSVector` 的默认构造语义）。

- Rotation: `TSQuaternion`
	- 读写。
	- 描述：实体在世界空间中的旋转（四元数）。
	- 与 Forward 的关系：`Forward == Rotation * TSVector.forward`。

- Forward: `TSVector`
	- 读/写（派生/反推）。
	- get：返回 `Rotation * TSVector.forward`。
	- set：执行 `Rotation = TSQuaternion.LookRotation(value, TSVector.up)`。
	- 注意：
		- 传入的 `value` 不应为零向量；建议使用归一化方向（非零，长度>0）。
		- 设置 `Forward` 仅更新 `Rotation`，不会改变 `Position`。

- ForecastPosition: `TSVector`
	- 读写。
	- 描述：用于插值/外推/网络同步的预测位置（是否启用由上层系统决定）。


契约（简要）
- 输入：
	- 设置 `Position`、`Rotation`、`ForecastPosition` 为有效 `TrueSync` 向量/四元数。
	- 设置 `Forward` 时传入非零方向向量。
- 输出：
	- 读取 `Forward` 时返回由当前 `Rotation` 派生的方向（世界空间）。
- 失败模式（由底层库决定）：
	- `TSQuaternion.LookRotation` 对非法方向（零向量等）可能抛出异常、断言或返回未定义结果（取决于 TrueSync 版本）。

常见用例
- 基于方向驱动旋转：仅通过设置 `Forward` 来统一调控朝向，避免直接计算四元数。
- 网络同步/插值：`Position` 用于权威状态；`ForecastPosition` 用于本地平滑与外推。

变更记录
- v1（当前）：基于 `TransformComponent.cs` 生成的首版 API 文档。

