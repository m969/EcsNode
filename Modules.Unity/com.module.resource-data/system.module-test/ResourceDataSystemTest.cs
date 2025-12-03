using ECS;
using ECSGame.ResourceDataModule;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ECSGame.ResourceDataModule.Tests
{
    [TestFixture]
    public class ResourceDataSystemTest
    {
        private EcsNode ecsNode;
        private EcsEntity player;
        private ResourceDataComponent resourceData;

        public class TestEcsNode : EcsNode
        {
            public TestEcsNode(ushort id) : base(id) { }
        }

        [SetUp]
        public void SetUp()
        {
            ecsNode = new TestEcsNode(1);
            player = ecsNode.AddChild<EcsEntity>();
            resourceData = player.AddComponent<ResourceDataComponent>(c =>
            {
                c.ResourceValues = new Dictionary<int, int>
                {
                    { ResourceType.Coin, 100 },
                    { ResourceType.Diamond, 50 },
                    { ResourceType.Stamina, 10 }
                };
                c.LastSyncTime = DateTime.MinValue;
            });
        }

        // 将所有 Assert.AreEqual(...) 替换为 Assert.That(..., Is.EqualTo(...))
        [Test]
        public void TC01_资源获取测试()
        {
            ResourceDataSystem.GainResource(player, ResourceType.Coin, 10, 0);
            Assert.That(resourceData.ResourceValues[ResourceType.Coin], Is.EqualTo(110));
        }

        [Test]
        public void TC02_资源消耗测试()
        {
            ResourceDataSystem.ConsumeResource(player, ResourceType.Diamond, 20, 0);
            Assert.That(resourceData.ResourceValues[ResourceType.Diamond], Is.EqualTo(30));
        }

        [Test]
        public void TC03_资源同步测试()
        {
            resourceData.ResourceValues[ResourceType.Stamina] = 99;
            ResourceDataSystem.SyncResource(player);
            Assert.That(resourceData.LastSyncTime, Is.Not.EqualTo(DateTime.MinValue));
        }

        [Test]
        public void TC04_资源变更日志记录测试()
        {
            var log = ResourceChangeLogSystem.AddChangeLog(player, ResourceChangeType.Gain, 5, "任务奖励", ResourceType.Coin, 1);
            Assert.That(log.ChangeType, Is.EqualTo(ResourceChangeType.Gain));
            Assert.That(log.ChangeValue, Is.EqualTo(5));
            Assert.That(log.Reason, Is.EqualTo("任务奖励"));
            Assert.That(log.ResourceType, Is.EqualTo(ResourceType.Coin));
            Assert.That(log.OwnerId, Is.EqualTo(1));
        }

        [Test]
        public void TC05_资源批量变更测试()
        {
            var changes = new Dictionary<int, int>
            {
                {ResourceType.Coin, 20 },
                {ResourceType.Diamond, -10 }
            };
            ResourceDataSystem.BatchChange(player, changes);
            Assert.That(resourceData.ResourceValues[ResourceType.Coin], Is.EqualTo(120));
            Assert.That(resourceData.ResourceValues[ResourceType.Diamond], Is.EqualTo(40));
        }

        [Test]
        public void TC06_资源重置测试()
        {
            ResourceDataSystem.ResetResource(player);
            Assert.That(ResourceDataSystem.GetResourceValue(player, ResourceType.Coin), Is.EqualTo(0));
            Assert.That(ResourceDataSystem.GetResourceValue(player, ResourceType.Diamond), Is.EqualTo(0));
            Assert.That(ResourceDataSystem.GetResourceValue(player, ResourceType.Stamina), Is.EqualTo(0));
        }

        [Test]
        public void TC08_资源与道具结合场景测试()
        {
            ResourceDataSystem.GainResource(player, ResourceType.Item, 3, 1001);
            Assert.That(resourceData.ResourceValues[ResourceType.Item], Is.EqualTo(3));
        }

        [Test]
        public void TC09_边界条件测试()
        {
            resourceData.ResourceValues[ResourceType.Coin] = 0;
            ResourceDataSystem.ConsumeResource(player, ResourceType.Coin, 1, 0);
            Assert.That(resourceData.ResourceValues[ResourceType.Coin], Is.EqualTo(0));
        }

        [Test]
        public void TC10_异常处理测试()
        {
            Assert.DoesNotThrow(() =>
            {
                ResourceDataSystem.GainResource(player, 999, 10, 0);
            });
        }
    }
}
