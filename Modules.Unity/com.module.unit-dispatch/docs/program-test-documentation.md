# （单位派遣）模块程序单元测试设计文档

本文档基于 `program-documentation.md` 的最新设计来撰写单元测试策略与用例。目标是保证模块的数据结构、组件与系统在常见路径及边界情况下行为正确，并在重构时保持向后兼容。

## 目标
- 明确需要覆盖的模块边界与行为契约（Contract）。
- 提供可重复、快速的单元测试用例（happy path + 参数校验 + 组件缺失等）。
- 指导如何在 CI 中运行测试并验证质量门（Quality Gates）。

## 适用范围
- 模块：`ECSGame.UnitDispatchModule`
- 范围：组件/实体类（`UnitDispatcher`（原 `DispatchExecution`）/ `DispatchStateComponent` / `DispatchRuleComponent` / `UnitDispatcherListComponent`（原 `DispatchExecutionListComponent`））与系统（`UnitDispatcherSystem`（原 `DispatchExecutionSystem`）/ `DispatchComponentSystem` / `UnitDispatcherListSystem`（原 `DispatchExecutionListSystem`））的静态方法与生命周期逻辑。

## 测试契约（Checklist / Contract）
- 输入：通过 `EcsEntity.AddComponent<T>()` 或 `AddChild<T>()` 创建/注入组件，并以静态方法调用系统逻辑。
- 输出：组件状态变更、组件属性更新、通过 `IDispatch` 系列接口派发的外部事件。
- 错误模式：参数校验失败（null、无效 Id）、组件缺失、重复派发、超时触发。

## 测试策略
- 以单元测试为主，框架为 `dotnet test`（xUnit / NUnit / MSTest，按项目现有测试框架）。
- 每个静态业务方法实现至少包含：
	- Happy path
	- 参数校验失败
	- 组件缺失
	- 并发或重复调用情形（若方法需处理）
- 对于生命周期 Hook（Awake/Init/AfterInit 等），使用模拟实体并验证组件在 Awake 前后字段已按约定填充（利用 `beforeAwake` 回调注入参数）。

## 关键测试用例（示例）

1) UnitDispatcherSystem.Create（原 DispatchExecutionSystem.Create）
	- 目的：验证 Create 会在实体上添加或创建 `DispatchExecution`（或创建独立执行实体）并且填充必要字段（ConfigId、DispatchCount 初始值等）。
	- 输入：有效 EcsEntity，合法 UnitDispatchConfig
	- 断言：返回的 `UnitDispatcher`（原 `DispatchExecution`）不为 null；ConfigId 与传入一致；DispatchCount 初始为 0 或文档规定值。

2) UnitDispatcherSystem.StartDispatch（原 DispatchExecutionSystem.StartDispatch）
	- 目的：在已有 `DispatchExecution` 时开始派遣并更新计数/状态、派发 `IOnDispatchStarted`。
	- 用例：正常开始（count>0），参数无效（count<=0），执行体或组件缺失。
	- 断言：状态变更、计数累加（或按文档规则）、监听接口收到回调（可用测试双替身/Mock）。

3) UnitDispatcherSystem.CancelDispatch（原 DispatchExecutionSystem.CancelDispatch）
	- 目的：验证取消逻辑会清理计时、恢复状态并派发 `IOnDispatchCancelled`。

4) DispatchComponentSystem.Tick（名称未变）
	- 目的：模拟时间流逝，验证超时检测会触发 `IOnDispatchTimeout`，并正确更新 `DispatchStateComponent`。

5) DispatchComponentSystem.CanDispatch
	- 目的：规则校验，用例包含满足规则与不满足规则的参数集。

6) 配置加载与边界
	- 目的：验证缺失配置或非法配置 Id 时的失败路径。

## 测试实现建议与 contract
- 每个测试应明确 Arrange / Act / Assert。测试中的实体应该尽量精简，仅包含被测组件与必须的最小依赖。
- 使用轻量 Mock 或 Fake 实现 `IDispatch` 接口以捕获事件调用（不要在单元测试中启动真实的外部流程）。
- 对于需要时间相关的逻辑（Timeout、Tick），使用注入的时间参数或将时间提供者抽象为可替换实现以便在测试中控制时间流。

## 测试示例代码（伪代码）

```csharp
// Arrange
var entity = new EcsEntity();
var cfg = new UnitDispatchConfig { Id = 1, MaxDispatchCount = 5 };

// Act
var comp = DispatchExecutionSystem.Create(entity, cfg);

// Assert
Assert.NotNull(comp);
Assert.Equal(1, comp.ConfigId);
Assert.Equal(0, comp.DispatchCount);
```

## 质量门（Quality Gates）
- 所有新增或修改的静态业务方法必须有对应单元测试覆盖（最低覆盖：happy path + 参数校验）。
- 在 PR 流程中，CI 应执行 `dotnet build` 与 `dotnet test`，任何编译错误或测试失败都会阻断合并。

## 在本地运行测试
- 推荐在模块根目录运行：

```powershell
cd d:\Documents\MyGithub\EcsNode\Modules.Unity\com.module.unit-dispatch
dotnet test system.module-test.unit-dispatch\System.UnitDispatchModule.Test.csproj
```

（说明：根据本地环境可替换为解决方案层运行 `dotnet test com.module.unit-dispatch.sln`。）

## 假设与备注
- 假设项目使用 .NET SDK 并且开发机已安装相应 SDK；若 CI 环境不同，需要在 CI 配置中预装 SDK。
- 假设 `EcsEntity` 与 `EcsComponent` 可在测试中直接实例化或通过轻量工厂创建。

## 后续工作（可选）
- 增加对复杂场景的集成测试（模拟上层流程、配置加载与事件回调链）。
- 在测试项目中引入时间服务抽象以稳定 Timeout 测试。

---

以上为根据最新程序文档整理的单元测试设计文档；如需我直接添加示例测试代码到 `system.module-test.unit-dispatch` 项目中，我可以继续生成并在本地运行验证。
