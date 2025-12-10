# Attack Module - Model Layer API

本模块定义了攻击系统的数据结构、组件、接口与基础类型。

## 实体 (Entities)

### `AttackAction`
表示一次普攻的运行时实例，继承自 `EcsEntity`。
- **AttackerId**: 发起攻击的实体 ID。
- **TargetId**: 攻击目标的实体 ID。
- **ConfigId**: 关联的攻击配置 ID。
- **StartTimeMs**: 攻击开始的时间戳（毫秒）。

## 组件 (Components)

### `AttackDamageComponent`
存储伤害计算相关的数据组件。
- **BaseDamage**: 基础伤害数值。
- **Formula**: 伤害计算使用的公式类型 (`DamageFormulaType`)。

### `AttackListComponent`
作为容器组件，挂载在角色实体上，用于管理该角色发起的所有 `AttackAction`。
- **Id2Entities**: 攻击 ID 到攻击实体的映射字典。
- **ConfigId2Entities**: 配置 ID 到攻击实体列表的映射，用于同类攻击管理。
- **AttackerId2LatestAttackId**: 记录发起者最近一次发起的攻击 ID。

### `AttackTimelineComponent`
管理攻击的时间轴与阶段状态。
- **WindupDurationMs**: 前摇（准备）阶段时长。
- **ActiveDurationMs**: 生效（判定）阶段时长。
- **RecoveryDurationMs**: 后摇（收招）阶段时长。
- **CurrentPhase**: 当前所处的攻击阶段 (`AttackPhase`)。
- **HitEmitted**: 标记是否已触发过命中事件，防止重复判定。
- **CancelReason**: 如果攻击被取消，存储取消的原因。

## 接口 (Interfaces)

### `IAttackConfig`
定义一次攻击的静态配置参数接口。
- **Id**: 配置唯一标识。
- **BaseDamage**: 基础伤害值。
- **WindupDurationMs**: 前摇时长。
- **ActiveDurationMs**: 生效时长。
- **RecoveryDurationMs**: 后摇时长。
- **DamageFormula**: 指定使用的伤害公式。

## 枚举 (Enums)

### `AttackPhase`
描述攻击生命周期的阶段：
- `None`: 未开始。
- `Windup`: 前摇/准备阶段。
- `Active`: 生效/判定阶段。
- `Recovery`: 后摇/收招阶段。
- `Ended`: 自然结束。
- `Canceled`: 被取消。

### `AttackCancelReason`
攻击被取消的原因：
- `None`: 未取消。
- `Interrupted`: 被打断（硬直、控制等）。
- `Manual`: 主动取消。
- `InvalidTarget`: 目标无效。

### `AttackFailureReason`
攻击启动失败的原因：
- `InvalidTarget`: 目标无效。
- `ConfigNotFound`: 配置缺失。
- `TimeInvalid`: 时间参数无效。
- `AlreadyAttacking`: 已在攻击中（互斥）。

### `DamageFormulaType`
伤害计算公式类型：
- `Flat`: 固定伤害。
- `AttackMinusDefense`: 攻防差值。
- `AttackTimesRatio`: 攻击倍率。

### `DamageFailReason`
伤害计算失败原因，如属性缺失、溢出或公式不支持。

## 结构体 (Structs)

### `AttackContext`
创建攻击时的上下文参数包，包含发起者、目标、配置ID和开始时间。

### `AttackComputeResult`
伤害计算的返回结果，包含成功状态、最终伤害值或失败原因。
