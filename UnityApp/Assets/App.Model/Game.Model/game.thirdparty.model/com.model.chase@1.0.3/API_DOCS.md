# 追踪模块数据层 API 文档

## 组件

### `ChaseComponent`
- 用途：追踪主体的核心数据入口，记录配置标识、当前目标与暂停状态。
- 属性：
  - `string ConfigId`：绑定的追踪配置标识，供系统或外部查表使用。
  - `long CurrentTargetId`：当前锁定的目标实体 Id，`0` 表示未锁定目标。
  - `bool IsPaused`：是否处于暂停态。
  - `float LastTargetChangeTime`：最近一次目标切换的时间戳，由系统在目标更新时写入。

### `ChaseConfigComponent`
- 用途：缓存追踪相关的半径阈值配置，便于系统层快速读取。
- 属性：
  - `string ConfigId`：配置标识。
  - `float EnterRadius`：进入判定半径。
  - `float ExitRadius`：退出判定半径。
  - `float KeepDistance`：保持距离阈值。
  - `float LostDistance`：丢失距离阈值。

### `ChaseStateComponent`
- 用途：记录追踪流程的实时状态与运动学信息。
- 属性：
  - `ChaseState State`：当前追踪状态，默认 `Idle`。
  - `float CurrentDistance`：与目标的当前距离。
  - `ChaseKinematics Kinematics`：当前运动学数据，包含方向与速度。

## 数据定义

### 枚举
- `ChaseState`：追踪流程的阶段（`Idle`、`Searching`、`Following`、`Intercepting`、`Lost`）。

### 结构体
- `ChaseKinematics`：描述追踪实体的运动学快照，包含方向向量与速度标量；`Velocity` 属性会在读取时将方向归一化后乘以速度得到速度向量。
- `RuntimeSnapshot`：内部结构体（定义于 `ChaseComponent` 文件），用于在系统逻辑中暂存追踪实体与目标的空间信息，包含 `OwnerPosition`、`TargetPosition`、`Distance`、`Direction` 等字段与属性。
