# BattleBase 模型层 API 文档

## 实体（Entities）

### BattleBaseEntity
- 用途：玩家战斗核心载体（战舰），承载基础属性、能量与部件。
- 字段：
	- ConfigId (int)：战斗基地配置Id。
	- Quality (BattleBaseQuality)：品级（Common/Uncommon/Rare/Epic/Legendary）。
	- Level (int)：级别（1-5），影响能量上限（MaxEnergy = Level × 100）。
	- CurrentEnergy (float)：当前能量值。
	- MaxEnergy (float)：能量上限。
	- EnergyRecoveryRate (float)：能量恢复率。
- 关系/组件：
	- 子实体：`BattleBasePartEntity`（通过 `AddChild<BattleBasePartEntity>()` 挂载）。
	- 组件：默认挂载 `BattleBasePartListComponent` 用于管理部件列表。

### BattleBasePartEntity
- 用途：战斗基地的部件子实体，提供具体战斗能力（武器/护盾/引擎）。
- 字段：
	- ConfigId (int)：部件配置Id。
	- PartType (BattleBasePartType)：部件类型。
	- Quality (BattleBaseQuality)：部件品级。
	- EnergyCost (float)：能量消耗值。
- 关系：归属于 `BattleBaseEntity` 作为子实体。

## 组件（Components）

### BattleBaseListComponent
- 作用：索引与管理全部战斗基地实体。
- 字段：
	- Id2Entities：`Dictionary<long, BattleBaseEntity>`，按实体 Id 索引战斗基地。
	- ConfigId2Entities：`Dictionary<int, List<BattleBaseEntity>>`，按配置 Id 分组战斗基地。
- 使用：挂载在域根或上层管理实体，由系统通过 `BattleBaseListSystem` 读写。

### BattleBasePartListComponent
- 作用：索引与管理单个战斗基地的部件。
- 字段：
	- Id2Entities：`Dictionary<long, BattleBasePartEntity>`，按部件 Id 索引。
	- ConfigId2Entities：`Dictionary<int, List<BattleBasePartEntity>>`，按配置 Id 分组。
- 使用：由 `BattleBaseEntitySystem.Create` 自动挂载到战斗基地，用 `BattleBasePartListSystem` 维护。

## 枚举（Enums）

- BattleBaseQuality：Common（下品）、Uncommon（中品）、Rare（上品）、Epic（极品）、Legendary（仙品）。
- BattleBasePartType：Weapon（武器）、Shield（护盾）、Engine（引擎）。

## 事件/派发接口（Handlers）

- IBattleBaseLevelChangedHandler：`OnBattleBaseLevelChanged(BattleBaseEntity entity, int oldLevel, int newLevel)`。
- IBattleBaseEnergyChangedHandler：`OnBattleBaseEnergyChanged(BattleBaseEntity entity, float currentEnergy, float maxEnergy)`。
- IBattleBasePartChangedHandler：`OnBattleBasePartChanged(BattleBaseEntity entity, BattleBasePartEntity part, bool isAdded)`。
