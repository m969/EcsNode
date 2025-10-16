# 永远用中文回答
# 永远不要做兼容性的改动和保留

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

## 通用开发规范

### 命名空间
所有模块代码命名空间为 `ECSGame.**Module`（**为模块名，忽略横杆下划线，大写开头）

### 常用引用
```csharp
using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
```

### 设计原则
- 实体、组件和系统都无需实现构造函数
- 实体包含简单基础属性
- 组件承载可插拔的复杂功能
- 同类型组件在实体中唯一
- 实体创建只能通过AddChild<T>()方法添加到父实体（parent）下
- 模块系统设计原则应遵循以下原则：
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
- `/docs/game.thirdparty.module`: 依赖的第三方模块代码
- `/com.model.**/API_DOCS.md`: 数据模型层API文档
- `/com.system.**/API_DOCS.md`: 系统逻辑层API文档