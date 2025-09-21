# （单位派遣）模块程序设计文档

本文件基于功能设计文档（design-documentation.md）撰写，目的是把设计约束、数据结构与系统业务接口以可实现的程序视角明确下来，方便开发者按约定实现模块。

## 目标
- 说明本模块的核心职责、对外接口与实现契约。
- 给出实体、组件、系统以及派发接口的详细设计，供实现与单元测试使用。

## 适用范围与约束
- 模块名：单位派遣（unit-dispatch）。
- 所有代码命名空间以 `ECSGame.UnitDispatchModule` 为根（遵循 `ECSGame.**Module` 规则，模块名驼峰首字母大写，去除横杆下划线）。
- 基于 EcsNode 框架实体/组件/系统规范实现：实体继承 `EcsEntity`，组件继承 `EcsComponent`，系统继承 `AEntitySystem<T>` 或 `AComponentSystem<T, C>`。

## 合同（Contract）
简短描述输入/输出与错误模式：
- 输入：其他系统或上层逻辑通过实体/事件调用本模块的系统业务静态方法，或通过实体 `AddComponent<T>()` / `AddChild<T>()` 创建实体/组件。
- 输出：对实体状态的修改、组件数据更新、通过 `IDispatch` 派发外部事件。
- 错误模式：参数校验失败（null、无效 Id）、组件缺失、并发重复派发。所有系统接口需以返回值或异常明确定义失败路径。

## 数据结构设计

### 配置接口
- 配置以整型 Id 为唯一标识，字符串 Key 作为辅助名称。
- 示例接口（伪代码）：
	- UnitDispatchConfig
		- int Id
		- string Key
		- int MaxDispatchCount
		- string Summary

> 备注：配置接口定义位于 `game.thirdparty.module` 或模块的 `Config` 子目录，按项目约定加载。

### 实体与组件设计
- 实体继承自 `EcsEntity`，仅包含基础属性，不包含方法逻辑。
 - 在 1.0.2 中，派遣相关数据模型重构为 `DispatchExecution` 概念；在 1.0.3 中统一更名为 `UnitDispatcher`。派遣执行体作为独立执行实体附着在宿主实体上，承载原 `DispatchUnitComponent` 的职责和数据。
 - UnitDispatcher 示例（执行实体）：
 	- UnitDispatcher : EcsEntity
 		- summary: 表示一次派遣执行记录，承载派遣次数、目标、超时等信息。
 		- 属性示例：
 			- int DispatchCount
 			- int ConfigId
 			- long TargetEntityId
 			- float Timeout

### 组件设计
- 组件继承自 `EcsComponent`，只包含数据属性。
- 常用组件：
	- DispatchStateComponent : EcsComponent
		- summary: 存储派遣当前状态、目标、超时信息等。
		- 属性示例：
			- int State（枚举）
			- long TargetEntityId
			- float Timeout

	- DispatchRuleComponent : EcsComponent
		- summary: 存储派遣规则引用与运行时缓存。
		- 属性示例：
			- int ConfigId
			- Dictionary<string, object> RuntimeParams

- UnitDispatcherListComponent（原 DispatchExecutionListComponent）
	- summary: 用于集中管理若干 UnitDispatcher 实体，提供快速按 Id 与按配置分组的查找与遍历能力。此组件只保存数据，不包含业务逻辑。
	- 属性设计示例：
		- Dictionary<long, EcsEntity> Id2Entities
			- summary: 以执行实体 Id 为键，存储 UnitDispatcher 实体引用。
		- Dictionary<int, List<EcsEntity>> ConfigId2Entities
			- summary: 以配置 Id 为键，存储使用该配置的执行实体列表，便于按配置批量查询与清理。
		- object SyncRoot
			- summary: 可选的线程同步对象（文档层建议），实际并发控制依赖于上层框架或实现方。

枚举值请在声明处提供必要 summary。

## 系统业务设计

总体要求：系统类为非静态类，业务方法为静态方法并尽量只操作实体；系统类实现必要生命周期接口并继承框架基类。

### 派遣创建与实体侧系统（示例）
- 系统类（实体系统）：UnitDispatcherSystem（原 DispatchExecutionSystem） : AEntitySystem<UnitDispatcher>
	- 实现生命周期接口：`IAwake<T>`, `IInit<T>`, `IAfterInit<T>`, `IEnable<T>`, `IDisable<T>`, `IDestroy<T>`（实体级生命周期）。
	- 静态业务方法示例：
		- static UnitDispatcher Create(EcsEntity hostEntity, UnitDispatchConfig cfg)
			- 输入：宿主实体、配置
			- 输出：创建的 UnitDispatcher 实体
			- 行为：Create 方法应通过 hostEntity.AddChild<UnitDispatcher>(beforeAwake) 创建独立执行实体，并在 beforeAwake 填充必要字段（如 ConfigId、初始计数）。不要将 UnitDispatcher 作为组件附加到宿主实体；执行体作为独立实体承载派遣生命周期更清晰且便于管理。
		- static void StartDispatch(UnitDispatcher exec, int count)
			- 直接操作执行实体 exec，执行状态变更、计数逻辑。
		- static void CancelDispatch(UnitDispatcher exec)
			- 取消派遣，清理计时/回调，派发外部事件；根据需要移除或销毁执行实体。

### 组件系统（示例）
- 系统类：DispatchStateSystem : AComponentSystem<EcsEntity, DispatchStateComponent>
	- 实现生命周期接口：`IAwake<T,C>`, `IInit<T,C>`, `IAfterInit<T,C>`, `IEnable<T,C>`, `IDisable<T,C>`, `IDestroy<T,C>`。
	- 静态业务方法示例：
		- static void Tick(EcsEntity entity, DispatchStateComponent c, float dt)
			- 用于组件驱动的周期性处理（如超时检查）。
		- static bool CanDispatch(EcsEntity entity, DispatchStateComponent c, int count)
			- 校验规则并返回是否允许派遣。

- UnitDispatcherListSystem（原 DispatchExecutionListSystem） : AComponentSystem<EcsEntity, UnitDispatcherListComponent>
	- 实现生命周期接口：`IAwake<T,C>`, `IInit<T,C>`, `IAfterInit<T,C>`, `IEnable<T,C>`, `IDisable<T,C>`, `IDestroy<T,C>`（组件级生命周期），用于在组件创建/销毁时做必要的初始化或清理。
	- 静态业务方法（示例）：


		- /// <summary>创建并返回一个新的 UnitDispatcherListComponent 实例并挂载到实体上（如果尚未存在）。</summary>
		  /// <param name="entity">宿主实体。</param>
		  /// <returns>UnitDispatcherListComponent</returns>
		  public static UnitDispatcherListComponent CreateList(EcsEntity entity)
		- /// <summary>向列表中添加一个 UnitDispatcher 实体（幂等）。</summary>
		  /// <param name="listComponent">目标列表组件。</param>
		  /// <param name="exec">要添加的 UnitDispatcher 实体。</param>
		  /// <returns>是否成功（false 表示参数无效或已存在）。</returns>
		  public static bool AddExecution(EcsEntity listOwner, UnitDispatcherListComponent listComponent, EcsEntity exec)
		- /// <summary>从列表中移除一个 UnitDispatcher 实体（幂等）。</summary>
		  /// <param name="listComponent">目标列表组件。</param>
		  /// <param name="execId">要移除的执行实体 Id。</param>
		  /// <returns>是否成功（false 表示不存在）。</returns>
		  public static bool RemoveExecution(UnitDispatcherListComponent listComponent, long execId)
		- /// <summary>根据 Id 获取执行实体（若不存在返回 null）。</summary>
		  public static EcsEntity GetExecution(UnitDispatcherListComponent listComponent, long execId)
		- /// <summary>按配置 Id 获取执行实体列表（若不存在返回空列表）。</summary>
		  public static IReadOnlyList<EcsEntity> GetExecutionsByConfig(UnitDispatcherListComponent listComponent, int configId)
		- /// <summary>对列表中的每个执行实体执行回调（安全遍历，忽略空引用）。</summary>
		  public static void ForEach(UnitDispatcherListComponent listComponent, Action<EcsEntity> action)

	- 错误与幂等性：
		- 所有方法应对 null 参数进行校验并明确返回失败（或抛出受控异常，视项目约定）。
		- Add/Remove 操作为幂等操作：重复添加不重复插入，重复移除不报错。
		- 对于可能的并发，文档建议实现方在方法内使用 SyncRoot 或依赖上层调度保证线程安全。

### 系统节点派发接口

派发接口继承 `IDispatch`，用于向外部暴露事件回调。为降低实现类的复杂度，接口按事件拆分为单方法接口（每个接口只包含一个回调）。当前约定：

- `IOnDispatchStarted : IDispatch` — 包含 `void OnDispatchStarted(EcsEntity entity, int count)`。
- `IOnDispatchCancelled : IDispatch` — 包含 `void OnDispatchCancelled(EcsEntity entity)`。
- `IOnDispatchCompleted : IDispatch` — 包含 `void OnDispatchCompleted(EcsEntity entity)`。
- `IOnDispatchTimeout : IDispatch` — 包含 `void OnDispatchTimeout(EcsEntity entity)`。

示例：在系统内派发“开始派遣”事件：

```csharp
// 派发给实现了 IOnDispatchStarted 的监听者
entity.Dispatch<IOnDispatchStarted>(d => d.OnDispatchStarted(entity, count));
```

示例：在实现类中只实现需要的事件接口，避免实现不必要的方法：

```csharp
public class DispatchLogger : IOnDispatchStarted, IOnDispatchCompleted
{
	public void OnDispatchStarted(EcsEntity entity, int count)
	{
		// 仅记录开始事件
	}

	public void OnDispatchCompleted(EcsEntity entity)
	{
		// 仅记录完成事件
	}
}
```

此拆分减少实现类的接口膨胀，并使得事件订阅更精细（只订阅关心的事件）。

## 系统业务逻辑方法规范
- 业务逻辑方法均为静态方法。
- 方法签名只包含实体及必要参数，组件通过 `entity.GetComponent<T>()` 在方法内部获取。
- 每个方法必须包含必要的 summary 注释，说明参数、返回值和错误条件。

示例：

```csharp
/// <summary>
/// 尝试开始一次派遣操作。
/// </summary>
/// <param name="entity">派遣的实体。</param>
/// <param name="count">派遣数量。</param>
/// <returns>是否成功。</returns>
public static bool StartDispatch(EcsEntity entity, int count)
{
		// implementation: get components, validate, update state, dispatch events
}
```

## 命名与引用
- 命名空间：`ECSGame.UnitDispatchModule`。
- 常用引用：
	```csharp
	using ECS;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	```

## 设计原则与注意事项
- 单一职责：每个系统/实体/组件只负责单一职责。
- 开闭原则：通过 `IDispatch` 接口派发事件以便外部扩展，不直接依赖具体实现。
- 依赖倒置：高层模块依赖抽象接口。
- 实体创建只能通过 `AddChild<T>()` 并在 `beforeAwake` 填充必要初始数据。
- 线程与并发：模块不主动创建线程，所有并发访问应通过上层调度或框架保证；系统方法需对可能的重复调用进行幂等或保护。

## 示例文件与位置建议（1.0.3 命名统一）
- 组件：`Scripts/Component/UnitDispatcher.cs`，`Scripts/Component/DispatchStateComponent.cs`，`Scripts/Component/DispatchRuleComponent.cs`，`Scripts/Component/UnitDispatcherListComponent.cs`
- 系统：`Scripts/System/UnitDispatcherSystem.cs`，`Scripts/System/DispatchComponentSystem.cs`，`Scripts/System/UnitDispatcherListSystem.cs`
- 接口：`Scripts/Dispatch/IDispatchEvents.cs`

## 质量门（Quality Gates）与测试建议
- 为每个静态业务方法编写单元测试（happy path + 参数校验 + 组件缺失）。
- 在变更实体/组件结构时运行类型检查与单元测试，确保不破坏向后兼容性。

## 后续工作（可选）
- 根据上层运行时框架添加集成测试场景（模拟派遣流程）。
- 补充配置加载示例与默认配置文件。

---

以上为模块程序设计文档的初稿，覆盖数据结构、系统设计与实现约定；如需我将这些设计转换为骨架代码（实体/组件/系统的 cs 文件）和单元测试，我可以继续生成实现代码和测试样例。
