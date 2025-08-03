using ECS;
using NUnit.Framework;
using ECSGame.Module.Achieve;
using System;
using System.Collections.Generic;

namespace ECSGame.Module.Achieve.Tests
{
    public class AchieveItemConfig : IAchieveItemConfig
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public AchieveType Type { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }
        public List<int> Prerequisites { get; set; } = new List<int>();
        public long ValidityPeriod { get; set; }
    }

    public class AchieveConditionConfig : IAchieveConditionConfig
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string ConditionType { get; set; } = string.Empty;
        public int TargetValue { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }

    public class AchieveRewardConfig : IAchieveRewardConfig
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public RewardType RewardType { get; set; }
        public int ItemId { get; set; }
        public int Amount { get; set; }
    }

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
            IAchieveItemConfig config = new AchieveItemConfig { Id = 1001, Name = "测试成就", Type = AchieveType.OneTime };
            // Act: 调用 AchieveItemSystem.Create
            var entity = AchieveItemSystem.Create(ecsNode, config);
            // Assert: 验证实体属性
            Assert.That(entity, Is.Not.Null);
            Assert.That(entity.Name, Is.EqualTo(config.Name));
        }

        [Test]
        public void TestAchieveItemInit()
        {
            IAchieveItemConfig config = new AchieveItemConfig { Id = 1002, Name = "初始化成就", Type = AchieveType.OneTime };
            var entity = AchieveItemSystem.Create(ecsNode, config);
            AchieveItemSystem.Init(entity, config);
        }

        [Test]
        public void TestAchieveItemUpdateStatus()
        {
            IAchieveItemConfig config = new AchieveItemConfig { Id = 1003, Name = "状态变更成就", Type = AchieveType.OneTime };
            var entity = AchieveItemSystem.Create(ecsNode, config);
            AchieveItemSystem.UpdateStatus(entity, AchieveStatus.Completed);
            Assert.That(entity.Status, Is.EqualTo(AchieveStatus.Completed));
        }

        [Test]
        public void TestAchieveItemIsCompleted()
        {
            IAchieveItemConfig config = new AchieveItemConfig { Id = 1004, Name = "完成判定成就", Type = AchieveType.OneTime };
            var entity = AchieveItemSystem.Create(ecsNode, config);
            AchieveItemSystem.UpdateStatus(entity, AchieveStatus.Completed);
            var result = AchieveItemSystem.IsCompleted(entity);
            Assert.That(result, Is.True);
        }

        [Test]
        public void TestAchieveConditionCreateAndProgress()
        {
            IAchieveConditionConfig config = new AchieveConditionConfig { Id = 2001, TargetValue = 10 };
            var entity = AchieveConditionSystem.Create(ecsNode, config);
            Assert.That(entity, Is.Not.Null);
            AchieveConditionSystem.UpdateProgress(entity, 5);
            Assert.That(entity.CurrentValue, Is.EqualTo(5));
        }

        [Test]
        public void TestAchieveConditionIsSatisfied()
        {
            IAchieveConditionConfig config = new AchieveConditionConfig { Id = 2002, TargetValue = 3 };
            var entity = AchieveConditionSystem.Create(ecsNode, config);
            AchieveConditionSystem.UpdateProgress(entity, 3);
            var result = AchieveConditionSystem.IsSatisfied(entity);
            Assert.That(result, Is.True);
        }

        [Test]
        public void TestAchieveItemConditionInitAndProgress()
        {
            IAchieveItemConfig itemConfig = new AchieveItemConfig { Id = 3001, Name = "条件集合成就", Type = AchieveType.OneTime };
            var entity = AchieveItemSystem.Create(ecsNode, itemConfig);
            var conditionConfigs = new List<IAchieveConditionConfig> { new AchieveConditionConfig { Id = 1, TargetValue = 5 }, new AchieveConditionConfig { Id = 2, TargetValue = 10 } };
            AchieveItemConditionSystem.InitConditions(entity, conditionConfigs);
            var comp = entity.GetComponent<AchieveItemConditionComponent>();
            Assert.That(comp.ConditionList.Count, Is.EqualTo(conditionConfigs.Count));
            AchieveItemConditionSystem.CalculateProgress(comp);
            Assert.That(comp.ProgressPercentage, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void TestAchieveItemConditionIsAllSatisfied()
        {
            IAchieveItemConfig itemConfig = new AchieveItemConfig { Id = 3002, Name = "全部满足成就", Type = AchieveType.OneTime };
            var entity = AchieveItemSystem.Create(ecsNode, itemConfig);
            var conditionConfigs = new List<IAchieveConditionConfig> { new AchieveConditionConfig { Id = 1, TargetValue = 1 }, new AchieveConditionConfig { Id = 2, TargetValue = 1 } };
            AchieveItemConditionSystem.InitConditions(entity, conditionConfigs);
            foreach (var condEntity in entity.GetComponent<AchieveItemConditionComponent>().ConditionList)
            {
                AchieveConditionSystem.UpdateProgress(condEntity, 1);
            }
            var result = AchieveItemConditionSystem.IsAllSatisfied(entity.GetComponent<AchieveItemConditionComponent>());
            Assert.That(result, Is.True);
        }

        [Test]
        public void TestAchieveItemRewardInitAndGrant()
        {
            IAchieveItemConfig itemConfig = new AchieveItemConfig { Id = 4001, Name = "奖励集合成就", Type = AchieveType.OneTime };
            var entity = AchieveItemSystem.Create(ecsNode, itemConfig);
            var rewardConfigs = new List<IAchieveRewardConfig> { new AchieveRewardConfig { Id = 1, Amount = 100 }, new AchieveRewardConfig { Id = 2, Amount = 200 } };
            AchieveItemRewardSystem.InitRewards(entity, rewardConfigs);
            var comp = entity.GetComponent<AchieveItemRewardComponent>();
            Assert.That(comp.RewardList.Count, Is.EqualTo(rewardConfigs.Count));
            AchieveItemRewardSystem.GrantRewards(comp, ecsNode); // 传入奖励组件和玩家实体
        }

        [Test]
        public void TestAchieveItemRewardUpdateStatus()
        {
            IAchieveItemConfig itemConfig = new AchieveItemConfig { Id = 4002, Name = "奖励状态成就", Type = AchieveType.OneTime };
            var entity = AchieveItemSystem.Create(ecsNode, itemConfig);
            var rewardConfigs = new List<IAchieveRewardConfig> { new AchieveRewardConfig { Id = 1, Amount = 100 } };
            AchieveItemRewardSystem.InitRewards(entity, rewardConfigs);
            var comp = entity.GetComponent<AchieveItemRewardComponent>();
            AchieveItemRewardSystem.UpdateRewardStatus(comp, RewardStatus.Granted);
            Assert.That(comp.RewardStatus, Is.EqualTo(RewardStatus.Granted));
        }
    }
}
