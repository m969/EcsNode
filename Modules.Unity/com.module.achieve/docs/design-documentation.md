# （达成系统）模块功能设计文档

## 1. 模块功能概述
达成系统用于设定可完成的目标与条件，引导玩家体验游戏内容，并在目标达成时更新状态与进度。本版本不包含任何奖励流程，奖励由上层模块订阅事件自行处理。

## 2. 配置接口设计

- IAchieveItemConfig：达成项配置
  - Id：唯一标识
  - Key：辅助名称标识
  - Name：名称
  - Description：描述
  - Type：类型（一次性/可重复）
  - Priority：优先级

- IAchieveConditionConfig：达成条件配置
  - Id：唯一标识
  - Key：辅助名称标识
  - ConditionType：条件类型（数值型等）
  - TargetValue：目标值
  - Parameters：参数

（移除：IAchieveRewardConfig 及一切奖励相关配置）

## 3. 实体功能设计

- AchieveItem（达成项实体）
  - Name
  - Description
  - Type
  - Status（未开始/进行中/已完成）
  - CreateTime
  - CompleteTime
  - Priority

- AchieveCondition（达成条件实体）
  - ConditionType
  - TargetValue
  - CurrentValue
  - Parameters
  - IsSatisfied

## 4. 组件功能设计

- AchieveConditionListComponent
  - ConditionList：条件集合
  - IsAllSatisfied：是否全部满足
  - CurrentProgress：当前进度
  - TotalProgress：总进度
  - ProgressPercentage：进度百分比

（移除：AchieveItemRewardComponent 及奖励状态等字段）

## 5. 系统设计

- AchieveItemSystem
  - 创建与初始化达成项
  - 状态管理与更新
  - 完成判定

- AchieveConditionSystem
  - 条件创建与进度更新
  - 满足判定

- AchieveConditionListSystem
  - 条件集合管理与进度计算

（移除：AchieveItemRewardSystem 及奖励发放/状态更新等）

## 6. 事件接口补充（便于外部扩展）
- IOnAchieveCompleted：
  - void OnAchieveCompleted(AchieveItem item)
  外部可订阅该事件以触发奖励或其他业务逻辑。

## 7. 类型补充
- AchieveType：OneTime / Repeatable
- AchieveStatus：NotStarted / InProgress / Completed

## 8. 设计说明
- ECS分层：实体/组件仅承载数据，系统仅承载方法逻辑。
- 系统方法应为静态方法；仅传入实体与必要参数；组件在方法内获取。
- 保持与EcsNode核心库一致的生命周期接口规范。

## 9. 命名空间
- 本模块命名空间统一为：`ECSGame.AchieveModule`（版本 1.0.3 约定）。
- 常用引用示例：

```csharp
using ECS;
using ECSGame.AchieveModule;
using System;
using System.Collections;
using System.Collections.Generic;
```

