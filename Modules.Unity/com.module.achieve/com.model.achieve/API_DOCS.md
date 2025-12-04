# 数据模型层 API 文档

## 实体 (Entities)

### AchieveItem
达成项实体，继承自 `EcsEntity`。

| 属性 | 类型 | 描述 |
| :--- | :--- | :--- |
| Name | `string` | 名称 |
| Description | `string` | 描述 |
| Type | `AchieveType` | 类型（如一次性/重复性） |
| Status | `AchieveStatus` | 状态（未开始/进行中/已完成） |
| CreateTime | `long` | 创建时间 |
| CompleteTime | `long` | 完成时间 |
| Priority | `int` | 优先级 |

## 组件 (Components)

### AchieveConditionListComponent
达成条件组件（列表），继承自 `EcsComponent`。

| 属性 | 类型 | 描述 |
| :--- | :--- | :--- |
| ConditionList | `List<AchieveCondition>` | 条件集合 |
| IsAllSatisfied | `bool` | 是否全部满足 |
| CurrentProgress | `int` | 当前进度 |
| TotalProgress | `int` | 总进度 |
| ProgressPercentage | `float` | 进度百分比 |

### AchieveItemListComponent
达成项列表组件，继承自 `EcsComponent`。

| 属性 | 类型 | 描述 |
| :--- | :--- | :--- |
| Items | `List<AchieveItem>` | 达成项列表 |

## 数据对象 (Data Objects)

### AchieveCondition
达成条件数据对象。

| 属性 | 类型 | 描述 |
| :--- | :--- | :--- |
| ConditionType | `string` | 条件类型 |
| TargetValue | `int` | 目标值 |
| CurrentValue | `int` | 当前进度值 |
| Parameters | `Dictionary<string, object>` | 参数集合 |
| IsSatisfied | `bool` | 是否已满足 |

## 接口 (Interfaces)

### IAchieveItemConfig
达成项配置接口，用于配置达成项的基础信息。

| 属性 | 类型 | 描述 |
| :--- | :--- | :--- |
| Id | `int` | 唯一标识 |
| Key | `string` | 辅助名称标识 |
| Name | `string` | 名称 |
| Description | `string` | 描述 |
| Type | `AchieveType` | 类型（如一次性/重复性） |
| Priority | `int` | 优先级 |

### IAchieveConditionConfig
达成条件配置接口。

| 属性 | 类型 | 描述 |
| :--- | :--- | :--- |
| Id | `int` | 唯一标识 |
| Key | `string` | 辅助名称标识 |
| ConditionType | `string` | 条件类型（如数值型等） |
| TargetValue | `int` | 目标值 |
| Parameters | `Dictionary<string, object>` | 参数集合 |

## 枚举 (Enums)

### AchieveType
达成类型枚举。

| 枚举值 | 描述 |
| :--- | :--- |
| OneTime | 一次性达成 |
| Repeatable | 可重复达成 |

### AchieveStatus
达成状态枚举。

| 枚举值 | 描述 |
| :--- | :--- |
| NotStarted | 未开始 |
| InProgress | 进行中 |
| Completed | 已完成 |
