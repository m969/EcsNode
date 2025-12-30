# （战斗基地）模块程序设计文档

## 设计规范如下：

基于EcsNode框架，EcsNode是基于ECS（Entity-Component-System）架构的Unity游戏开发框架，通过实体、组件和系统的组合方式实现高效、灵活的游戏逻辑。

## EcsNode核心库已有的实体和组件（在ECS命名空间下）有：
- 实体基类 EcsEntity，包含以下属性和接口：
    - Id（long）属性，实体唯一id。
    - ConfigId（long）属性，配置id。
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
            entity.Dispatch<IStartBuildHandler>((system) => system.OnStartBuildHandle(entity, count));
            ```

- 组件基类 EcsComponent：
    - 包含Entity（EcsEntity）属性，用于关联所属实体。

### 命名空间
所有模块代码命名空间为 `ECSGame.BattleBaseModule`

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
5. 不提供配置实现，配置由开发者自行设计，所有功能数据都通过组件和实体实现

## 文档模板

- 实体设计
    - 战斗基地实体 (BattleBaseEntity)
        - 战斗基地实体用途：作为玩家在战斗中的核心载体（战舰），承载属性、能量和部件。
    	- 战斗基地实体字段设计：
            - Quality (BattleBaseQuality)：品级（下品、中品、上品、极品、仙品）。
            - Level (int)：级别（1-5级）。
            - CurrentEnergy (float)：当前能量值。
            - MaxEnergy (float)：能量上限（受级别影响）。
            - EnergyRecoveryRate (float)：能量恢复率。
    - 战斗基地部件实体 (BattleBasePartEntity)
        - 战斗基地部件实体用途：作为战斗基地的子实体，提供具体战斗能力（武器、护盾、引擎）。
    	- 战斗基地部件实体字段设计：
            - PartType (BattleBasePartType)：部件类型（武器、护盾、引擎）。
            - Quality (BattleBaseQuality)：部件品级。
            - EnergyCost (float)：能量消耗值。

    - 战斗基地实体系统设计 (BattleBaseEntitySystem)
        - 战斗基地实体系统功能接口设计：
            - SetQuality(BattleBaseEntity entity, BattleBaseQuality quality)：设置品级。
            - SetLevel(BattleBaseEntity entity, int level)：设置级别，并更新能量上限。
            - ConsumeEnergy(BattleBaseEntity entity, float amount)：消耗能量，返回是否成功。
            - RecoverEnergy(BattleBaseEntity entity, float amount)：恢复能量。
            - AddPart(BattleBaseEntity entity, long partConfigId, BattleBasePartType type, BattleBaseQuality quality, float energyCost)：添加部件（需检查品级限制）。
            - RemovePart(BattleBaseEntity entity, long partId)：移除部件。
    - 战斗基地部件实体系统设计 (BattleBasePartEntitySystem)
        - 战斗基地部件实体系统功能接口设计：
            - SetPartData(BattleBasePartEntity entity, BattleBasePartType type, BattleBaseQuality quality, float energyCost)：初始化部件数据。

- 组件设计
    - 战斗基地列表组件 (BattleBaseListComponent) 用于存储和管理该实体
        - Id2Entities 字典，Key为实体Id，Value为实体对象
        - ConfigId2Entities 字典，Key为配置Id，Value为实体对象列表
    - 战斗基地部件列表组件 (BattleBasePartListComponent) 用于存储和管理该实体
        - Id2Entities 字典，Key为实体Id，Value为实体对象
        - ConfigId2Entities 字典，Key为配置Id，Value为实体对象列表
        
    - 战斗基地列表组件系统设计 (BattleBaseListSystem)
        - 战斗基地列表组件系统功能接口设计：
            - GetBattleBase(long id)：获取战斗基地。
            - GetBattleBasesByConfigId(long configId)：根据配置ID获取战斗基地列表。
    - 战斗基地部件列表组件系统设计 (BattleBasePartListSystem)
        - 战斗基地部件列表组件系统功能接口设计：
            - GetPart(long id)：获取部件。

- 其他类型补充
    - 流程节点派发接口补充
        - IBattleBaseLevelChangedHandler：void OnBattleBaseLevelChanged(BattleBaseEntity entity, int oldLevel, int newLevel)
        - IBattleBaseEnergyChangedHandler：void OnBattleBaseEnergyChanged(BattleBaseEntity entity, float currentEnergy, float maxEnergy)
        - IBattleBasePartChangedHandler：void OnBattleBasePartChanged(BattleBaseEntity entity, BattleBasePartEntity part, bool isAdded)
    - 基础数据类型补充
    - 枚举补充
        - BattleBaseQuality：Common (下品), Uncommon (中品), Rare (上品), Epic (极品), Legendary (仙品)
        - BattleBasePartType：Weapon (武器), Shield (护盾), Engine (引擎)
