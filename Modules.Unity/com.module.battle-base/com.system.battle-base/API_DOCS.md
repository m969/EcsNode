## BattleBaseEntitySystem

- Create(EcsEntity parent, int configId, BattleBaseQuality quality, int level, float energyRecoveryRate) → BattleBaseEntity
	- 创建战斗基地实体；初始化 Quality/Level/EnergyRecoveryRate，计算能量上限 `MaxEnergy = level * 100` 并将 `CurrentEnergy` 置满；挂载 `BattleBasePartListComponent`；注册到父级的 `BattleBaseListComponent`。
- SetQuality(BattleBaseEntity entity, BattleBaseQuality quality)
	- 设置战斗基地品级。
- SetLevel(BattleBaseEntity entity, int level)
	- 更新等级与能量上限（`MaxEnergy = level * 100`），若当前能量超过上限则裁剪；派发等级变更与能量变更事件。
- ConsumeEnergy(BattleBaseEntity entity, float amount) → bool
	- 当前能量不足返回 false；否则扣减能量并派发能量变更事件。
- RecoverEnergy(BattleBaseEntity entity, float amount)
	- 将能量恢复至不超过上限的值并派发能量变更事件。
- AddPart(BattleBaseEntity entity, long partConfigId, BattleBasePartType type, BattleBaseQuality quality, float energyCost) → BattleBasePartEntity?
	- 若部件品级高于战斗基地品级返回 null；否则创建部件，注册列表并派发部件变更事件（isAdded = true）。
- RemovePart(BattleBaseEntity entity, long partId)
	- 未找到部件直接返回；否则从列表与子实体中移除，并派发部件变更事件（isAdded = false）。

## BattleBasePartEntitySystem

- Create(BattleBaseEntity parent, int configId, BattleBasePartType type, BattleBaseQuality quality, float energyCost) → BattleBasePartEntity
	- 在战斗基地下创建部件实体，调用 `SetPartData` 赋值并注册到 `BattleBasePartListComponent`。
- SetPartData(BattleBasePartEntity entity, BattleBasePartType type, BattleBaseQuality quality, float energyCost)
	- 初始化或刷新部件类型、品级与能量消耗。

## BattleBaseListSystem

- GetBattleBase(EcsEntity entity, long id) → BattleBaseEntity
	- 直接按 Id 从 `Id2Entities` 取值；未注册时会因字典取值抛出异常。
- GetBattleBasesByConfigId(EcsEntity entity, long configId) → List<BattleBaseEntity>
	- 返回指定配置 Id 的列表，若不存在则创建空列表并返回引用。
- AddBattleBase(EcsEntity entity, BattleBaseEntity battleBase)
	- 注册战斗基地到 `Id2Entities`，并追加到对应配置 Id 的列表。
- RemoveBattleBase(EcsEntity entity, BattleBaseEntity battleBase)
	- 从 Id 与配置列表移除战斗基地，配置列表为空时清理键。

## BattleBasePartListSystem

- GetPart(BattleBaseEntity entity, long id) → BattleBasePartEntity?
	- 按 Id 尝试获取部件，未找到返回 null。
- AddPart(BattleBaseEntity entity, BattleBasePartEntity part)
	- 注册部件到 Id 映射，并追加到对应配置 Id 的列表（不存在时创建）。
- RemovePart(BattleBaseEntity entity, long partId)
	- 未找到部件直接返回；找到则从 Id 与配置列表移除，列表为空时清理键。
