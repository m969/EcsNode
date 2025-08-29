# （消息）模块程序设计文档

模块程序设计文档（基于功能设计文档design-documentation.md）：
- 一个类一个文件

命名空间：`ECSGame.Module.Message`

常用引用：
```csharp
using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
```

## 1. 程序功能概述
- 提供消息Id 与 消息类型（System.Type）之间的高效双向映射能力。
- 对外暴露静态业务方法：
	- GetMessageType：根据消息Id获取消息类型。
	- GetMessageTypeId：根据消息类型获取消息Id。
- 支持通过系统事件在初始化阶段扩展注册外部消息映射，遵循开闭原则与依赖倒置原则。

## 2. 数据结构设计

### 2.1 配置接口设计
为便于从外部配置/表格注册消息映射，定义配置接口（以整型Id为唯一标识，字符串Key为辅助名称标识）：

- 接口：`IMessageTypeConfig`
	- `int Id`：消息唯一Id。
	- `string Key`：辅助名字标识（可读性用途）。
	- `string TypeName`：消息类型的AssemblyQualifiedName或完整类型名（由系统解析为`System.Type`）。

### 2.2 实体设计
- 承载实体：直接使用基础实体 `EcsEntity`（不再定义专用派生实体）。
	- Summary：任何`EcsEntity`都可作为消息映射组件的承载实体。
	- 说明：利用`EcsEntity`的通用能力（`Id`、`Parent`、`Id2Children`、`type2Component` 等），无需额外实体类型。

### 2.3 组件设计
- 类名：`NeterMessageComponent`，继承自`EcsComponent`。
	- Summary：Neter消息组件，存储消息Id与消息类型的双向映射。
	- 属性：
		- `Dictionary<int, Type> Id2Type`：消息Id到消息类型的映射。
		- `Dictionary<Type, int> Type2Id`：消息类型到消息Id的映射。

### 2.4 补充类型设计
- 暂无额外枚举或值对象需求；后续如需扩展错误码或类型解析策略，可在不修改现有系统的前提下新增补充类型。

## 3. 系统业务设计

系统类：只实现方法逻辑，不实现属性数据。

### 3.1 组件附加与初始化（无专用实体系统）
- 不再提供专用实体系统；直接将`NeterMessageComponent`挂载到任意`EcsEntity`。
- 静态辅助方法：
	- 方法：`static NeterMessageComponent Attach(EcsEntity entity, IEnumerable<IMessageTypeConfig> configs)`
		- Summary：将`NeterMessageComponent`挂载到给定`entity`并根据`configs`初始化Id/Type映射；随后派发扩展事件以允许外部追加映射。
		- 参数：
			- `entity`：目标实体（作为组件承载）。
			- `configs`：消息类型配置集合，可为空（为空时仅通过事件扩展注册）。

### 3.2 组件系统设计（AComponentSystem<T, C>）
- 类名：`NeterMessageSystem`，继承自`AComponentSystem<EcsEntity, NeterMessageComponent>`。
- 生命周期接口：实现`IAwake<EcsEntity, NeterMessageComponent>`、`IInit<EcsEntity, NeterMessageComponent>`、`IAfterInit<EcsEntity, NeterMessageComponent>`、`IEnable<EcsEntity, NeterMessageComponent>`、`IDisable<EcsEntity, NeterMessageComponent>`、`IDestroy<EcsEntity, NeterMessageComponent>`。
- 系统业务逻辑方法（均为静态方法，仅传入实体与必要参数，组件在方法内获取）：
	- 方法：`static Type GetMessageType(EcsEntity entity, int messageId)`
		- Summary：获取给定消息Id对应的消息类型。
		- 返回：`Type`（未找到返回`null`）。
	- 方法：`static int GetMessageTypeId(EcsEntity entity, Type messageType)`
		- Summary：获取给定消息类型对应的消息Id。
		- 返回：`int`（未找到返回`-1`）。

### 3.3 系统事件接口设计（可扩展）
- 为遵循开闭原则和依赖倒置原则，提供可派发的扩展事件接口，外部可在初始化阶段注册额外的消息类型映射。

- 接口：`IOnRegisterNeterMessageTypes : IDispatch`
	- 方法：`void OnRegisterNeterMessageTypes(EcsEntity entity, IDictionary<int, Type> id2Type);`
	- Summary：在`NeterMessageComponent`初始化后派发，允许外部向`id2Type`中追加/覆盖映射；系统内部会同步维护`Type2Id`。

## 4. 设计与实现说明
- 一个类一个文件，放置于命名空间`ECSGame.Module.Message`下。
- 组件在实体上唯一；可直接将组件挂载到任意`EcsEntity`，或使用`NeterMessageSystem.Attach`完成初始化与扩展派发。
- 组件系统所有业务方法使用静态方法，对外只暴露必要的查询能力；注册扩展通过事件完成，避免直接修改系统。
- 类型解析：`IMessageTypeConfig.TypeName`建议使用`AssemblyQualifiedName`，以避免跨程序集解析歧义。

## 5. 示例交互（伪代码）
```csharp
// 1) 选择承载实体（可复用现有实体，或创建一个基础子实体）
var entity = parent.AddChild<EcsEntity>();

// 2) 挂载并初始化消息映射组件
var comp = NeterMessageSystem.Attach(entity, configs);

// 3) 查询：Id -> Type
var type = NeterMessageSystem.GetMessageType(entity, messageId);

// 4) 查询：Type -> Id
var id = NeterMessageSystem.GetMessageTypeId(entity, typeof(SomeMessage));

// 5) 扩展注册（外部系统，可以在AfterInit时机）
entity.Dispatch<IOnRegisterNeterMessageTypes>(s => s.OnRegisterNeterMessageTypes(entity, comp.Id2Type));
```

## 6. 边界与约定
- 未找到时返回`null`或`-1`，调用方应做好判空/校验。
- 若存在重复Id或重复Type的注册，后注册可覆盖先注册的映射，系统将以「最后写入」为准并保持双向字典一致。
- 线程安全由调用方保障；如需并发访问可在后续迭代引入读写锁或无锁快照结构。

## 7. 与功能设计文档的一致性
- 组件：`NeterMessageComponent` 用于“存储消息Id和消息类型的映射”。
- 组件系统：`NeterMessageSystem` 提供 `GetMessageType` 与 `GetMessageTypeId` 两个业务方法。
