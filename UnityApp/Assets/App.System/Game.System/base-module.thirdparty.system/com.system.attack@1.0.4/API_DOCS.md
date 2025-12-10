# Attack Module - System Layer API

本模块实现了攻击流程的驱动、状态流转、伤害计算与事件分发。

## 系统 (Systems)

### `AttackActionSystem`
单次普攻流程的核心驱动系统。
- **`TryStartAttack`**: 尝试发起一次攻击。
    - 负责校验目标有效性、时间戳、配置是否存在。
    - 检查并发限制（如是否已有正在进行的攻击）。
    - 创建 `AttackAction` 实体并初始化相关组件。
    - 触发 `IOnAttackStart` 事件。
- **`Tick`**: 驱动单个攻击实体的流程。
    - 调用 `AttackTimelineSystem` 推进时间轴。
    - 在 `Active` 阶段且未触发过命中时，尝试计算伤害并触发 `IOnAttackHit`。
    - 在 `Ended` 阶段触发 `IOnAttackEnd`。

### `AttackListSystem`
管理实体上的攻击列表组件 (`AttackListComponent`)。
- **`Create`**: 创建新的 `AttackAction` 实体，并将其注册到列表组件的索引中。
- **`Remove`**: 从列表和索引中移除指定的 `AttackAction`。
- **`Tick`**: 轮询列表中的所有攻击实例，驱动其更新 (`AttackActionSystem.Tick`)，并统一清理已结束或取消的实例。

### `AttackTimelineSystem`
处理时间轴逻辑与阶段流转。
- **`InitFromConfig`**: 根据配置初始化时间轴组件的数据。
- **`TickAndTransit`**: 根据当前时间与阶段开始时间的差值，自动推进攻击阶段 (`Windup` -> `Active` -> `Recovery` -> `Ended`)。
- **`EnterNextPhase`**: 强制进入下一个阶段。

### `AttackDamageSystem`
纯函数式的伤害计算系统。
- **`TryCalculateDamage`**: 根据 `AttackDamageComponent` 的数据与公式类型计算最终伤害。
    - 支持 `Flat` (固定值)、`AttackMinusDefense` (攻防差)、`AttackTimesRatio` (倍率) 等公式。
    - 返回计算结果及可能的失败原因 (`DamageFailReason`)。

## 事件接口 (Events)

系统通过 ECS 的 Dispatch 机制分发以下事件，外部系统可订阅这些接口以响应攻击行为：

- **`IOnAttackStart`**: 攻击成功启动时触发。
- **`IOnAttackHit`**: 攻击在生效阶段判定命中（并成功计算伤害）时触发。
- **`IOnAttackEnd`**: 攻击流程自然结束时触发。
- **`IOnAttackCancel`**: 攻击被取消时触发。

## 配置提供者 (Providers)

### `IAttackConfigProvider`
外部系统需实现此接口并通过 ECS Dispatch 机制提供服务。
- **`GetConfig`**: 根据实体和配置 ID 返回 `IAttackConfig` 实例。攻击模块依赖此接口获取静态配置数据。
