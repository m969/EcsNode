# Resource Data Model API Documentation

## 命名空间
`ECSGame.ResourceDataModule`

## 枚举 (Enums)

### ResourceType
资源类型枚举。

| 枚举值 | 描述 |
| :--- | :--- |
| Coin | 金币 |
| Diamond | 钻石 |
| Stamina | 体力 |
| Item | 物品 |

### ResourceChangeType
资源变更类型枚举。

| 枚举值 | 描述 |
| :--- | :--- |
| Gain | 获得资源 |
| Consume | 消耗资源 |
| Reward | 奖励资源 |
| Deduct | 扣除资源 |

## 实体 (Entities)

### ResourceChangeLog
资源变更记录实体，继承自 `EcsEntity`。

| 属性 | 类型 | 描述 |
| :--- | :--- | :--- |
| ChangeTime | DateTime | 资源变更时间 |
| ChangeType | ResourceChangeType | 资源变更类型 |
| ChangeValue | int | 资源变更数值 |
| Reason | string | 资源变更原因 |
| ResourceType | ResourceType | 资源类型 |
| OwnerId | long | 资源所属者Id |

## 组件 (Components)

### ResourceDataComponent
资源数据组件，继承自 `EcsComponent`。

| 属性 | 类型 | 描述 |
| :--- | :--- | :--- |
| ResourceValues | Dictionary<ResourceType, int> | 各资源类型对应的资源数值 |
| LastSyncTime | DateTime | 上次同步资源数据的时间 |

## 事件接口 (Event Interfaces)

### IResourceChanged
资源变更事件接口（增/减均触发；批量逐条触发）。继承自 `IDispatch`。

#### 方法
`void OnResourceChanged(EcsEntity entity, ResourceType type, int delta, int newValue, ResourceChangeType changeType, string reason)`

| 参数 | 类型 | 描述 |
| :--- | :--- | :--- |
| entity | EcsEntity | 发生变更的实体 |
| type | ResourceType | 资源类型 |
| delta | int | 变更量（正数增加，负数减少） |
| newValue | int | 变更后的新值 |
| changeType | ResourceChangeType | 变更类型 |
| reason | string | 变更原因 |

### IResourceSynced
资源同步完成事件接口（加载或持久化写回完成）。继承自 `IDispatch`。

#### 方法
`void OnResourceSynced(EcsEntity entity, DateTime syncTime)`

| 参数 | 类型 | 描述 |
| :--- | :--- | :--- |
| entity | EcsEntity | 同步的实体 |
| syncTime | DateTime | 同步时间 |
