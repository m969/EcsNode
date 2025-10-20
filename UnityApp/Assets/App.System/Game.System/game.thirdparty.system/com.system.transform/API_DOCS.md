## TransformSystem API

### 基本信息

- **命名空间**: `ECSGame`
- **继承关系**: `AComponentSystem<EcsEntity, TransformComponent>`
- **职责**: 提供对实体 `TransformComponent` 的读取与写入工具方法，集中封装常见的坐标、旋转和方向操作。

### 读取方法

#### `TSVector GetPosition(EcsEntity entity)`

- **描述**: 返回实体当前的世界坐标。
- **参数**: `entity` — 目标实体。
- **返回值**: `TSVector`，表示世界坐标。

#### `TSQuaternion GetRotation(EcsEntity entity)`

- **描述**: 获取实体的旋转四元数。
- **参数**: `entity` — 目标实体。
- **返回值**: `TSQuaternion`，表示当前旋转。

#### `TSVector GetForward(EcsEntity entity)`

- **描述**: 获取实体的前向单位向量，通常用于朝向或移动方向判定。
- **参数**: `entity` — 目标实体。
- **返回值**: `TSVector`，表示正向方向。

#### `TSVector GetForecastPosition(EcsEntity entity)`

- **描述**: 获取预测的世界坐标，用于表现或逻辑上提前计算的位置。
- **参数**: `entity` — 目标实体。
- **返回值**: `TSVector`，表示预测坐标。

### 写入方法

#### `void ChangePosition(EcsEntity entity, TSVector position)`

- **描述**: 设置实体的世界坐标。
- **参数**:
	- `entity`: 目标实体。
	- `position`: 新的世界坐标。

#### `void ChangeRotation(EcsEntity entity, TSQuaternion rotation)`

- **描述**: 设置实体的旋转四元数。
- **参数**:
	- `entity`: 目标实体。
	- `rotation`: 新的旋转四元数。

#### `void ChangeForecastPosition(EcsEntity entity, TSVector target)`

- **描述**: 更新实体的预测位置。
- **参数**:
	- `entity`: 目标实体。
	- `target`: 新的预测坐标。

#### `void ChangeForward(EcsEntity entity, TSVector target)`

- **描述**: 设置实体的朝向向量。
- **参数**:
	- `entity`: 目标实体。
	- `target`: 新的朝向向量。

### 使用建议

- 在调用写入方法前，确保目标实体已绑定 `TransformComponent`，避免运行时异常。
- 若需要同步多个属性（如位置与朝向），建议在同一帧内按需调用对应方法，以保持状态一致。
