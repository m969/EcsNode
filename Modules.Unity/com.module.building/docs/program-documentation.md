# SLG游戏建筑建造程序设计文档

---
**文档说明：**
本设计文档详细描述了SLG游戏建筑建造模块的功能、数据结构、系统接口及测试方案，便于开发、维护和扩展。
---

## 1. 模块功能概述
实现SLG游戏建筑的建造、升级、拆除等功能，支持多种建筑类型、建造队列、资源消耗、建筑效果。模块采用EcsEntity、EcsComponent、EcsSystem架构，便于扩展和维护。

## 2. 数据结构设计
### 2.1 实体（Entities）
**BuildingEntity（继承自EcsEntity）**
- string Type：建筑类型（如"Farm"、"Barracks"等）
- int Level：建筑等级
- Vector2Int Position：地图坐标（x, y）
- BuildingState State：建造/升级/完成/拆除等状态（枚举类型）
- int OwnerId：所属玩家ID

### 2.2 组件（Components）
**BuildingStateComponent**
- BuildingState State：当前状态（建造中/升级中/已完成/待拆除等）
- float TimeLeft：剩余时间（秒）

**BuildingEffectComponent**
- int ResourceBonus：资源产出加成
- int PopulationBonus：人口上限加成
- int DefenseBonus：防御加成

**BuildingCostComponent**
- Dictionary<string, int> Cost：资源类型及数量（如{"Wood":100, "Stone":50}）
- float BuildTime：建造/升级所需时间（秒）

### 2.3 系统（Systems）接口

**BuildingSystem（AEntitySystem<BuildingEntity>）**
- // 生命周期接口为成员函数
- void Awake(BuildingEntity entity)
- void Init(BuildingEntity entity)
- void AfterInit(BuildingEntity entity)
- void Enable(BuildingEntity entity)
- void Disable(BuildingEntity entity)
- void Update(BuildingEntity entity)
- void Destroy(BuildingEntity entity)
- // 其余接口为静态方法
- static void HandleBuildingLogic(BuildingEntity entity) // 通用建筑业务逻辑入口，可扩展具体功能

**BuildingConstructionSystem（AEntitySystem<BuildingEntity>）**
- // 生命周期接口为成员函数
- void Awake(BuildingEntity entity)
- void Init(BuildingEntity entity)
- void AfterInit(BuildingEntity entity)
- void Enable(BuildingEntity entity)
- void Disable(BuildingEntity entity)
- void Update(BuildingEntity entity)
- void Destroy(BuildingEntity entity)
- // 其余接口为静态方法
- static void StartBuild(BuildingEntity entity, int buildLevel) // 发起建造，buildLevel为目标等级，组件在方法体里获取
- static void UpgradeBuild(BuildingEntity entity, int targetLevel) // 发起升级，targetLevel为目标等级，组件在方法体里获取
- static void CancelBuild(BuildingEntity entity) // 取消建造/升级
- static void RemoveBuild(BuildingEntity entity) // 拆除建筑



**BuildingStateSystem（AComponentSystem<BuildingEntity, BuildingStateComponent>）**
- // 生命周期接口为成员函数
- void Awake(BuildingEntity entity, BuildingStateComponent component)
- void Init(BuildingEntity entity, BuildingStateComponent component)
- void AfterInit(BuildingEntity entity, BuildingStateComponent component)
- void Enable(BuildingEntity entity, BuildingStateComponent component)
- void Disable(BuildingEntity entity, BuildingStateComponent component)
- void Destroy(BuildingEntity entity, BuildingStateComponent component)
- // 其余接口为静态方法
- static void OnBuildComplete(BuildingEntity entity) // 建造完成回调，组件在方法体里获取

**BuildingEffectSystem（AComponentSystem<BuildingEntity, BuildingEffectComponent>）**
- // 生命周期接口为成员函数
- void Awake(BuildingEntity entity, BuildingEffectComponent component)
- void Init(BuildingEntity entity, BuildingEffectComponent component)
- void AfterInit(BuildingEntity entity, BuildingEffectComponent component)
- void Enable(BuildingEntity entity, BuildingEffectComponent component)
- void Disable(BuildingEntity entity, BuildingEffectComponent component)
- void Destroy(BuildingEntity entity, BuildingEffectComponent component)


**BuildingCostSystem（AComponentSystem<BuildingEntity, BuildingCostComponent>）**
- // 生命周期接口为成员函数
- void Awake(BuildingEntity entity, BuildingCostComponent component)
- void Init(BuildingEntity entity, BuildingCostComponent component)
- void AfterInit(BuildingEntity entity, BuildingCostComponent component)
- void Enable(BuildingEntity entity, BuildingCostComponent component)
- void Disable(BuildingEntity entity, BuildingCostComponent component)
- void Destroy(BuildingEntity entity, BuildingCostComponent component)
- // 其余接口为静态方法
- static bool CheckCost(BuildingEntity entity) // 校验资源，组件在方法体里获取
- static void DeductCost(BuildingEntity entity) // 扣除资源，组件在方法体里获取


**BuildingInteractionSystem（AEntitySystem<BuildingEntity>）**
- // 生命周期接口为成员函数
- void Awake(BuildingEntity entity)
- void Enable(BuildingEntity entity)
- void Disable(BuildingEntity entity)
- void Destroy(BuildingEntity entity)
- // 其余接口为静态方法
- static void OnClick(BuildingEntity entity) // 玩家点击
- static void OnMove(BuildingEntity entity, Vector2Int newPosition) // 移动建筑
- static void OnPlace(BuildingEntity entity, Vector2Int position) // 放置建筑

## 3. 逻辑流程设计
1. 玩家选择地块并发起建造请求，系统校验资源和建造条件。
2. 校验通过后，建筑实体加入建造队列，扣除资源。
3. 建造队列定时更新，建造时间结束后，建筑状态变为已完成。
4. 建筑完成后，触发建筑效果，影响资源产出、人口等。
5. 支持建筑升级、拆除等操作，流程类似。

## 4. 测试方案设计
- 单元测试：验证各系统方法的正确性，如建造、升级、资源校验等。
- 集成测试：模拟完整建造流程，测试实体、组件、系统协作。
- 边界测试：测试资源不足、异常操作等情况。
- 性能测试：大规模建筑建造下的系统性能。

---
**附录：类型说明**
- Vector2Int：二维整型坐标结构，包含x、y。
- BuildingState：建筑状态枚举，如Constructing、Upgrading、Completed、ToBeRemoved等。
