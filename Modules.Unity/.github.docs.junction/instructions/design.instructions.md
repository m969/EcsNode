---
applyTo: "design-documentation.md"
---

模块程序设计文档：

## 设计规范如下：

基于EcsNode框架，EcsNode是基于ECS（Entity-Component-System）架构的Unity游戏开发框架，通过实体、组件和系统的组合方式实现高效、灵活的游戏逻辑。

## EcsNode核心库已有实体和组件（在ECS命名空间下）有：
- 实体基类 EcsEntity，包含以下属性和接口：
    - Id（long）属性。
    - Parent（EcsEntity）属性。
    - EcsNode 所属Ecs域根节点。
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
所有模块代码命名空间为 `ECSGame.**Module`（**为模块名，忽略横杆下划线，大写开头）

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

## 文档模板

- 配置接口设计（没有需求则留空，命名以Config结尾，如IItemConfig）
    - 配置项1
        - 配置项1用途
        - 配置项1字段设计
    - 配置项2
        - 配置项2用途
        - 配置项2字段设计

- 其他类型补充
    - 流程节点派发接口补充（方便外部监听扩展，例如其他模块依赖或视图刷新，一个节点一个接口，方法命名以On开头，没有需求则留空）
        - 为遵循开闭原则和依赖倒置原则，可扩展的系统功能应提供节点接口派发到外部由开发者自定义扩展逻辑
        - 例如：ITaskActivated：void OnTaskActivated(int taskId)，节点派发接口继承 `IDispatch`
    - 基础数据类型补充（没有需求则留空）
    - 枚举补充（没有需求则留空）

- 实体设计（没有需求则留空）
    - 实体1
        - 实体1用途
    	- 实体1字段设计
    - 实体2
        - 实体2用途
    	- 实体2字段设计

    - 实体1系统设计（以 System 为后缀）
        - 实体1系统功能接口设计
    - 实体2系统设计（以 System 为后缀）
        - 实体2系统功能接口设计

    - 实体1列表组件（Entity1ListComponent） 用于存储和管理该实体
        - Id2Entities 字典，Key为实体Id，Value为实体对象
        - ConfigId2Entities 字典，Key为配置Id，Value为实体对象列表（没有需求则留空）
        - 其他
    - 实体2列表组件（Entity2ListComponent） 用于存储和管理该实体
        - Id2Entities 字典，Key为实体Id，Value为实体对象
        - ConfigId2Entities 字典，Key为配置Id，Value为实体对象列表（没有需求则留空）
        - 其他
        
    - 实体1列表组件系统设计（命名省略 Component 并以 System 为后缀）
        - 实体1列表组件系统功能接口设计
    - 实体2列表组件系统设计（命名省略 Component 并以 System 为后缀）
        - 实体2列表组件系统功能接口设计

- 组件设计（没有需求则留空）
    - 组件A
    	- 组件A用途
    	- 组件A字段设计
    - 组件B
    	- 组件B用途
    	- 组件B字段设计

    - 组件A系统设计（命名省略 Component 并以 System 为后缀）
        - 组件A系统功能接口设计
    - 组件B系统设计（命名省略 Component 并以 System 为后缀）
        - 组件B系统功能接口设计