# （角色状态）模块程序设计文档

## 实体设计
（无）

## 组件设计

- 角色状态组件（ActorStateComponent）
    - 角色状态组件用途
        - 用于管理角色的状态，记录当前激活的状态以及处理状态切换。
    - 角色状态组件字段设计
        - CurrentStates：Dictionary<ActorStateType, EcsComponent>，当前激活的状态字典，Key为状态类型，Value为对应的状态数据组件（若有）。
        - DefaultState：ActorStateType，默认状态，初始化时进入。

- 角色存活状态组件（ActorAliveStateComponent）
    - 角色存活状态组件用途
        - 标记角色处于存活状态，存储存活状态相关数据。
    - 角色存活状态组件字段设计
        - （暂无特定字段，作为标记使用）

- 角色死亡状态组件（ActorDeathStateComponent）
    - 角色死亡状态组件用途
        - 标记角色处于死亡状态，存储死亡状态相关数据。
    - 角色死亡状态组件字段设计
        - DeathTime：float，死亡时刻的时间戳。

- 角色眩晕状态组件（ActorStunnedStateComponent）
    - 角色眩晕状态组件用途
        - 标记角色处于眩晕状态，存储眩晕状态相关数据。
    - 角色眩晕状态组件字段设计
        - StunDuration：float，眩晕持续时间。
        - StunStartTime：float，眩晕开始时间。

- 角色状态系统设计（ActorStateSystem）
    - 角色状态系统功能接口设计
        - void RequestChangeState(EcsEntity entity, ChangeStateRequest request)：请求改变状态。
        - bool IsStateActive(EcsEntity entity, ActorStateType stateType)：查询状态是否激活。
        - void SetDefaultState(EcsEntity entity, ActorStateType stateType)：设置默认状态。
        - void OnUpdate(EcsEntity entity)：每帧更新状态逻辑。

- 角色存活状态系统设计（ActorAliveStateSystem）
    - 角色存活状态系统功能接口设计
        - void OnEnter(EcsEntity entity)：进入存活状态逻辑。
        - void OnExit(EcsEntity entity)：退出存活状态逻辑。
        - void OnUpdate(EcsEntity entity)：存活状态持续逻辑。

- 角色死亡状态系统设计（ActorDeathStateSystem）
    - 角色死亡状态系统功能接口设计
        - void OnEnter(EcsEntity entity)：进入死亡状态逻辑。
        - void OnExit(EcsEntity entity)：退出死亡状态逻辑。

- 角色眩晕状态系统设计（ActorStunnedStateSystem）
    - 角色眩晕状态系统功能接口设计
        - void OnEnter(EcsEntity entity)：进入眩晕状态逻辑。
        - void OnExit(EcsEntity entity)：退出眩晕状态逻辑。
        - void OnUpdate(EcsEntity entity)：眩晕状态持续逻辑（如倒计时）。

## 其他类型补充

- 流程节点派发接口补充
    - IOnStateEnter：void OnStateEnter(EcsEntity entity, ActorStateType stateType)，状态进入时派发。
    - IOnStateExit：void OnStateExit(EcsEntity entity, ActorStateType stateType)，状态退出时派发。
    - IOnStateUpdate：void OnStateUpdate(EcsEntity entity)，状态更新时派发。

- 基础数据类型补充
    - ChangeStateRequest：切换状态请求结构
        - StateType：ActorStateType，目标状态类型。
        - Enable：bool，开启或关闭该状态。
        - Force：bool，是否强制切换（忽略条件检查）。

- 枚举补充
    - ActorStateType：角色状态类型
        - Alive：存活
        - Death：死亡
        - Stunned：眩晕
