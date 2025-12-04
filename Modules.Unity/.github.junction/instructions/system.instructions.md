---
applyTo: "com.system.**/**/*.cs"
---

- 禁止生成静态类
- 禁止给系统类加static标记

## 系统类：
- 系统类不得用静态类
- 只实现方法逻辑，不实现属性数据
- 系统为实例类，但系统业务方法仍为静态方法，方便调用
- 实体系统继承 `AEntitySystem<T>`
    - 实体系统应有一个或多个Create静态业务方法用于创建实体

- 组件系统继承 `AComponentSystem<T, C>`

### 系统派发事件接口
- 派发事件接口继承 `IDispatch`
示例：
```csharp
public interface IStateEnterHandler : IDispatch
{
    void OnStateEnterHandle(EcsEntity entity, int param);
}
```
- 为遵循开闭原则和依赖倒置原则，可扩展的系统功能应提供事件接口派发到外部由开发者自定义扩展逻辑

### 系统业务逻辑方法
- 系统业务逻辑方法均为静态方法
- 只传实体和必要参数，组件在方法内获取
- 需要详细summary注释
- 免除判空判断，默认组件和参数不为null
- 模块内的系统业务逻辑调用规范为：只有自身组件系统逻辑允许GetComponent，跨组件系统逻辑调用必须通过System的静态方法调用，不允许跨组件GetComponent

示例：
```csharp
/// <summary>
/// 处理业务逻辑
/// </summary>
/// <param name="entity">实体</param>
/// <param name="param">参数</param>
public static void HandleLogic(EcsEntity entity, int param)
{
    var component = entity.GetComponent<MyComponent>();
    // 处理逻辑
}
```
