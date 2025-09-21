# （奖励）模块程序设计文档

基于《design-documentation.md》与程序规范（program.instructions.md），在不破坏现有模块边界的前提下，给出奖励模块的实体/组件/系统/事件与配置接口设计，目标是最小复杂度实现“可配置、可审计、可回滚、多触发源、幂等发放”。

命名空间：`ECSGame.RewardModule`

常用引用：
- `using ECS;`
- `using System;`
- `using System.Collections;`
- `using System.Collections.Generic;`

第三方模块放置：`game.thirdparty.module/`（JSON 序列化、可选的异步库等）

---

## 数据结构设计

说明：配置接口以整型 Id 为唯一标识，字符串 Key 为辅助名称标识；所有类型与枚举提供必要的摘要说明。

### 配置接口设计

- IRewardEntryConfig（奖励条目配置，最小粒度）
	- Id:int（唯一）
	- Key:string（辅助标识，用于本地化映射）
	- Type:RewardType（奖励类型）
	- RefId:int（引用的货币/道具/经验等在外部系统中的 Id）
	- Amount:long（数量，统一最小单位）
	- Meta:Dictionary<string,string>?（元信息，如来源标签、展示参数）

- IRewardPackageConfig（奖励包配置，由多个条目组合）
	- Id:int（唯一）
	- Key:string（辅助标识）
	- Entries:int[]（条目 Id 列表，对应 IRewardEntryConfig.Id）
	- Tags:string[]?
	- Version:int（版本号）

- IRewardLootTableConfig（可选，掉落表配置）
	- Id:int
	- Key:string
	- Items:List<{ EntryId:int, Weight:int }>

校验要求（导入/热更/CI 阶段）：
- 唯一性：EntryId/PackageId 全局唯一且稳定。
- 参照完整性：RefId 在经济与背包模块中合法存在。
- 数值边界：Amount 禁止负值，精度与上限统一；掉落表权重 > 0。
- 逻辑一致性：时间窗/每日上限/唯一性标记/黑白名单等冲突检查。
- 本地化：必须字段提供本地化 Key。

### 补充类型与枚举

- enum RewardType
	- Currency：货币
	- Item：道具
	- Exp：经验
	- Energy：体力/能量
	- Custom：自定义（由业务方解释）
	- LootTable：掉落表（需展开为条目）

- enum GrantStatus
	- Success：全部成功
	- Partial：部分成功（少量失败/容量不足）
	- Failed：全部失败
	- Queued：入队等待（内部状态）

- enum ErrorDomain
	- ConfigMissing：配置缺失/不合法
	- ConditionNotMet：条件不满足（时间窗/等级/名单/平台地区等）
	- CapacityInsufficient：容量不足（背包或货币上限）
	- DedupeConflict：幂等键冲突
	- RateLimited：速率限制/冷却
	- BackendError：后端错误（存储/网络等）
	- Unknown：未知错误

- RewardEntryDefinition（运行期用于组合/展开的条目描述）
	- Type:RewardType
	- RefId:int
	- Amount:long
	- Meta:Dictionary<string,string>?

- ExpandResult（预览展开结果）
	- Entries:List<RewardEntryDefinition>（合并同类项后的最终列表）
	- Logs:List<string>?（标准化/合并/替换提示）

- GrantContext（发放上下文）
	- PlayerId:long
	- Source:string（触发源：任务/签到/活动/邮件等）
	- Reason:string（人类可读原因）
	- TraceId:string（链路追踪）
	- DedupeKey:string?（幂等键）
	- Region:string
	- Platform:string
	- Time:DateTime

- GrantResult（发放结果）
	- Status:GrantStatus
	- EntriesApplied:List<RewardEntryDefinition>
	- EntriesSkipped:List<{ Entry:RewardEntryDefinition, Reason:string }>
	- Error:{ Domain:ErrorDomain, Code:int, Message:string }?
	- TxId:string（事务/流水号）
	- Logs:List<string>?

---

## 实体与组件设计

说明：实体与组件仅承载数据属性，不实现方法逻辑；实体继承自 `EcsEntity`（已包含 Id、Parent），并标记 `partial`；组件继承自 `EcsComponent`。

### 实体（Entities）

- RewardServiceEntity（模块根实体，partial）
	- Summary：奖励模块服务根，挂载配置/队列/审计等组件，作为统一入口。

- PlayerRewardEntity（玩家维度实体，partial）
	- Summary：承载玩家侧奖励历史、幂等键、每日上限等状态。

### 组件（Components）

- RewardConfigComponent
	- Summary：缓存奖励条目与包配置、掉落表，提供快速查询。
	- Properties：
		- Entries:Dictionary<int,IRewardEntryConfig>
		- Packages:Dictionary<int,IRewardPackageConfig>
		- LootTables:Dictionary<int,IRewardLootTableConfig>

- RewardHistoryComponent（挂载于 PlayerRewardEntity）
	- Summary：保存幂等键与发放流水索引，支持去重与审计查询。
	- Properties：
		- DedupeKeys:HashSet<string>
		- TxIndex:List<string>

- RewardQueueComponent（挂载于 RewardServiceEntity）
	- Summary：失败重试/补偿队列（轻量信息，重数据由外部持久化）。
	- Properties：
		- Pending:list<{ QueueId:string, PlayerId:long, PackageId:int, NextAt:DateTime, Attempt:int, Reason:string }>

- RewardAuditComponent（可选，挂于 RewardServiceEntity）
	- Summary：模块内审计缓冲，供外部订阅/落库。
	- Properties：
		- Logs:Queue<string>

---

## 系统业务设计

说明：系统类只实现方法逻辑，不实现属性数据。实体系统继承 `AEntitySystem<T>`；组件系统继承 `AComponentSystem<T,C>`。所有业务方法为静态方法；仅传入实体与必要参数，组件在方法内部获取。

### 实体系统设计

- RewardServiceSystem : AEntitySystem<RewardServiceEntity>
	- 实现生命周期接口：`IAwake<RewardServiceEntity>`, `IInit<RewardServiceEntity>`, `IAfterInit<RewardServiceEntity>`, `IEnable<RewardServiceEntity>`, `IDisable<RewardServiceEntity>`, `IDestroy<RewardServiceEntity>`
	- 静态业务接口：
		- Create(EcsEntity parent, IRewardConfigProvider provider) : RewardServiceEntity
			- Summary：创建模块根实体，挂载配置组件并完成初始化。
		- GetPlayerReward(EcsEntity parentOrService, long playerId) : PlayerRewardEntity
			- Summary：获取/创建玩家奖励实体，确保挂载历史组件。

- PlayerRewardSystem : AEntitySystem<PlayerRewardEntity>
	- 实现生命周期接口：`IAwake<PlayerRewardEntity>`, `IInit<PlayerRewardEntity>`, `IAfterInit<PlayerRewardEntity>`, `IEnable<PlayerRewardEntity>`, `IDisable<PlayerRewardEntity>`, `IDestroy<PlayerRewardEntity>`

### 组件系统设计

- RewardConfigSystem : AComponentSystem<RewardServiceEntity, RewardConfigComponent>
	- 实现生命周期接口：`IAwake<RewardServiceEntity,RewardConfigComponent>`, `IInit<RewardServiceEntity,RewardConfigComponent>`, `IAfterInit<RewardServiceEntity,RewardConfigComponent>`, `IEnable<RewardServiceEntity,RewardConfigComponent>`, `IDisable<RewardServiceEntity,RewardConfigComponent>`, `IDestroy<RewardServiceEntity,RewardConfigComponent>`
	- 静态方法：
		- Reload(RewardServiceEntity service, IRewardConfigProvider provider) : void
			- Summary：热更/重新加载配置并完成强校验。

- RewardGrantSystem : AComponentSystem<RewardServiceEntity, RewardConfigComponent>
	- 实现生命周期接口：`IAwake<RewardServiceEntity,RewardConfigComponent>`, `IInit<RewardServiceEntity,RewardConfigComponent>`, `IAfterInit<RewardServiceEntity,RewardConfigComponent>`, `IEnable<RewardServiceEntity,RewardConfigComponent>`, `IDisable<RewardServiceEntity,RewardConfigComponent>`, `IDestroy<RewardServiceEntity,RewardConfigComponent>`
	- 静态方法（核心业务）：
		- PreviewReward(RewardServiceEntity service, GrantContext ctx, int packageId | RewardEntryDefinition[] def) : ExpandResult
			- Summary：读取并展开奖励（含掉落表），合并同类项与单位标准化，不落库；派发 OnRewardPreviewed。
		- CanGrant(RewardServiceEntity service, GrantContext ctx, int packageId | RewardEntryDefinition[] def) : { ok:bool, reasons:string[] }
			- Summary：校验时间窗/每日上限/等级/平台/地区/名单/唯一性/速率限制，不产生副作用。
		- GrantReward(RewardServiceEntity service, PlayerRewardEntity player, GrantContext ctx, int packageId | RewardEntryDefinition[] def, string? dedupeKey) : GrantResult
			- Summary：检查幂等键，执行经济与背包结算（委托外部模块），派发 OnRewardGranted/OnRewardFailed；失败时可入队并派发 OnRewardQueued。

- RewardQueueSystem : AComponentSystem<RewardServiceEntity, RewardQueueComponent>
	- 实现生命周期接口同上
	- 静态方法：
		- Enqueue(RewardServiceEntity service, long playerId, int packageId, DateTime nextAt, int attempt, string reason) : void
		- DequeueAndProcess(RewardServiceEntity service, int maxCount) : int
			- Summary：批量拉取队列并调用 GrantReward 进行重试，返回处理数量。

---

## 系统事件接口设计（派发接口继承 IDispatch）

- IOnRewardPreviewed : IDispatch
	- void OnRewardPreviewed(EcsEntity entity, GrantContext ctx, ExpandResult result)
	- Summary：预览完成后派发，便于展示与埋点。

- IOnRewardGranted : IDispatch
	- void OnRewardGranted(EcsEntity entity, GrantContext ctx, GrantResult result)
	- Summary：发放成功后派发，便于成就/任务推进与弹窗。

- IOnRewardFailed : IDispatch
	- void OnRewardFailed(EcsEntity entity, GrantContext ctx, ErrorDomain domain, int code, string message)
	- Summary：发放失败后派发（含入队前），便于告警/监控/GM。

- IOnRewardQueued : IDispatch
	- void OnRewardQueued(EcsEntity entity, long playerId, string queueId, DateTime nextAt, int attempt, string reason)
	- Summary：失败进入补偿队列时派发，供后台任务与客服工具订阅。

---

## 系统业务逻辑方法约束

- 所有业务方法为静态方法；不在系统类中持有状态。
- 方法仅接收实体与必要参数；组件在方法内通过 `entity.GetComponent<T>()` 获取。
- 提供必要的 XML summary 注释；保持数据驱动，消除分支与特殊情况。

---

## 第三方模块与集成建议（game.thirdparty.module/）

- JSON：Newtonsoft.Json（或同类）用于配置序列化/反序列化。
- 二进制缓存（可选）：MessagePack/ProtoBuf。
- 异步（可选）：UniTask。

---

## 说明与兼容性

- 配置使用整型 Id + 字符串 Key 的双标识方案，满足规范与稳定性要求。
- RewardType 包含 LootTable，运行期由 Preview/Grant 流程统一展开，避免在外层分支处理。
- 幂等通过 PlayerRewardEntity 的 RewardHistoryComponent.DedupeKeys 保证；相同 DedupeKey 重试直接返回首个结果。
- 不包含任何 UI/经济结算/背包实现；通过系统事件与外部模块集成，遵守“零破坏向后兼容”。
