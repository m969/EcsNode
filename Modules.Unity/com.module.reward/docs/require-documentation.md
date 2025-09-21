# （奖励）模块需求文档

## 依赖的第三方模块（优先复用现有模块/实体，第三方放置于 `game.thirdparty.module/`）

- 必备对接（复用现有通用模块，如有）：
	- 玩家档案/身份模块：读取玩家ID、区服、平台、等级等上下文。
	- 经济/货币模块：增减货币、兑现兑换比率、精度与上限控制。
	- 物品与背包模块：道具定义、堆叠规则、容量检测与溢出处理。
	- 事件总线/消息模块：发布奖励发放前后事件，支持订阅扩展。

- 可选第三方库（如采用，存放于 `docs/game.thirdparty.module/` 并在文档登记）：
	- Newtonsoft.Json 或同类：奖励配置的序列化/反序列化。
	- MessagePack/ProtoBuf：高性能本地缓存与持久化（可选）。
	- UniTask（或等价异步库）：异步发放与批处理。
	- DOTween/Addressables：仅用于客户端表现（奖励弹窗/资源加载，若本模块覆盖到展示层）。

说明：本仓当前 `docs/game.thirdparty.module/` 为空；以上为建议清单，具体以工程集成时最终选型为准。

## 模块需求列表

1) 奖励定义与配置（Must）
- 奖励类型：Currency/Item/Exp/Energy/Custom。
- 组合奖励：一个“奖励包”可包含多条奖励项，支持条件化条目（如首充/地区/平台）。
- 配置来源：支持文本配置（JSON/YAML）或资源化（ScriptableObject），含校验器。
- 数值与单位：统一精度与上限；支持小数或大数时的安全处理策略。
- 本地化：名称/描述可带本地化Key（前端展示使用）。

2) 发放流程与规则（Must）
- 触发来源：任务完成、关卡通关、日常签到、成就达成、活动、邮件/补偿、兑换码、GM面板。
- 条件判定：时间窗、每日上限、玩家等级/阶段、平台/地区、黑白名单、唯一性（一次性奖励）。

4) 接口与事件（Must）
- IRewardService（示意）：
	- PreviewReward(ctx, rewardId|definition) -> 计算实际将发放的明细（含掉落展开）。
	- CanGrant(ctx, rewardId|definition) -> Bool 与不可发放原因（上限/时间窗/条件不满足）。
	- GrantReward(ctx, rewardId|definition, dedupeKey?) -> 发放结果（成功、部分、失败）。
	- GrantBatch(ctx, list<rewardSpec>) -> 批量原子或分组原子策略可配置。
	- GetPending(ctx) -> 待领取/补偿队列查询。
- 事件：OnRewardPreviewed/OnRewardGranted/OnRewardFailed/OnRewardQueued，供运营/成就/弹窗订阅。
- 错误码：标准化错误域（配置缺失/不满足条件/容量不足/幂等冲突/后端错误）。

5) 数据模型（Must）
- RewardId（string）、RewardEntry{type, refId, amount, meta?}、RewardPackage{entries[], tags[], version}。
- GrantContext{playerId, source, reason, traceId, dedupeKey, region, platform, time}。
- GrantResult{status, entriesApplied[], entriesSkipped[], error?, txId, logs[]}。
