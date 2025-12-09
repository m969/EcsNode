# 角色状态模块 (ActorStateModule) - Model层 API 文档

本文档详细说明了 `com.model.actor-state` 目录下的数据模型组件和枚举定义。

## 命名空间
`ECSGame.ActorStateModule`

## 枚举 (Enums)

### ActorStateType
角色状态类型枚举，定义了角色可能处于的各种状态。

| 枚举值 | 描述 |
| :--- | :--- |
| `Alive` | 存活状态 |
| `Death` | 死亡状态 |
| `Stunned` | 眩晕状态 |

## 组件 (Components)

### ActorStateComponent
**继承自:** `EcsComponent`

角色状态核心组件，用于管理角色的当前状态集合。

#### 字段 (Fields)
| 类型 | 名称 | 描述 |
| :--- | :--- | :--- |
| `Dictionary<ActorStateType, EcsComponent>` | `CurrentStates` | 当前激活的状态字典，Key为状态类型，Value为对应的状态数据组件（若有）。 |
| `ActorStateType` | `DefaultState` | 默认状态，初始化时进入。 |

---

### ActorAliveState
**继承自:** `EcsComponent`

角色存活状态组件。标记角色处于存活状态，存储存活状态相关数据。
目前作为标记组件使用，无特定数据字段。

---

### ActorDeathState
**继承自:** `EcsComponent`

角色死亡状态组件。标记角色处于死亡状态，存储死亡状态相关数据。

#### 字段 (Fields)
| 类型 | 名称 | 描述 |
| :--- | :--- | :--- |
| `float` | `DeathTime` | 死亡时刻的时间戳。 |

---

### ActorStunnedState
**继承自:** `EcsComponent`

角色眩晕状态组件。标记角色处于眩晕状态，存储眩晕状态相关数据。

#### 字段 (Fields)
| 类型 | 名称 | 描述 |
| :--- | :--- | :--- |
| `float` | `StunDuration` | 眩晕持续时间。 |
| `float` | `StunStartTime` | 眩晕开始时间。 |

## 结构体 (Structs)

### ChangeStateRequest
切换状态请求结构，用于向系统传递状态变更的参数。

#### 字段 (Fields)
| 类型 | 名称 | 描述 |
| :--- | :--- | :--- |
| `ActorStateType` | `StateType` | 目标状态类型。 |
| `bool` | `Enable` | 开启或关闭该状态。 |
| `bool` | `Force` | 是否强制切换（忽略条件检查）。 |
