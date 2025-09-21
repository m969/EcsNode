# （任务）模块需求文档

## 依赖的第三方模块
- Achieve（达成）模块
    - AchieveItem：存放类型、条件与状态，作为任务进度的唯一真实来源

## 模块概述
- 在 Achieve（达成）模块之上实现任务系统：以 AchieveItem 作为进度与状态的唯一真实来源，Task 负责编排（激活、调度、结算、重置）。
- 任务是对“达成项”的一种包装与编排，避免重复实现进度判定逻辑，所有条件判断复用 AchieveItem。

## 模块功能（基础功能列表）
- 任务定义与配置
    - 基本信息：id、名称、描述
    - 绑定 AchieveItemId（唯一进度源）
- 生命周期管理
    - 激活方式：手动激活
    - 状态流转：未激活 → 进行中 → 可领取 → 已领取
- 结算与奖励
    - 完成后进入“可领取”
    - 奖励定义与发放
- 查询与接口
    - 查询：任务列表/详情/进度
    - 命令：激活、领取
    - 事件：TaskActivated/TaskProgressChanged/TaskCompleted/TaskRewardClaimed/TaskReset
