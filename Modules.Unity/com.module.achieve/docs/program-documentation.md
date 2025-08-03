# （达成系统）模块程序设计文档

## 1. 程序功能概述

达成系统模块用于设定和追踪玩家目标，判定完成并发放奖励，提升游戏体验。采用ECS架构，便于扩展和维护。

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

```csharp
/// <summary>
/// 达成奖励配置接口
/// </summary>
public interface IAchieveRewardConfig
{
    /// <summary>唯一标识</summary>
    int Id { get; }
    /// <summary>辅助名称标识</summary>
    string Key { get; }
    /// <summary>奖励类型（虚拟货币/道具）</summary>
    RewardType RewardType { get; }
    /// <summary>物品ID</summary>
    int ItemId { get; }
    /// <summary>奖励数量</summary>
    int Amount { get; }
}
```

### 实体设计

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
public class AchieveItemConditionComponent : EcsComponent
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

```csharp
/// <summary>
/// 达成奖励组件
/// </summary>
public class AchieveItemRewardComponent : EcsComponent
{
    /// <summary>奖励列表</summary>
    public List<IAchieveRewardConfig> RewardList { get; set; }
    /// <summary>奖励状态（未发放/已发放）</summary>
    public RewardStatus RewardStatus { get; set; }
    /// <summary>领取时间</summary>
    public long ClaimTime { get; set; }
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

/// <summary>
/// 奖励类型
/// </summary>
public enum RewardType
{
    /// <summary>虚拟货币</summary>
    Currency,
    /// <summary>道具</summary>
    Item
}

/// <summary>
/// 奖励发放状态
/// </summary>
public enum RewardStatus
{
    /// <summary>未发放</summary>
    NotGranted,
    /// <summary>已发放</summary>
    Granted
}
```

## 3. 系统业务设计


### AchieveItemSystem

- 创建与初始化达成项
- 状态管理与更新
- 完成判定

#### 生命周期接口实现要求

系统类需继承 `AEntitySystem<AchieveItem>`，并根据业务需要实现以下生命周期接口：

- `IAwake<AchieveItem>`
- `IInit<AchieveItem>`
- `IAfterInit<AchieveItem>`
- `IEnable<AchieveItem>`
- `IDisable<AchieveItem>`
- `IUpdate<AchieveItem>`
- `IDestroy<AchieveItem>`

#### 静态接口方法设计

```csharp
/// <summary>
/// 创建达成项实体
/// </summary>
/// <param name="parent">父实体</param>
/// <param name="config">达成项配置</param>
/// <returns>达成项实体</returns>
public static AchieveItem Create(EcsEntity parent, IAchieveItemConfig config);

/// <summary>
/// 初始化达成项实体
/// </summary>
/// <param name="entity">达成项实体</param>
/// <param name="config">达成项配置</param>
public static void Init(AchieveItem entity, IAchieveItemConfig config);

/// <summary>
/// 更新达成项状态
/// </summary>
/// <param name="entity">达成项实体</param>
/// <param name="status">目标状态</param>
public static void UpdateStatus(AchieveItem entity, AchieveStatus status);

/// <summary>
/// 判定达成项是否完成
/// </summary>
/// <param name="entity">达成项实体</param>
/// <returns>是否完成</returns>
public static bool IsCompleted(AchieveItem entity);
```


### AchieveConditionSystem

- 条件创建与进度更新
- 满足判定

#### 生命周期接口实现要求

系统类需继承 `AEntitySystem<AchieveCondition>`，并根据业务需要实现以下生命周期接口：

- `IAwake<AchieveCondition>`
- `IInit<AchieveCondition>`
- `IAfterInit<AchieveCondition>`
- `IEnable<AchieveCondition>`
- `IDisable<AchieveCondition>`
- `IUpdate<AchieveCondition>`
- `IDestroy<AchieveCondition>`

#### 静态接口方法设计

```csharp
/// <summary>
/// 创建达成条件实体
/// </summary>
/// <param name="parent">父实体</param>
/// <param name="config">条件配置</param>
/// <returns>达成条件实体</returns>
public static AchieveCondition Create(EcsEntity parent, IAchieveConditionConfig config);

/// <summary>
/// 更新达成条件进度
/// </summary>
/// <param name="entity">达成条件实体</param>
/// <param name="value">增加的进度值</param>
public static void UpdateProgress(AchieveCondition entity, int value);

/// <summary>
/// 判定条件是否满足
/// </summary>
/// <param name="entity">达成条件实体</param>
/// <returns>是否满足</returns>
public static bool IsSatisfied(AchieveCondition entity);
```


### AchieveItemConditionSystem

- 条件集合管理与进度计算

#### 生命周期接口实现要求

系统类需继承 `AComponentSystem<AchieveItem, AchieveItemConditionComponent>`，并根据业务需要实现以下生命周期接口：

- `IAwake<AchieveItem, AchieveItemConditionComponent>`
- `IInit<AchieveItem, AchieveItemConditionComponent>`
- `IAfterInit<AchieveItem, AchieveItemConditionComponent>`
- `IEnable<AchieveItem, AchieveItemConditionComponent>`
- `IDisable<AchieveItem, AchieveItemConditionComponent>`
- `IDestroy<AchieveItem, AchieveItemConditionComponent>`

#### 静态接口方法设计

```csharp
/// <summary>
/// 初始化达成项条件组件
/// </summary>
/// <param name="entity">达成项实体</param>
/// <param name="conditionConfigs">条件配置列表</param>
public static void InitConditions(AchieveItem entity, List<IAchieveConditionConfig> conditionConfigs);

/// <summary>
/// 计算达成项条件进度
/// </summary>
/// <param name="component">达成项条件组件</param>
public static void CalculateProgress(AchieveItemConditionComponent component);

/// <summary>
/// 判定所有条件是否全部满足
/// </summary>
/// <param name="component">达成项条件组件</param>
/// <returns>是否全部满足</returns>
public static bool IsAllSatisfied(AchieveItemConditionComponent component);
```


### AchieveItemRewardSystem

- 奖励发放与状态更新

#### 生命周期接口实现要求

系统类需继承 `AComponentSystem<AchieveItem, AchieveItemRewardComponent>`，并根据业务需要实现以下生命周期接口：

- `IAwake<AchieveItem, AchieveItemRewardComponent>`
- `IInit<AchieveItem, AchieveItemRewardComponent>`
- `IAfterInit<AchieveItem, AchieveItemRewardComponent>`
- `IEnable<AchieveItem, AchieveItemRewardComponent>`
- `IDisable<AchieveItem, AchieveItemRewardComponent>`
- `IDestroy<AchieveItem, AchieveItemRewardComponent>`

#### 静态接口方法设计

```csharp
/// <summary>
/// 初始化达成项奖励组件
/// </summary>
/// <param name="entity">达成项实体</param>
/// <param name="rewardConfigs">奖励配置列表</param>
public static void InitRewards(AchieveItem entity, List<IAchieveRewardConfig> rewardConfigs);

/// <summary>
/// 发放奖励
/// </summary>
/// <param name="component">达成项奖励组件</param>
/// <param name="player">玩家实体</param>
public static void GrantRewards(AchieveItemRewardComponent component, EcsEntity player);

/// <summary>
/// 更新奖励状态
/// </summary>
/// <param name="component">达成项奖励组件</param>
/// <param name="status">奖励状态</param>
public static void UpdateRewardStatus(AchieveItemRewardComponent component, RewardStatus status);
```