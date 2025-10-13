# （普攻）模块程序设计文档

命名空间：ECSGame.AttackModule

常用引用：
- using ECS;
- using System;
- using System.Collections;
- using System.Collections.Generic;

说明：基于EcsNode（ECS）架构，实体仅承载基础数据，复杂可插拔能力放入组件；同类型组件在单一实体内唯一；所有实体仅能通过父实体的 AddChild<T>() 创建。

本设计覆盖以下需求：
- 统一普攻触发接口（输入校验与布尔/错误码反馈）
- 攻击分段与时序（准备/判定/收招，可配置时长，状态暴露）
- 命中判定（单体目标）
- 命中事件回调（含攻击者、受击者、最终伤害值）
- 伤害计算（组合基础伤害与影响因子，输出确定性数值或失败原因）
- 事件钩子（OnAttackStart/OnAttackHit/OnAttackCancel/OnAttackEnd）


## 配置接口设计

- IAttackConfig（普攻配置）
	- 用途：定义一次普攻的静态参数。
	- 字段设计：
		- int Id：配置Id。
		- int BaseDamage：基础伤害值（整数）。
		- int WindupDurationMs：准备阶段持续毫秒数（>=0）。
		- int ActiveDurationMs：判定阶段持续毫秒数（>0）。
		- int RecoveryDurationMs：收招阶段持续毫秒数（>=0）。
		- DamageFormulaType DamageFormula：伤害公式类型（决定影响因子组合方式）。


## 其他类型补充

- 流程节点派发接口补充（均继承 IDispatch）
	- IOnAttackStart：void OnAttackStart(EcsEntity owner, AttackAction action)
	- IOnAttackHit：void OnAttackHit(EcsEntity owner, AttackAction action, long attackerId, long targetId, int finalDamage)
	- IOnAttackCancel：void OnAttackCancel(EcsEntity owner, AttackAction action, AttackCancelReason reason)
	- IOnAttackEnd：void OnAttackEnd(EcsEntity owner, AttackAction action)

	说明：上述接口通过 entity.Dispatch<TSystem>((sys)=> ...) 进行分发，供外部模块（如Buff、数值、表现层）订阅扩展。

- 基础数据类型补充
	- struct AttackContext：
		- long AttackerId
		- long TargetId
		- int ConfigId
		- long StartTimeMs
	- struct AttackComputeResult：
		- bool Success
		- int FinalDamage（Success == true 时有效）
		- DamageFailReason FailReason（Success == false 时有效）

- 枚举补充
	- enum AttackPhase：None=0, Windup=1, Active=2, Recovery=3, Ended=4, Canceled=5
	- enum AttackCancelReason：None=0, Interrupted=1, Manual=2, InvalidTarget=3
	- enum AttackFailureReason：None=0, InvalidAttacker=1, InvalidTarget=2, ConfigNotFound=3, TimeInvalid=4, AlreadyAttacking=5, InternalError=99
	- enum DamageFailReason：None=0, AttributeMissing=1, Overflow=2, FormulaUnsupported=3, InternalError=99
	- enum DamageFormulaType：Flat=0, AttackMinusDefense=1, AttackTimesRatio=2


## 实体设计

- AttackAction（一次普攻的运行时实例）
	- 用途：描述并驱动一次从启动到结束的普攻过程。
	- 字段设计（仅基础数据，复杂能力入组件）：
		- long AttackerId
		- long TargetId
		- int ConfigId
		- long StartTimeMs（发起时的时间戳）
		- AttackPhase Phase
		- long PhaseStartTimeMs（当前阶段开始时间）
		- bool HasHit（是否已在Active阶段内完成命中）
		- AttackCancelReason CancelReason（若被取消）


### 实体系统设计（以 System 为后缀）

- AttackRuntimeSystem（入口与调度）
	- 接口：
		- bool TryStartAttack(EcsEntity runtime, long attackerId, long targetId, long nowMs, int configId, out AttackFailureReason fail, out long newActionId)
			- 功能：统一触发接口。校验输入、装配 AttackAction 与组件、派发 OnAttackStart。
		- bool TryCancelAttack(EcsEntity runtime, long actionId, AttackCancelReason reason)
			- 功能：取消指定攻击并派发 OnAttackCancel。
		- void Tick(EcsEntity runtime, long nowMs)
			- 功能：遍历管理下的 AttackAction 执行阶段推进与命中处理；收尾移除。

- AttackActionSystem（单次普攻驱动）
	- 接口：
		- void Tick(AttackAction action, long nowMs)
			- 功能：依据 AttackTimelineComponent 推进一步进度：
				- Windup 超时 -> 进入 Active，仍保留命中标记未触发。
				- Active 内若未命中 -> 立即执行命中判定与伤害计算 -> OnAttackHit；Active 超时 -> Recovery。
				- Recovery 超时 -> Ended -> OnAttackEnd。
		- bool TryCancel(AttackAction action, AttackCancelReason reason)
			- 功能：设置 Canceled 状态并派发 OnAttackCancel。


### 实体列表组件

- AttackListComponent（挂于角色实体）
	- 用途：存储与管理所有 AttackAction。
	- 字段：
		- Dictionary<long, AttackAction> Id2Entities
		- Dictionary<int, List<AttackAction>> ConfigId2Entities（可选：按配置分组检索）
		- Dictionary<long, long> AttackerId2LatestAttackId（可选：限制并发、查询最近攻击）


### 实体列表组件系统设计（命名省略 Component 并以 System 为后缀）

- AttackListSystem
	- 接口：
		- AttackAction Create(EcsEntity runtime, in AttackContext ctx, IAttackConfig cfg)
		- bool Remove(EcsEntity runtime, long actionId)
		- AttackAction Get(EcsEntity runtime, long actionId)
		- IReadOnlyList<AttackAction> GetByConfig(EcsEntity runtime, int configId)
		- void ForEach(EcsEntity runtime, Action<AttackAction> visitor)


## 组件设计

- AttackTimelineComponent（时间轴/阶段推进能力）
	- 用途：驱动阶段切换与命中单次性控制。
	- 字段：
		- int WindupDurationMs
		- int ActiveDurationMs
		- int RecoveryDurationMs
		- long CreatedTimeMs
		- long CurrentPhaseStartMs
		- AttackPhase CurrentPhase
		- bool HitEmitted（命中事件是否已发出）

- AttackDamageComponent（伤害计算相关数据）
	- 用途：承载伤害基础值与计算所需上下文，确保可插拔扩展。
	- 字段：
		- int BaseDamage
		- DamageFormulaType Formula


### 组件系统设计（命名省略 Component 并以 System 为后缀）

- AttackTimelineSystem
	- 接口：
		- void InitFromConfig(AttackAction action, IAttackConfig cfg, long nowMs)
		- bool EnterNextPhase(AttackAction action, long nowMs)
		- void TickAndTransit(AttackAction action, long nowMs)

- AttackDamageSystem
	- 接口：
		- bool TryCalculateDamage(AttackAction action, out int finalDamage, out DamageFailReason fail)
			- 约束：
				- 纯函数行为：仅依赖 AttackAction 及其组件与只读数据，保证相同输入得到确定性输出。
				- 失败时返回 false 并填充 fail，避免异常主流程中断。


## 时序与流程（概述）

1) 触发：
	 - runtime.Dispatch<IAttackRuntimeSystem>(sys => sys.TryStartAttack(...))。
		- 校验成功后，创建 AttackAction：
		 - 初始化 AttackTimelineComponent（CurrentPhase=Windup）。
		 - 初始化 AttackDamageComponent（BaseDamage/Formula 等来自 IAttackConfig）。
		 - 派发 IOnAttackStart。

2) 驱动：
	 - 外部每帧或定期调用 runtime.Tick(nowMs)。
		- AttackActionSystem.Tick 内部依据时间推进：
		 - Windup -> Active（到达阈值）。
		 - 进入 Active 后：若未命中，立即对 TargetId 执行单体命中：
			 - 调用 AttackDamageSystem.TryCalculateDamage 得到 finalDamage。
			- 派发 IOnAttackHit（包含 attackerId/targetId/finalDamage）。
		 - Active -> Recovery（到达阈值）。
		 - Recovery -> Ended：派发 IOnAttackEnd，并从 AttackListComponent 移除。

3) 取消/打断：
		- runtime 或外部系统可调用 TryCancelAttack / TryCancel。
	 - 设置状态为 Canceled 并派发 IOnAttackCancel；移除实体。


## 输入校验与异常反馈（覆盖需求16）

- TryStartAttack 校验点：
	- attackerId/targetId 必须有效可见（可通过外部实体查询组件验证，不在本模块内实现，失败反馈 InvalidAttacker/InvalidTarget）。
	- configId 对应 IAttackConfig 必须存在（ConfigNotFound）。
	- 时间戳必须非负且单调（TimeInvalid）。
	- 可选：限制并发（AttackerId2LatestAttackId 存在未结束攻击则 AlreadyAttacking）。
	- 失败返回 false 并输出 AttackFailureReason；成功返回 true。

- TryCalculateDamage 失败场景：
	- 必要属性缺失（AttributeMissing）。
	- 公式不支持（FormulaUnsupported）。
	- 数值溢出/下溢（Overflow）。


## 示例派发片段（说明性）

```csharp
// 触发一次普攻
 runtime.Dispatch<IAttackRuntimeSystem>(sys =>
{
		var ok = sys.TryStartAttack(runtime, attackerId, targetId, nowMs, configId, out var fail, out var actionId);
		// 根据 ok/fail 做后续逻辑
});

// 命中事件监听（例如在其他模块中）
entity.Dispatch<IOnAttackHit>(sys => sys.OnAttackHit(entity, action, attackerId, targetId, finalDamage));
```


## 备注

- 单体目标命中策略固定：Active 阶段内首次触发一次命中事件；不进行范围/多段命中。
- 伤害计算保持可插拔：通过 AttackDamageSystem 与 DamageFormulaType 承载扩展；本模块不内置具体数值来源（如角色属性），由外部模块提供或在 TryCalculateDamage 内部读取标准化能力接口。
- 开放扩展点：
	- 事件钩子（IOnAttackStart/Hit/Cancel/End）。
	- 可新增自定义公式类型；不影响现有接口。

