# （Player）模块功能设计文档

## 模块功能概述

- 目标：为基于 ECS 的 Unity 项目提供“玩家（Player）”实体的最小通用能力与统一接口。

## 配置接口设计（无）

- 当前需求未提出配置项，留空。

## 实体设计

- 实体：Player
	- 用途：表示游戏中的玩家对象（当前版本假定单一本地玩家），承载生命周期状态并可被系统管理。
	- 字段（数据组件化拆分，核心最小集）：
		- Id（EntityId）：实体唯一标识（只读）。

- 实体系统设计：PlayerSystem

- 实体列表组件：PlayerListComponent
	- 用途：集中存储与管理 PlayerEntity 的集合，支持查找、遍历与计数。
	- 字段设计：
		- players（Map<EntityId, Player>）
		- count（int，派生或缓存）

- 实体列表组件系统设计（PlayerListSystem）
	- 接口设计：
		- Add(EntityId id, in Player e) -> bool
		- Remove(EntityId id) -> bool
		- Get(EntityId id) -> Player | null
		- Contains(EntityId id) -> bool
		- Count() -> int
		- ForEach(Action<Player> action)

## 组件设计
