# Project Overview

这个工程包是EcsNode的模块工程，用于实现可插入EcsNode框架的模块代码，使用C#语言编写，遵循EcsNode的开发规范和最佳实践。
EcsNode是一个基于ECS（Entity-Component-System）架构的游戏开发框架，旨在提供高效、灵活的游戏逻辑实现方式。
EcsNode的核心思想是将游戏对象拆分为实体（Entity）、组件（Component）和系统（System），通过组合的方式实现复杂的游戏逻辑。
EcsNode基于Unity引擎开发，提供了易于使用的API和工具，支持快速开发和迭代。
EcsNode使用C#语言编写，遵循Unity的开发规范和最佳实践，适用于各种类型的游戏项目。

EcsNode核心库已有实体和组件（在ECS命名空间下）有：
- 实体基类EcsEntity，包含以下属性和接口：
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

- 组件基类EcsComponent：
    - 包含Entity（EcsEntity）属性，用于关联所属实体。

EcsNode框架已有核心实体（在ECSGame命名空间下）有：
- Player：玩家实体，包含玩家的基本信息和状态。
- Actor：角色实体，包含角色的基本信息和状态。
- Item：物品实体，包含物品的基本信息和状态。

## Folder Structure

- `/com.model.**`: 存放模块的实体（EcsEntity）和组件（EcsComponent）脚本
- `/com.system.**`: 存放模块的系统（EcsSystem）脚本
- `/com.view-model.**`: 存放模块的视图实体（EcsEntity）和视图组件（EcsComponent）脚本
- `/com.view-system.**`: 存放模块的视图系统（EcsSystem）脚本
- `/system.module-test`: 存放模块系统的单元测试代码
- `/**-require-documentation.md`: 模块需求文档
- `/**-design-documentation.md`: 模块功能设计文档
- `/**-program-documentation.md`: 模块程序设计文档
- `/**-program-test-documentation.md`: 模块程序单元测试设计文档

---
applyTo: "**-require-documentation.md"
---

模块需求文档，包含模块的整体需求：
1. 模块需求概述
2. 模块相关实体功能需求（如果有）
3. 模块相关组件功能需求（如果有）

---
applyTo: "**-design-documentation.md"
---

模块功能设计文档，应以模块需求文档为基础，归纳总结出以下内容：
1. 模块功能概述
2. 模块相关实体功能设计（如果有）
- 实体属性设计
- 实体系统设计
    - 实体系统功能设计
3. 模块相关组件功能设计（如果有）
- 组件属性设计
- 组件系统设计（组件系统命名可省略Component后缀，缩短命名方便阅读）
    - 组件系统功能设计

---
applyTo: "**-program-documentation.md"
---

模块程序设计文档，应以模块功能设计文档为基础，归纳总结出以下内容：

1. 模块程序功能概述

2. 模块程序数据结构

- 实体（Entities）设计
    - 实体属性设计（并补充详细的summary摘要）

- 组件（Components）设计
    - 组件属性设计（并补充详细的summary摘要）

- 类型补充（枚举类型、结构体、数据类、配置类、常量等，并补充详细的summary摘要）

3. 模块程序逻辑流程

- 系统（Systems）设计（与实体和组件一一对应，AEntity对应AEntitySystem，AComponent对应AComponentSystem）
    - 系统接口设计（基于拆解功能，写明参数作用，除了生命周期接口方法是成员函数外，其余接口方法均为静态方法，并补充详细的summary摘要）

---
applyTo: "**-program-test-documentation.md"
---

模块程序单元测试设计文档，应以模块程序设计文档为基础，归纳总结出以下内容：
1. 模块程序单元测试概述
    - 测试目标
    - 测试范围
2. 模块程序单元测试用例设计
    - 测试用例列表
        - 用例编号
        - 用例名称
        - 测试目标
        - 测试步骤
        - 预期结果
3. 模块程序单元测试执行
    - 测试环境准备
    - 测试数据准备
    - 测试执行步骤
4. 模块程序单元测试结果
    - 测试结果记录
    - 测试结论

---
applyTo: "system.module-test.**/**/*.cs"
---



---
applyTo: "com.model.**/**/*.cs"
---

com.model.** 目录存放实体（EcsEntity）和组件（EcsComponent）

实体（EcsEntity）和组件（EcsComponent）只实现属性数据，不实现方法逻辑

实体继承自EcsEntity，一个实体类单独一个文件

组件继承自EcsComponent，一个组件类单独一个文件

命名空间为ECSGame.Module.**，其中**为模块名称（命名空间忽略横杆和下划线，并以大写字母开头）

常用引用命名空间：
```csharp
using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
```

---
applyTo: "com.system.**/**/*.cs"
---

com.system.** 目录存放系统（EcsSystem），与com.model.** 目录下的实体（EcsEntity）和组件（EcsComponent）一一对应

系统（EcsSystem）只实现方法逻辑，不实现属性数据，一个系统类单独一个文件

实体系统继承自AEntitySystem<T>，其中T为实体类型

实体系统生命周期接口方法：
- `Awake(T entity)`：唤醒实体时调用（继承自IAwake<T>）
- `Init(T entity)`：初始化实体时调用（继承自Init<T>）
- `AfterInit(T entity)`：初始化实体后调用（继承自IAfterInit<T>）
- `Enable(T entity)`：激活实体时调用（继承自IEnable<T>）
- `Disable(T entity)`：禁用实体时调用（继承自IDisable<T>）
- `Update(T entity)`：定时更新实体状态（继承自IUpdate<T>）
- `Destroy(T entity)`：销毁实体时调用（继承自IDestroy<T>）

例如：
```csharp
public class PlayerSystem : AEntitySystem<Player>, IAwake<Player>, IInit<Player>
{
    public void Awake(Player entity)
    {
        // 实体唤醒逻辑
    }

    public void Init(Player entity)
    {
        // 实体初始化逻辑
    }
}
```

组件系统继承自AComponentSystem<T, C>，其中T为实体类型，C为组件类型

组件系统生命周期接口方法：
- `Awake(T entity, C component)`：唤醒实体组件时调用（继承自IAwake<T, C>）
- `Init(T entity, C component)`：初始化实体组件时调用（继承自Init<T, C>）
- `AfterInit(T entity, C component)`：初始化实体组件后调用（继承自IAfterInit<T, C>）
- `Enable(T entity, C component)`：激活实体组件时调用（继承自IEnable<T, C>）
- `Disable(T entity, C component)`：禁用实体组件时调用（继承自IDisable<T, C>）
- `Destroy(T entity, C component)`：销毁实体组件时调用（继承自IDestroy<T, C>）

例如：
```csharp
public class MoveSystem : AComponentSystem<Actor, MoveComponent>, IAwake<Actor, MoveComponent>, IInit<Actor, MoveComponent>
{
    public void Awake(Actor entity, MoveComponent component)
    {
        // 组件唤醒逻辑
    }

    public void Init(Actor entity, MoveComponent component)
    {
        // 组件初始化逻辑
    }
}
```

除了生命周期接口方法是成员函数外，其余接口方法均为静态方法

除了生命周期接口方法需传实体和组件，其余静态接口方法只需传实体和对应需要的参数，组件在方法体里再获取
例如:
```csharp
public static void HandleLogic(AEntity entity, int param1, string param2)
{
    // 获取组件
    var component = entity.GetComponent<AComponent>();
    // 处理逻辑
}
```

静态业务逻辑接口方法应有summary摘要注释

命名空间为ECSGame.Module.**，其中**为模块名称（命名空间忽略横杆和下划线，并以大写字母开头）

常用引用命名空间：
```csharp
using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
```