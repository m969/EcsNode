# （Player）模块程序设计文档

> 命名空间：`ECSGame.Module.Player`

## 1. 程序功能概述

为基于 EcsNode 的 Unity 项目提供“玩家（Player）”实体的最小通用能力与统一接口：
- 标准的玩家实体生命周期管理；
- 玩家实体集中管理（列表组件 + 系统静态方法）；
- 对外可派发的玩家创建/移除等事件接口，便于扩展。

## 2. 数据结构设计

通用引用：
```csharp
using ECS;
using System;
using System.Collections.Generic;
```

### 2.1 配置接口设计（无）
- 当前版本无配置项；如未来引入配置，应以整型 Id 为唯一标识、字符串 Key 为辅助标识，命名以 `Config` 结尾。

### 2.2 实体设计

#### Player : EcsEntity
- 用途：表示游戏中的玩家对象（当前版本假定单一本地玩家），承载生命周期状态并可被系统管理。
- 说明：仅承载必要属性数据；不实现任何方法逻辑。
- 基类继承已提供：`Id: long`、`Parent: EcsEntity`、`Id2Children`、`type2Component`。

### 2.3 组件设计

#### PlayerListComponent : EcsComponent
- 用途：集中存储与管理 `Player` 的集合，支持查找、遍历与计数。
- 属性：
	- `Players: Dictionary<long, Player>`
		- 摘要：以实体 Id 为键存储玩家实体的字典。
	- `Count: int`
		- 摘要：玩家数量；推荐视为从 `Players.Count` 派生（可缓存）。

### 2.4 补充类型设计
- `EntityId`：等价于 `long`，用于标识实体 Id 的语义（文档层说明）。

## 3. 系统业务设计

系统类只实现方法逻辑，不实现属性数据。

### 3.1 实体系统设计

#### PlayerSystem : AEntitySystem<Player>
- 生命周期接口实现：`IAwake<Player>`, `IInit<Player>`, `IAfterInit<Player>`, `IEnable<Player>`, `IDisable<Player>`, `IDestroy<Player>`。
- 静态业务方法（仅传实体及必要参数，组件在方法内获取）：
	- `static Player Create(EcsEntity parent)`
		- 摘要：创建并挂载一个 `PlayerEntity` 到指定 `parent` 下；内部通过 `parent.AddChild<PlayerEntity>(...)` 完成，并在 Awake 前填充必要数据。
		- 参数：`parent` 父实体。
		- 返回：创建的玩家实体。
	- `static void Remove(Player player)`
		- 摘要：销毁指定玩家实体；内部调用 `player.Parent.RemoveChild<PlayerEntity>()` 或由框架统一销毁流程处理。

- 生命周期方法（接口约定，签名示意）：
	- `void Awake(Player e)`：在实体创建时调用，用于最早期初始化。
	- `void Init(Player e)`：在 `Awake` 之后进行常规初始化。
	- `void AfterInit(Player e)`：在 `Init` 之后进行依赖已就绪的初始化。
	- `void Enable(Player e)`：启用实体相关逻辑。
	- `void Disable(Player e)`：禁用实体相关逻辑。
	- `void Destroy(Player e)`：销毁前清理。

### 3.2 组件系统设计

#### PlayerListSystem : AComponentSystem<PlayerListComponent, Player>
- 生命周期接口实现：`IAwake<PlayerListComponent, Player>`, `IInit<PlayerListComponent, Player>`, `IAfterInit<PlayerListComponent, Player>`, `IEnable<PlayerListComponent, Player>`, `IDisable<PlayerListComponent, Player>`, `IDestroy<PlayerListComponent, Player>`。

- 静态业务方法（只传实体和必要参数，组件在方法内通过 `entity.GetComponent<PlayerListComponent>()` 获取）：
	- `static bool Add(EcsEntity entity, long id, Player player)`
		- 摘要：将玩家实体添加到集合。
		- 返回：添加是否成功（若已存在则返回 false）。
	- `static bool Remove(EcsEntity entity, long id)`
		- 摘要：从集合中移除指定 Id 的玩家实体；若存在触发移除事件。
	- `static Player Get(EcsEntity entity, long id)`
		- 摘要：按 Id 获取玩家实体；不存在返回 `null`。
	- `static bool Contains(EcsEntity entity, long id)`
		- 摘要：集合中是否包含指定 Id。
	- `static int Count(EcsEntity entity)`
		- 摘要：返回玩家数量（优先从组件派生字段获取）。
	- `static void ForEach(EcsEntity entity, Action<Player> action)`
		- 摘要：遍历集合中所有玩家实体并执行回调。

- 生命周期方法（接口约定，签名示意）：
	- `void Awake(PlayerListComponent c, Player owner)`：组件挂载时调用。
	- `void Init(PlayerListComponent c, Player owner)`：初始化集合结构。
	- `void AfterInit(PlayerListComponent c, Player owner)`：依赖准备就绪后的初始化。
	- `void Enable(PlayerListComponent c, Player owner)`：启用集合管理逻辑。
	- `void Disable(PlayerListComponent c, Player owner)`：禁用集合管理逻辑。
	- `void Destroy(PlayerListComponent c, Player owner)`：组件销毁前清理。

### 3.3 系统事件接口设计（派发扩展）

为遵循开闭原则与依赖倒置原则，系统通过事件接口向外派发扩展点，接口均继承 `IDispatch`：

```csharp
namespace ECSGame.Module.Player
{
		using ECS;

		/// <summary>
		/// 玩家被创建时派发；常用于统计与外部订阅扩展。
		/// </summary>
		public interface IOnPlayerCreated : IDispatch
		{
				void OnPlayerCreated(EcsEntity context, Player player);
		}

		/// <summary>
		/// 玩家被移除时派发；常用于清理、记录与 UI 更新。
		/// </summary>
		public interface IOnPlayerRemoved : IDispatch
		{
				void OnPlayerRemoved(EcsEntity context, long playerId);
		}
}
```

派发示例：
```csharp
// 创建后
entity.Dispatch<IOnPlayerCreated>(s => s.OnPlayerCreated(entity, player));
// 移除后
entity.Dispatch<IOnPlayerRemoved>(s => s.OnPlayerRemoved(entity, id));
```

## 4. 设计说明与约束

- 实体只包含属性数据，不包含方法逻辑；实体创建只能通过 `AddChild<T>()`；
- 同类型组件在实体中唯一；
- 系统业务方法统一为静态方法，入参只传实体与必要参数，组件在方法内获取；
- 命名空间固定为 `ECSGame.Module.Player`，避免跨模块耦合；
- 事件接口用于对外扩展，内部系统不依赖具体实现。

## 5. 后续扩展建议（非必须）

- 如需多玩家支持，可在 `PlayerListComponent` 上增加查询索引（如按名称/会话 Id）；
- 可增加 `IOnPlayerEnabled` / `IOnPlayerDisabled` 等生命周期事件接口；
- 可引入 `IPlayerConfig` 以支持多种玩家初始参数与外观预设。
