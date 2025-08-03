# （达成系统）模块程序单元测试设计文档

## 1. 测试目标

本单元测试文档旨在验证达成系统各核心系统类（AchieveItemSystem、AchieveConditionSystem、AchieveItemConditionSystem、AchieveItemRewardSystem）的主要业务逻辑方法的正确性，确保模块功能的健壮性和可扩展性。

## 2. 测试范围

仅需对系统类的静态业务方法和生命周期接口进行测试，不需要对实体和组件的属性数据进行单独测试。

## 3. 测试结构规范

- 测试类命名为 `**ModuleTests`，位于 `system.module-test` 文件夹下。
- 测试类需包含如下结构：
  - `ecsNode` 属性，用于测试实体和组件。
  - `TestEcsNode` 类继承自 `EcsNode`，模拟Ecs域。
  - `[SetUp]` 方法初始化测试环境。
  - 测试方法使用 `[Test]` 特性标记。
- 断言使用 `Assert.That` 接口。

## 4. 各系统测试建议

### 4.1 AchieveItemSystem 测试

- **测试内容：**
  - Create 创建达成项实体，验证属性赋值是否正确。
  - Init 初始化达成项实体，验证初始化后状态。
  - UpdateStatus 状态变更逻辑。
  - IsCompleted 完成判定逻辑。
- **断言建议：**
  - 实体属性与配置一致。
  - 状态变更后符合预期。
  - 完成判定结果正确。

### 4.2 AchieveConditionSystem 测试

- **测试内容：**
  - Create 创建条件实体，验证属性赋值。
  - UpdateProgress 进度更新逻辑。
  - IsSatisfied 满足判定逻辑。
- **断言建议：**
  - 进度更新后 CurrentValue 正确。
  - 满足判定结果正确。

### 4.3 AchieveItemConditionSystem 测试

- **测试内容：**
  - InitConditions 条件集合初始化。
  - CalculateProgress 进度计算。
  - IsAllSatisfied 全部满足判定。
- **断言建议：**
  - 条件集合初始化后数量与配置一致。
  - 进度百分比计算正确。
  - 全部满足判定结果正确。

### 4.4 AchieveItemRewardSystem 测试

- **测试内容：**
  - InitRewards 奖励集合初始化。
  - GrantRewards 奖励发放逻辑。
  - UpdateRewardStatus 状态变更。
- **断言建议：**
  - 奖励集合初始化后数量与配置一致。
  - 发放后状态与领取时间正确。

## 5. 示例测试类结构

```csharp
using ECS;
using NUnit.Framework;

namespace ECSGame.Module.Achieve.Tests
{
    [TestFixture]
    public class AchieveModuleTests
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
        public void TestAchieveItemCreate()
        {
            // Arrange: 构造配置
            // Act: 调用 AchieveItemSystem.Create
            // Assert: 验证实体属性
        }
    }
}
```

## 6. 其他说明

- 测试用例应覆盖正常流程、边界情况和异常输入。
- 可根据实际业务扩展更多测试用例。

