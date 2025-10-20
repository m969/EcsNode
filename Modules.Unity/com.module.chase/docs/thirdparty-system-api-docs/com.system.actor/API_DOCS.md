# 角色子系统 API

本文档总结了位于 `Assets/App.System/Game.System/system.actor/com.system.actor` 路径下的角色相关 ECS 系统对外接口。

## 模块概览

- **主要系统**：`ActorSystem`、`ActorListSystem`
- **核心职责**：角色生命周期管理、追逐目标解析、全局角色检索
- **关联核心组件**：`TransformComponent`、`CollisionComponent`、`MoveComponent`、`HealthComponent`、`FireComponent`、`TaskListComponent`、`AIComponent` 以及追逐相关组件

## ActorSystem

`ActorSystem` 负责调度 `Actor` 实体的运行时行为，接口实现涵盖 `IAwake`、`IInit`、`IUpdate`

### 静态工厂方法

`Actor Create(EcsEntity gameWorld, long actorId)`

- 在 `gameWorld` 下以 `actorId` 生成一个子级 `Actor`。
- 注册移动、战斗、生命、AI、任务及追逐所需的基础组件。
- 默认将 `Type` 字段设为 `ActorType.Hero`（`1`）。

### 生命周期回调

- `void Awake(Actor entity)`
	- 组件装配完成后立即触发，目前未实现具体逻辑，预留后续扩展。
- `void Init(Actor entity)`
	- 设置基础移动速度（`10`）、停止速度（`5`）与开火频率（`5`）。
	- 若存在 `AIComponent` 则启用，使 `AISystem.Update` 得以执行。
- `void Update(Actor entity)`
	- 调用 AI 更新、按朝向执行移动，并以 `AppStatic.DeltaTimeSeconds` 驱动追逐子系统。
- `void OnHealthChange(Actor entity, HealthComponent component)`
	- 生命值变更时触发的保留回调，可在此处理受击逻辑。

## ActorListSystem

`ActorListSystem` 负责维护世界中的 `ActorListComponent`，用于追踪所有角色。

- `static void AddActor(EcsEntity world, Actor actor)`
	- 将角色加入 `ActorList` 与 `ActorDict`。
- `static void RemoveActor(EcsEntity world, Actor actor)`
	- 从上述集合中移除角色。
- `static Actor GetActor(EcsEntity world, long actorId)`
	- 按标识查询角色，若不存在则返回 `null`。

### 组件约定

确保 `ActorListComponent` 至少包含：

- `List<Actor> ActorList`
- `Dictionary<long, Actor> ActorDict`

系统假设上述集合在使用前已完成初始化。
