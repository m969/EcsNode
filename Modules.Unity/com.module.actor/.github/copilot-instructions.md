# 永远用中文回答

# 工程概览

这个工程包是EcsNode的模块工程，用于实现可插入EcsNode框架的模块代码，使用C#语言编写，遵循EcsNode的开发规范和最佳实践。

EcsNode是基于ECS（Entity-Component-System）架构的Unity游戏开发框架，通过实体、组件和系统的组合方式实现高效、灵活的游戏逻辑。

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
    - Dispatch<T>((T system) => system.Handle(entity, a))方法，用于分发事件。
        - 例如：
            ```csharp
            entity.Dispatch<IOnStartBuild>((system) => system.OnStartBuild(entity, count));
            ```

- 组件基类 EcsComponent：
    - 包含Entity（EcsEntity）属性，用于关联所属实体。

### 核心实体（ECSGame命名空间）
- Player：玩家实体
- Actor：角色实体
- Item：物品实体

## 通用开发规范

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

## 文件夹结构

- `/com.model.**`: 实体和组件脚本
- `/com.system.**`: 系统脚本
- `/com.view-model.**`: 视图实体和组件脚本
- `/com.view-system.**`: 视图系统脚本
- `/system.module-test`: 单元测试代码
- `/docs/*-documentation.md`: 各类设计文档

---
applyTo: "com.model.**/**/*.cs"
---

## 实体和组件：
- 只实现属性数据，不实现方法逻辑
- 实体继承自EcsEntity，组件继承自EcsComponent
- 一个类一个文件

---
applyTo: "com.system.**/**/*.cs"
---

## 系统类：
- 只实现方法逻辑，不实现属性数据
- 系统为实例类，但系统业务方法仍为静态方法，方便调用
- 实体系统继承 `AEntitySystem<T>`
    - 实体系统应实现必要的生命周期接口：`IAwake<T>`, `IInit<T>`, `IAfterInit<T>`, `IEnable<T>`, `IDisable<T>`, `IUpdate<T>`, `IDestroy<T>`
    - 实体系统应有一个或多个Create静态业务方法用于创建实体

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
- 需要详细summary注释

示例：
```csharp
/// <summary>
/// 处理业务逻辑
/// </summary>
/// <param name="entity">实体</param>
/// <param name="param">参数</param>
public static void HandleLogic(EcsEntity entity, int param)
{
    var component = entity.GetComponent<MyComponent>();
    // 处理逻辑
}
```

---
applyTo: "system.module-test.**/**/*.cs"
---

模块系统单元测试代码

只需要实现系统类的测试流程，不需要实现实体和组件等的测试流程

基于以下测试框架实现测试流程：
- "NUnit" Version="4.3.2"
- "Microsoft.NET.Test.Sdk" Version="17.14.1"
- "NUnit3TestAdapter" Version="5.0.0"

使用Visual Studio或Rider等IDE运行测试
- 测试类命名为 `**ModuleTests`
- 测试类放在 `system.module-test` 文件夹下
- 测试断言使用 `Assert` 类的Assert.That接口，例如：
```csharp
Assert.That(condition, Is.Not.Null);
Assert.That(config.Id, Is.EqualTo(item.Id));
```

- 测试类应有以下结构：
    - ecsNode属性，用于测试实体和组件
    - TestEcsNode 类继承自 `EcsNode`模拟Ecs域，用于测试实体和组件
    - `SetUp` 方法用于初始化测试环境
    - 测试方法使用 `[Test]` 特性标记
    - 模拟配置类定义（如果需要）


- 测试类示例：
```csharp
using ECS;
using NUnit.Framework;

namespace ECSGame.Module.Achieve.Tests
{
    [TestFixture]
    public class MyModuleTests
    {
        private EcsNode ecsNode;

        public class TestEcsNode : EcsNode
        {
            public TestEcsNode(ushort id) : base(id) { }
        }

        [SetUp]
        public void SetUp()
        {
            ecsNode = new TestEcsNode(1);
        }

        [Test]
        public void TestMethodA()
        {

        }
    }
}