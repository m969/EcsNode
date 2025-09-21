# （奖励）模块功能设计文档

基于最新《require-documentation.md》，定义奖励发放模块的功能设计与边界，确保“可配置、可审计、可回滚、多触发源、多奖励类型、零破坏向后兼容”。

— 目标：最小复杂度实现可靠发放；消除特殊分支，通过数据驱动与统一流程覆盖常见场景。


## 模块功能设计概述

- 能力范围：
	- 奖励定义与解析：支持 Currency/Item/Exp/Energy/Custom 与组合包。
	- 触发与条件：来自任务、关卡、签到、成就、活动、邮件/补偿、兑换码、GM 等。
- 依赖与边界：
	- 复用已有模块：玩家档案/身份、经济/货币、物品/背包、事件总线、存储/持久化、时间/时钟。
	- 非本模块职责：UI 展示、具体经济结算实现、道具效果实现。
- 第三方模块放置约定：`game.thirdparty.module/`（文档内引用清单，最终以工程集成选择为准）。


## 模块功能设计列表

- 配置功能设计
- 数据结构设计
- 接口功能设计
- 功能流程设计
- 事件节点设计


## 配置功能设计

- 配置来源：
	- 文本：JSON/YAML；资源化：ScriptableObject（若在 Unity 侧使用）。
	- 支持本地化 Key（名称/描述），用于前端展示。
- 配置组织：
	- RewardEntry：最小奖励粒度，含类型、引用ID、数量、元信息。
	- RewardPackage：一组 RewardEntry 的组合包，带 tags 与 version。
- 校验器（导入/热更/CI 阶段强校验）：
	- 唯一性：RewardId/PackageId 唯一且稳定。
	- 参照完整性：refId 指向的货币/道具在经济与背包模块中合法存在。
	- 数值边界：amount 精度/上限；禁止负值；大数/小数策略一致。
	- 逻辑一致性：时间窗、每日上限、唯一性标记、黑白名单互斥冲突检查。
	- 本地化：必须字段提供本地化 Key；缺失时报错或降级。


## 数据结构设计

- 枚举与常量：
	- RewardType：Currency | Item | Exp | Energy | Custom | LootTable。
	- GrantStatus：Success | Partial | Failed | Queued（仅内部状态）。
	- ErrorDomain：ConfigMissing | ConditionNotMet | CapacityInsufficient | DedupeConflict | RateLimited | BackendError | Unknown。
- 核心模型：
	- RewardEntry：{ type, refId, amount, meta? }
	- RewardPackage：{ id:string, entries:RewardEntry[], tags?:string[], version:int }
	- GrantContext：{ playerId, source, reason, traceId, dedupeKey?, region, platform, time }
	- GrantResult：{ status:GrantStatus, entriesApplied:RewardEntry[], entriesSkipped:[{entry, reason}], error?, txId, logs?:string[] }


## 接口功能设计

- IRewardService（示意）：
	- PreviewReward(ctx, rewardId|definition) → ExpandResult
		- 解析掉落表并展开最终明细，执行规则标准化与合并去重，不落库。
	- CanGrant(ctx, rewardId|definition) → { ok:bool, reasons[] }
		- 评估时间窗、每日上限、等级/平台/地区、黑白名单、唯一性等，不改变状态。
	- GrantReward(ctx, rewardId|definition, dedupeKey?) → GrantResult
		- 幂等：同一 dedupeKey/上下文重复调用不重复发放，直接返回首个结果。
- 事件（事件总线）：
	- OnRewardPreviewed, OnRewardGranted, OnRewardFailed, OnRewardQueued。
	- 载荷包含：playerId, txId/traceId, packageId, entries, status, error。
- 错误码规范：
	- 以 ErrorDomain 划分，细化 code 与 message（本地化展示使用 key）。


## 功能流程设计

- 预览（PreviewReward）：
	1) 读取配置（包）。
	2) 合并同类项、标准化数量与单位精度。
	3) 评估容量与替代策略的潜在影响，给出提示但不改变状态。

- 可发放校验（CanGrant）：
	1) 校验时间窗、每日上限、等级/阶段、平台/地区、黑白名单、唯一性。
	2) 速率限制（玩家/来源维度）与冷却检查。
	3) 返回不可发放的原因枚举，不产生副作用。

- 发放（GrantReward）：
	1) 幂等：检查 dedupeKey/RewardInstanceId 是否已完成，如存在则直接返回历史结果。
	6) 派发事件：OnRewardGranted（成功）/OnRewardFailed（失败）。


## 事件节点设计

- OnRewardPreviewed：
	- 触发：调用 PreviewReward 后。
	- 载荷：playerId, traceId, packageId, expandedEntries, timestamp。
	- 订阅：展示层、埋点。

- OnRewardGranted：
	- 触发：GrantReward 成功提交事务后。
	- 载荷：playerId, txId, traceId, packageId, entriesApplied, tags, version。
	- 订阅：运营、成就、任务推进、弹窗。

- OnRewardFailed：
	- 触发：GrantReward 失败（含入补偿队列前）。
	- 载荷：playerId, traceId, packageId, errorDomain, errorCode, message。
	- 订阅：告警、监控、GM 工具。

- OnRewardQueued：
	- 触发：失败进入 PendingGrant 队列时。
	- 载荷：playerId, queueId, nextAttemptAt, attempt, reason。
	- 订阅：后台任务、客服工具。


## 第三方模块建议（放置于 game.thirdparty.module/）

- JSON：Newtonsoft.Json 或等价库（配置序列化/反序列化）。
- 二进制缓存：MessagePack/ProtoBuf（可选）。
- 异步：UniTask 或等价库（可选）。
