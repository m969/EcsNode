# （达成系统）模块程序设计文档

## 1. 程序功能概述

达成系统模块用于设定和追踪玩家目标，判定完成并维护状态与进度；不包含奖励逻辑。奖励应通过订阅事件在上层模块实现。

## 2. 数据结构设计

### 配置接口

```csharp
/// <summary>
/// 达成项配置接口
/// </summary>
public interface IAchieveItemConfig
{
    /// <summary>唯一标识</summary>
    int Id { get; }
    /// <summary>辅助名称标识</summary>
    string Key { get; }
    /// <summary>名称</summary>
    string Name { get; }
    /// <summary>描述</summary>
    string Description { get; }
    /// <summary>类型（如一次性/重复性）</summary>
    AchieveType Type { get; }
    /// <summary>优先级</summary>
    int Priority { get; }
}
```

```csharp
/// <summary>
/// 达成条件配置接口
/// </summary>
public interface IAchieveConditionConfig
{
    /// <summary>唯一标识</summary>
    int Id { get; }
    /// <summary>辅助名称标识</summary>
    string Key { get; }
    /// <summary>条件类型（如数值型等）</summary>
    string ConditionType { get; }
    /// <summary>目标值</summary>
    int TargetValue { get; }
    /// <summary>参数集合</summary>
    Dictionary<string, object> Parameters { get; }
}
```

### 实体设计（仅属性，无方法）

```csharp
/// <summary>
/// 达成项实体
/// </summary>
public class AchieveItem : EcsEntity
{
    /// <summary>名称</summary>
    public string Name { get; set; }
    /// <summary>描述</summary>
    public string Description { get; set; }
    /// <summary>类型（如一次性/重复性）</summary>
    public AchieveType Type { get; set; }
    /// <summary>状态（未开始/进行中/已完成）</summary>
    public AchieveStatus Status { get; set; }
    /// <summary>创建时间</summary>
    public long CreateTime { get; set; }
    /// <summary>完成时间</summary>
    public long CompleteTime { get; set; }
    /// <summary>优先级</summary>
    public int Priority { get; set; }
}
```

```csharp
/// <summary>
/// 达成条件实体
/// </summary>
public class AchieveCondition : EcsEntity
{
    /// <summary>条件类型</summary>
    public string ConditionType { get; set; }
    /// <summary>目标值</summary>
    public int TargetValue { get; set; }
    /// <summary>当前进度值</summary>
    public int CurrentValue { get; set; }
    /// <summary>参数集合</summary>
    public Dictionary<string, object> Parameters { get; set; }
    /// <summary>是否已满足</summary>
    public bool IsSatisfied { get; set; }
}
```

### 组件设计

```csharp
/// <summary>
/// 达成条件组件
/// </summary>
public class AchieveConditionListComponent : EcsComponent
{
    /// <summary>条件集合</summary>
    public List<AchieveCondition> ConditionList { get; set; }
    /// <summary>是否全部满足</summary>
    public bool IsAllSatisfied { get; set; }
    /// <summary>当前进度</summary>
    public int CurrentProgress { get; set; }
    /// <summary>总进度</summary>
    public int TotalProgress { get; set; }
    /// <summary>进度百分比</summary>
    public float ProgressPercentage { get; set; }
}
```

### 类型补充

```csharp
/// <summary>
/// 达成类型
/// </summary>
public enum AchieveType
{
    /// <summary>一次性达成</summary>
    OneTime,
    /// <summary>可重复达成</summary>
    Repeatable
}

/// <summary>
/// 达成状态
/// </summary>
public enum AchieveStatus
{
    /// <summary>未开始</summary>
    NotStarted,
    /// <summary>进行中</summary>
    InProgress,
    /// <summary>已完成</summary>
    Completed
}
```

## 3. 系统一览

- AchieveItemSystem : `AEntitySystem<AchieveItem>`
  - `IAwake<AchieveItem>`
  - `IInit<AchieveItem>`
  - `IAfterInit<AchieveItem>`
  - `IEnable<AchieveItem>`
  - `IDisable<AchieveItem>`
  - `IUpdate<AchieveItem>`
  - `IDestroy<AchieveItem>`
  - `static AchieveItem Create(EcsEntity parent, IAchieveItemConfig config)`
  - `static void Init(AchieveItem entity, IAchieveItemConfig config)`
  - `static void UpdateStatus(AchieveItem entity, AchieveStatus status)`
  - `static bool IsCompleted(AchieveItem entity)`

- AchieveConditionSystem : `AEntitySystem<AchieveCondition>`
  - `IAwake<AchieveCondition>`
  - `IInit<AchieveCondition>`
  - `IAfterInit<AchieveCondition>`
  - `IEnable<AchieveCondition>`
  - `IDisable<AchieveCondition>`
  - `IUpdate<AchieveCondition>`
  - `IDestroy<AchieveCondition>`
  - `static AchieveCondition Create(EcsEntity parent, IAchieveConditionConfig config)`
  - `static void UpdateProgress(AchieveCondition entity, int value)`
  - `static bool IsSatisfied(AchieveCondition entity)`

- AchieveConditionListSystem : `AComponentSystem<AchieveItem, AchieveConditionListComponent>`
  - `IAwake<AchieveItem, AchieveConditionListComponent>`
  - `IInit<AchieveItem, AchieveConditionListComponent>`
  - `IAfterInit<AchieveItem, AchieveConditionListComponent>`
  - `IEnable<AchieveItem, AchieveConditionListComponent>`
  - `IDisable<AchieveItem, AchieveConditionListComponent>`
  - `IDestroy<AchieveItem, AchieveConditionListComponent>`
  - `static void InitConditions(AchieveItem entity, List<IAchieveConditionConfig> conditionConfigs)`
  - `static void CalculateProgress(AchieveConditionListComponent component)`
  - `static bool IsAllSatisfied(AchieveConditionListComponent component)`

## 4. 事件接口（对外扩展）

- interface `IOnAchieveCompleted` : `IDispatch`
  - `void OnAchieveCompleted(EcsEntity entity, AchieveItem item)`
  用于外部订阅并执行业务（如奖励、引导等）。

## 5. 设计规范与约束

- 系统仅实现方法逻辑，实体/组件仅承载属性数据。
- 系统方法为静态；仅传实体与必要参数，组件在方法内获取。
- 保持命名空间为 `ECSGame.AchieveModule`；遵循EcsNode生命周期接口规范。