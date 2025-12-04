using ECS;
using NUnit.Framework;
using ECSGame.AchieveModule;
using System;
using System.Collections.Generic;

namespace ECSGame.AchieveModule.Tests
{
    public class AchieveItemConfig : IAchieveItemConfig
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public AchieveType Type { get; set; }
        public int Priority { get; set; }
    }

    public class AchieveConditionConfig : IAchieveConditionConfig
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string ConditionType { get; set; } = string.Empty;
        public int TargetValue { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }

    [TestFixture]
    public class AchieveModuleTests
    {
        private EcsNode? ecsNode;

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
            IAchieveItemConfig config = new AchieveItemConfig { Id = 1001, Name = "测试成就", Type = AchieveType.OneTime };
            // Act: 调用 AchieveItemSystem.Create
            var entity = AchieveItemSystem.Create(ecsNode!, config);
            // Assert: 验证实体属性
            Assert.That(entity, Is.Not.Null);
            Assert.That(entity.Name, Is.EqualTo(config.Name));
        }

        [Test]
        public void TestAchieveItemInit()
        {
            IAchieveItemConfig config = new AchieveItemConfig { Id = 1002, Name = "初始化成就", Type = AchieveType.OneTime };
            var entity = AchieveItemSystem.Create(ecsNode!, config);
            AchieveItemSystem.Init(entity, config);
        }

        [Test]
        public void TestAchieveItemUpdateStatus()
        {
            IAchieveItemConfig config = new AchieveItemConfig { Id = 1003, Name = "状态变更成就", Type = AchieveType.OneTime };
            var entity = AchieveItemSystem.Create(ecsNode!, config);
            AchieveItemSystem.UpdateStatus(entity, AchieveStatus.Completed);
            Assert.That(entity.Status, Is.EqualTo(AchieveStatus.Completed));
        }

        [Test]
        public void TestAchieveItemIsCompleted()
        {
            IAchieveItemConfig config = new AchieveItemConfig { Id = 1004, Name = "完成判定成就", Type = AchieveType.OneTime };
            var entity = AchieveItemSystem.Create(ecsNode!, config);
            AchieveItemSystem.UpdateStatus(entity, AchieveStatus.Completed);
            var result = AchieveItemSystem.IsCompleted(entity);
            Assert.That(result, Is.True);
        }

        [Test]
        public void TestAchieveConditionProgress()
        {
            // Arrange
            IAchieveItemConfig itemConfig = new AchieveItemConfig { Id = 2001, Name = "条件测试成就" };
            var entity = AchieveItemSystem.Create(ecsNode!, itemConfig);
            
            var conditionConfigs = new List<IAchieveConditionConfig>
            {
                new AchieveConditionConfig { Id = 1, TargetValue = 10 }
            };
            AchieveConditionListSystem.InitConditions(entity, conditionConfigs);

            // Act
            AchieveConditionListSystem.UpdateConditionProgress(entity, 0, 5);

            // Assert
            var component = entity.GetComponent<AchieveConditionListComponent>();
            Assert.That(component.ConditionList[0].CurrentValue, Is.EqualTo(5));
            Assert.That(component.CurrentProgress, Is.EqualTo(5));
            Assert.That(component.IsAllSatisfied, Is.False);
        }

        [Test]
        public void TestAchieveAutoCompletion()
        {
            // Arrange
            IAchieveItemConfig itemConfig = new AchieveItemConfig { Id = 2002, Name = "自动完成测试" };
            var entity = AchieveItemSystem.Create(ecsNode!, itemConfig);
            
            var conditionConfigs = new List<IAchieveConditionConfig>
            {
                new AchieveConditionConfig { Id = 1, TargetValue = 10 }
            };
            AchieveConditionListSystem.InitConditions(entity, conditionConfigs);

            // Act
            AchieveConditionListSystem.UpdateConditionProgress(entity, 0, 10);

            // Assert
            var component = entity.GetComponent<AchieveConditionListComponent>();
            Assert.That(component.ConditionList[0].IsSatisfied, Is.True);
            Assert.That(component.IsAllSatisfied, Is.True);
            Assert.That(entity.Status, Is.EqualTo(AchieveStatus.Completed));
        }
    }
}
