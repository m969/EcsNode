# 资源地块策划文档

---

## 一、ECS 设计目标

本模块核心功能包括：

- 地块作为资源点，具备类型（基础/稀有）、存量、产出效率等属性

---

## 二、ECS 实体与组件设计

### 1. 实体（Entities）

- ResourceNode: 资源地块（如森林、矿洞、联盟矿场等），包含所有资源属性

---

### 2. 组件（Components）

#### A. 资源地块相关

**ResourceNode**（实体）

- resourceType: ResourceType（枚举：粮食、木材、矿石、宝石等）
- resourceTier: ResourceTier（基础/稀有）
- currentAmount: float（当前存量）
- maxAmount: float（最大存量）
- baseYield: float（基础单次产量）
- efficiency: float（地块采集效率）
- position: Vector2Int（地图坐标）
- isBeingCollected: bool（是否正被采集）
- lastCollectedTime: float（离线计算用）
- protectionTimeRemaining: float（保护时间倒计时）
- ownerAllianceId: int（所属联盟ID）

---

### 3. 系统（Systems）设计

- **ResourceNodeSystem**  
  资源地块实体系统，实现与 ResourceNode 实体一一对应的逻辑处理（如生命周期、状态同步等，仅包含方法逻辑，不含属性数据）  
  **主要接口方法：**  
  - `Awake(ResourceNode node)`：初始化资源地块  
  - `Update(ResourceNode node)`：定时更新地块状态  
  - `Destroy(ResourceNode node)`：销毁资源地块时的处理  
  - `SyncState(ResourceNode node)`：同步资源地块状态到客户端

- **CollectionSystem**  
  采集调度，定时检查采集单位，计算采集量，更新资源与玩家收益，支持离线累计（CollectorComponent + ResourceNodeComponent + OwnershipComponent）  
  **主要接口方法：**  
  - `StartCollection(CollectorComponent collector, ResourceNode node)`：开始采集  
  - `UpdateCollection(CollectorComponent collector, ResourceNode node, float deltaTime)`：采集进度更新  
  - `FinishCollection(CollectorComponent collector, ResourceNode node)`：采集完成结算  
  - `CalculateYield(CollectorComponent collector, ResourceNode node)`：计算采集产量  
  - `HandleOfflineCollection(Player player)`：处理离线采集收益

---

## 三、ECS 模块关系简图（文字描述）

- ResourceNode:
  - ResourceYieldBonusComponent
  - OwnershipComponent

- 系统:
  - ResourceNodeSystem → 资源地块实体系统
  - CollectionSystem → 采集逻辑
  - AutoDispatchSystem → 自动派遣
  - OfflineYieldSystem → 离线收益
  - ProtectionSystem → 保护与竞争

---

## 四、补充说明

- **数据存储与同步**: ResourceNode 数据可存储于服务器内存+数据库，Component 数据可序列化同步到客户端。
- **性能优化**: 大地图资源点采用分块加载，Tag 组件优化系统筛选效率。
- **扩展性**: 可灵活添加新资源类型、采集单位、事件类型，系统低耦合便于测试与热更。

---

## 五、核心组成总结

- 实体: ResourceNode
- 组件: ResourceNodeComponent、CollectorComponent、ResourceYieldBonusComponent、OwnershipComponent、BuildingBonusComponent 等
- 系统: ResourceNodeSystem、CollectionSystem、AutoDispatchSystem、OfflineYieldSystem、ProtectionSystem、EventSystem 等

---
**实体**: ResourceNode  
**组件**: ResourceNodeComponent、CollectorComponent、ResourceYieldBonusComponent、OwnershipComponent、BuildingBonusComponent 等  
**系统**: ResourceNodeSystem、CollectionSystem、AutoDispatchSystem、OfflineYieldSystem、ProtectionSystem、EventSystem 等