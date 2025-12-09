# 角色状态模块 (ActorStateModule) - System层 API 文档

本文档详细说明了 `com.system.actor-state` 目录下的系统逻辑、请求结构和事件接口。

## 命名空间
`ECSGame.ActorStateModule`

## 系统 (Systems)

### ActorStateSystem
**继承自:** `AComponentSystem<EcsEntity, ActorStateComponent>`

角色状态管理的核心系统，负责处理状态切换请求、查询状态以及驱动状态的更新逻辑。

#### 静态方法 (Static Methods)

| 方法签名 | 描述 |
| :--- | :--- |
| `void RequestChangeState(EcsEntity entity, ChangeStateRequest request)` | 请求改变角色的状态（开启或关闭）。处理状态的进入和退出逻辑。 |
| `bool IsStateActive(EcsEntity entity, ActorStateType stateType)` | 查询指定状态当前是否处于激活状态。 |
| `void SetDefaultState(EcsEntity entity, ActorStateType stateType)` | 设置角色的默认状态，并强制进入该状态。 |
| `void OnUpdate(EcsEntity entity)` | 每帧更新状态逻辑。会调用各激活状态对应的 System 的 OnUpdate 方法，并派发 `IStateUpdateHandler` 事件。 |

---

### ActorAliveStateSystem
**继承自:** `AComponentSystem<EcsEntity, ActorAliveState>`

处理角色存活状态的具体逻辑。

#### 静态方法 (Static Methods)
| 方法签名 | 描述 |
| :--- | :--- |
| `void OnEnter(EcsComponent component)` | 进入存活状态时的逻辑。 |
| `void OnExit(EcsComponent component)` | 退出存活状态时的逻辑。 |
| `void OnUpdate(EcsEntity entity)` | 存活状态下的持续逻辑更新。 |

---

### ActorDeathStateSystem
**继承自:** `AComponentSystem<EcsEntity, ActorDeathState>`

处理角色死亡状态的具体逻辑。

#### 静态方法 (Static Methods)
| 方法签名 | 描述 |
| :--- | :--- |
| `void OnEnter(EcsComponent component)` | 进入死亡状态时的逻辑。 |
| `void OnExit(EcsComponent component)` | 退出死亡状态时的逻辑。 |

---

### ActorStunnedStateSystem
**继承自:** `AComponentSystem<EcsEntity, ActorStunnedState>`

处理角色眩晕状态的具体逻辑。

#### 静态方法 (Static Methods)
| 方法签名 | 描述 |
| :--- | :--- |
| `void OnEnter(EcsComponent component)` | 进入眩晕状态时的逻辑。 |
| `void OnExit(EcsComponent component)` | 退出眩晕状态时的逻辑。 |
| `void OnUpdate(EcsEntity entity)` | 眩晕状态下的持续逻辑更新（如倒计时处理）。 |

## 事件接口 (Events)

通过 `entity.Dispatch<T>` 分发的事件接口。

### IStateEnterHandler
**继承自:** `IDispatch`

当角色进入某个状态时触发。

| 方法签名 | 描述 |
| :--- | :--- |
| `void OnStateEnterHandle(EcsEntity entity, ActorStateType stateType)` | 状态进入回调。 |

### IStateExitHandler
**继承自:** `IDispatch`

当角色退出某个状态时触发。

| 方法签名 | 描述 |
| :--- | :--- |
| `void OnStateExitHandle(EcsEntity entity, ActorStateType stateType)` | 状态退出回调。 |

### IStateUpdateHandler
**继承自:** `IDispatch`

当角色状态更新时触发。

| 方法签名 | 描述 |
| :--- | :--- |
| `void OnStateUpdateHandle(EcsEntity entity)` | 状态更新回调。 |

### IEnterHandler
**继承自:** `IDispatch`

组件进入时触发。

| 方法签名 | 描述 |
| :--- | :--- |
| `void OnEnter(EcsComponent component)` | 组件进入回调。 |

### IExitHandler
**继承自:** `IDispatch`

组件退出时触发。

| 方法签名 | 描述 |
| :--- | :--- |
| `void OnExit(EcsComponent component)` | 组件退出回调。 |
