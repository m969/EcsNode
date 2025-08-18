# （建筑建造）程序设计文档

---
**文档说明：**
本设计文档详细描述了游戏建筑建造模块的功能、数据结构、系统接口，便于开发、维护和扩展。
---

## 1. 模块功能概述
实现游戏建筑的放置、建造、升级、拆除等功能，支持多种建筑类型。模块采用EcsEntity、EcsComponent、EcsSystem架构，便于扩展和维护。

## 2. 数据结构设计
### 2.1 实体（Entities）
**BuildingEntity（继承自EcsEntity）**
- int Type：建筑类型
- int Level：建筑等级
- Vector2Int Position：地图坐标（x, y）
- BuildingState State：建造/升级/完成/拆除等状态（枚举类型）
- long OwnerId：所属玩家ID（1.0.2 起使用长整型，与玩家ID统一）

### 2.2 组件（Components）
**BuildingStateComponent**
- BuildingState State：当前状态（建造中/升级中/已完成/待拆除等）
- float TimeLeft：剩余时间（秒）

### 2.3 系统（Systems）接口

通用约定：
- 系统类为实例类（不使用static修饰），仅包含方法逻辑，不包含属性数据
- 业务方法为静态方法，仅传入实体与必要参数，组件在方法内获取
- 实体系统：继承 AEntitySystem<T>，实现 IAwake<T>, IInit<T>, IAfterInit<T>, IEnable<T>, IDisable<T>, IDestroy<T>
- 组件系统：继承 AComponentSystem<T, C>，实现 IAwake<T, C>, IInit<T, C>, IAfterInit<T, C>, IEnable<T, C>, IDisable<T, C>, IDestroy<T, C>

BuildingSystem（实体系统）
- 继承与生命周期：
  - 继承：AEntitySystem<BuildingEntity>
  - 实现：IAwake<BuildingEntity>, IInit<BuildingEntity>, IAfterInit<BuildingEntity>, IEnable<BuildingEntity>, IDisable<BuildingEntity>, IDestroy<BuildingEntity>
- 业务方法（静态）：
  - static BuildingEntity Create(EcsEntity parent, int type, Vector2Int position, long ownerId, int level = 1)
  - static void HandleBuildingLogic(BuildingEntity entity)

BuildingConstructionSystem（实体系统）
- 继承与生命周期：
  - 继承：AEntitySystem<BuildingEntity>
  - 实现：IAwake<BuildingEntity>, IInit<BuildingEntity>, IAfterInit<BuildingEntity>, IEnable<BuildingEntity>, IDisable<BuildingEntity>, IDestroy<BuildingEntity>
- 业务方法（静态）：
  - static void StartBuild(BuildingEntity entity, int targetLevel)
  - static void UpgradeBuild(BuildingEntity entity, int targetLevel)
  - static void CancelBuild(BuildingEntity entity)
  - static void RemoveBuild(BuildingEntity entity)

BuildingStateSystem（组件系统）
- 继承与生命周期：
  - 继承：AComponentSystem<BuildingEntity, BuildingStateComponent>
  - 实现：IAwake<BuildingEntity, BuildingStateComponent>, IInit<BuildingEntity, BuildingStateComponent>, IAfterInit<BuildingEntity, BuildingStateComponent>, IEnable<BuildingEntity, BuildingStateComponent>, IDisable<BuildingEntity, BuildingStateComponent>, IDestroy<BuildingEntity, BuildingStateComponent>
- 业务方法（静态）：
  - static void UpdateState(BuildingEntity entity, float deltaTime)
  - static void OnBuildComplete(BuildingEntity entity)

### 2.4 系统事件派发接口（可扩展）
为遵循开闭原则与依赖倒置原则，系统在关键节点派发事件到外部扩展逻辑（均继承IDispatch，示例）：
- IOnStartBuild: void OnStartBuild(EcsEntity entity, int targetLevel)
- IOnUpgradeBuild: void OnUpgradeBuild(EcsEntity entity, int targetLevel)
- IOnCancelBuild: void OnCancelBuild(EcsEntity entity)
- IOnRemoveBuild: void OnRemoveBuild(EcsEntity entity)
- IOnBuildComplete: void OnBuildComplete(EcsEntity entity)
- IOnBuildingStateTick: void OnBuildingStateTick(EcsEntity entity, float deltaTime)

---
**附录：类型说明**
- Vector2Int：二维整型坐标结构，包含x、y。
- BuildingState：建筑状态枚举，如Constructing、Upgrading、Completed、ToBeRemoved等。
