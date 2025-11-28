# （角色状态）模块需求文档

## 功能需求列表

- 支持为角色实体挂载状态组件，用于记录当前状态信息。
- 支持通过命令请求改变角色的当前状态。
- 支持多状态维护
- 支持状态切换时的生命周期管理，包括触发状态退出逻辑和状态进入逻辑。
- 支持状态的持续更新逻辑，在每一帧执行当前状态的行为。
- 支持查询角色当前的活动状态。
- 支持定义默认状态，当角色初始化时自动进入该状态。

## 核心关键词

- ActorStateComponent (角色状态组件)
- ChangeStateRequest (切换状态请求)
- CurrentStates (当前状态字典)
- StateEnterEvent (状态进入事件)
- StateExitEvent (状态退出事件)
- ActorStateSystem (角色状态系统)
- ActorStateType（角色状态类型枚举，存活状态Alive，死亡状态Death，眩晕状态Stunned）

- ActorAliveStateComponent (角色存活状态组件)
- ActorDeathStateComponent (角色死亡状态组件)
- ActorStunnedStateComponent (角色眩晕状态组件)