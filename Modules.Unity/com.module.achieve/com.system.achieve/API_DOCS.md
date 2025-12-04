# 系统逻辑层 API 文档

## 系统 (Systems)

### AchieveItemSystem
达成项系统，继承自 `AEntitySystem<AchieveItem>`。

#### 方法

| 方法名 | 参数 | 返回值 | 描述 |
| :--- | :--- | :--- | :--- |
| Create | `EcsEntity parent`, `IAchieveItemConfig config` | `AchieveItem` | 创建达成项实体 |
| Init | `AchieveItem entity`, `IAchieveItemConfig config` | `void` | 初始化达成项实体 |
| UpdateStatus | `AchieveItem entity`, `AchieveStatus status` | `void` | 更新达成项状态 |
| IsCompleted | `AchieveItem entity` | `bool` | 判定达成项是否完成 |

### AchieveConditionListSystem
达成条件列表系统，继承自 `AComponentSystem<AchieveItem, AchieveConditionListComponent>`。

#### 方法

| 方法名 | 参数 | 返回值 | 描述 |
| :--- | :--- | :--- | :--- |
| InitConditions | `AchieveItem entity`, `List<IAchieveConditionConfig> conditionConfigs` | `void` | 初始化达成项条件组件 |
| UpdateConditionProgress | `AchieveItem entity`, `int conditionIndex`, `int delta` | `void` | 更新特定条件进度 |
| CalculateProgress | `AchieveConditionListComponent component` | `void` | 计算进度 |

### AchieveItemListSystem
达成项列表系统，继承自 `AComponentSystem<EcsEntity, AchieveItemListComponent>`。

#### 方法

| 方法名 | 参数 | 返回值 | 描述 |
| :--- | :--- | :--- | :--- |
| AddItem | `EcsEntity entity`, `AchieveItem item` | `void` | 添加达成项 |
| RemoveItem | `EcsEntity entity`, `AchieveItem item` | `void` | 移除达成项 |
| GetItems | `EcsEntity entity` | `List<AchieveItem>` | 获取所有达成项 |

## 事件 (Events)

### IAchieveCompletedHandler
达成完成事件（对外订阅），继承自 `IDispatch`。

#### 方法

| 方法名 | 参数 | 描述 |
| :--- | :--- | :--- |
| OnAchieveCompletedHandle | `EcsEntity entity`, `AchieveItem item` | 达成完成回调 |
