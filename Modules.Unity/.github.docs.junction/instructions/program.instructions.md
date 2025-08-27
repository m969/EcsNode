---
applyTo: "program-documentation.md"
---

模块程序设计文档（基于功能设计文档design-documentation.md）：
- 一个类一个文件

1. 程序功能概述
2. 数据结构设计
    实体和组件：
    - 只实现属性数据，不实现方法逻辑
    - 配置接口设计（配置接口以整型Id为唯一标识，字符串Key作为辅助名称标识）
    - 实体设计，继承自EcsEntity，属性应有必要summary（实体继承自EcsEntity，EcsEntity包含Id、Parent属性，不需要额外再定义）
    - 组件设计，继承自EcsComponent，属性应有必要summary
    - 补充类型设计，属性应有必要summary，枚举值应有必要summary
3. 系统业务设计
    - 系统设计（系统为非静态类，但系统业务方法为静态方法，应标明方法名称和方法参数，方法应有必要summary）
        - 实体系统设计（应实现必要的生命周期接口：`IAwake<T>`, `IInit<T>`, `IAfterInit<T>`, `IEnable<T>`, `IDisable<T>`, `IDestroy<T>`）
        - 组件系统设计（应实现必要的生命周期接口：`IAwake<T, C>`, `IInit<T, C>`, `IAfterInit<T, C>`, `IEnable<T, C>`, `IDisable<T, C>`, `IDestroy<T, C>`）
    - 系统事件接口设计，应标明方法名称和方法参数，方法应有必要summary

## 系统类：
- 只实现方法逻辑，不实现属性数据，
- 实体系统继承 `AEntitySystem<T>`
    - 实体系统应实现必要的生命周期接口：`IAwake<T>`, `IInit<T>`, `IAfterInit<T>`, `IEnable<T>`, `IDisable<T>`, `IDestroy<T>`
    - 实体系统应有一个或多个Create静态业务接口用于创建实体，并将父实体和配置接口作为参数传入

- 组件系统继承 `AComponentSystem<T, C>`
    - 组件系统应实现必要的生命周期接口：`IAwake<T, C>`, `IInit<T, C>`, `IAfterInit<T, C>`, `IEnable<T, C>`, `IDisable<T, C>`, `IDestroy<T, C>`

### 系统派发事件接口
- 派发事件接口继承 `IDispatch`
示例：
```csharp
public interface IOnHandleScore : IDispatch
{
    void OnHandleScore(EcsEntity entity, int param);
}
```
- 为遵循开闭原则和依赖倒置原则，可扩展的系统功能应提供事件接口派发到外部由开发者自定义扩展逻辑

### 系统业务逻辑方法
- 系统业务逻辑方法均为静态方法
- 只传实体和必要参数，组件在方法内获取
- 需要必要的summary摘要


## 设计规范如下：

基于EcsNode框架，EcsNode是基于ECS（Entity-Component-System）架构的Unity游戏开发框架，通过实体、组件和系统的组合方式实现高效、灵活的游戏逻辑。

## EcsNode核心库已有实体和组件（在ECS命名空间下）有：
- 实体基类 EcsEntity，包含以下属性和接口：
    - Id（long）属性。
    - Parent（EcsEntity）属性。
    - Id2Children（存放子实体的字典）。
    - type2Component（存放实体的组件）。
    - GetComponent<T>()方法，用于获取实体的组件。
    - AddComponent<T>(beforeAwake)方法，用于添加组件，beforeAwake委托用于在Awake生命周期前填充参数。
    - RemoveComponent<T>()方法，用于移除组件。
    - AddChild<T>(beforeAwake)方法，用于添加子实体，beforeAwake委托用于在Awake生命周期前填充参数。
    - RemoveChild<T>()方法，用于移除子实体。
    - Dispatch<T>((T system) => system.Handle(entity, a))方法，用于分发系统事件。
        - 例如：
            ```csharp
            entity.Dispatch<IOnStartBuild>((system) => system.OnStartBuild(entity, count));
            ```

- 组件基类 EcsComponent：
    - 包含Entity（EcsEntity）属性，用于关联所属实体。

### 命名空间
所有模块代码命名空间为 `ECSGame.Module.**`（**为模块名，忽略横杆下划线，大写开头）

### 常用引用
```csharp
using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
```

### 设计原则
1. 实体包含简单基础属性
2. 组件承载可插拔的复杂功能
3. 同类型组件在实体中唯一
4. 实体创建只能通过AddChild<T>()方法添加到父实体（parent）下
5. 模块系统设计原则应遵循以下原则：
    - 单一职责原则
        单一职责原则指出，一个类应该只有一个改变的理由。这意味着一个类应该只负责一项任务或功能。如果一个类承担了过多的职责，它将变得复杂，难以维护和扩展。遵循这个原则有助于保持类的聚焦和高内聚，使得代码更加清晰和易于管理
    - 开闭原则
        开闭原则是面向对象设计的核心所在，它强调软件实体（如类、模块、函数等）应该对扩展开放，对修改关闭。这意味着设计时应该允许系统在不修改现有代码的情况下引入新功能。这可以通过使用接口和抽象类来实现，使得系统更容易扩展和维护
    - 依赖倒置原则
        依赖倒置原则要求高层模块不应依赖于低层模块，两者都应依赖于抽象。抽象不应依赖于细节，细节应依赖于抽象。这个原则鼓励我们面向接口编程，而不是面向实现编程，从而减少代码间的耦合，提高系统的灵活性和可维护性
