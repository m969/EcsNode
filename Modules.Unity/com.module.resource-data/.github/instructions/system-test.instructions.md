---
applyTo: "system.module-test.**/**/*.cs"
---

模块系统单元测试代码

只需要实现系统类的测试流程，不需要实现实体和组件等的测试流程

基于以下测试框架实现测试流程：
- "NUnit" Version="4.3.2"
- "Microsoft.NET.Test.Sdk" Version="17.14.1"
- "NUnit3TestAdapter" Version="5.0.0"

使用Visual Studio或Rider等IDE运行测试
- 测试类命名为 `**ModuleTests`
- 测试类放在 `system.module-test` 文件夹下
- 测试断言使用 `Assert` 类的Assert.That接口，例如：
```csharp
Assert.That(condition, Is.Not.Null);
Assert.That(config.Id, Is.EqualTo(item.Id));
```

- 测试类应有以下结构：
    - ecsNode属性，用于测试实体和组件
    - TestEcsNode 类继承自 `EcsNode`模拟Ecs域，用于测试实体和组件
    - `SetUp` 方法用于初始化测试环境
    - 测试方法使用 `[Test]` 特性标记
    - 模拟配置类定义（如果需要）
    - 系统循环更新使用EcsNode的DriveEntityUpdate函数


- 测试类示例：
```csharp
using ECS;
using NUnit.Framework;

namespace ECSGame.AchieveModule.Tests
{
    [TestFixture]
    public class MyModuleTests
    {
        private EcsNode ecsNode;

        public class TestEcsNode : EcsNode
        {
            public TestEcsNode(ushort id) : base(id) { }
        }

        [SetUp]
        public void SetUp()
        {
            ecsNode = new TestEcsNode(1);
        }

        [Test]
        public void TestMethodA()
        {

        }
    }
}