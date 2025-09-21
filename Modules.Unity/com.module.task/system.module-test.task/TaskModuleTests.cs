using ECS;
using NUnit.Framework;
using System;

namespace ECSGame.TaskModule.Tests
{
    [TestFixture]
    public class TaskModuleTests
    {
        private EcsNode ecsNode;

        public class TestEcsNode : EcsNode
        {
            public TestEcsNode(ushort id) : base(id) { }
        }

        private class TaskConfig : ITaskConfig
        {
            public int Id { get; init; }
            public string Key { get; init; } = string.Empty;
            public string Name { get; init; } = string.Empty;
            public string Desc { get; init; } = string.Empty;
            public int AchieveItemId { get; init; }
            public int RewardConfigId { get; init; }
        }

        [SetUp]
        public void SetUp()
        {
            ecsNode = new TestEcsNode(1);
        }

        [Test]
        public void Create_Activate_Complete_Claim_Reset()
        {
            var root = ecsNode;
            var list = root.AddComponent<TaskListComponent>();

            var config = new TaskConfig { Id = 100, Key = "K", Name = "N", Desc = "D", AchieveItemId = 200, RewardConfigId = 300 };
            var task = TaskItemSystem.Create(root, config);

            Assert.That(task.TaskId, Is.EqualTo(config.Id));
            Assert.That(task.State, Is.EqualTo(TaskState.Inactive));

            Assert.That(TaskListSystem.Add(root, task), Is.True);
            Assert.That(TaskListSystem.Get(root, config.Id), Is.Not.Null);

            TaskListSystem.Activate(root, config.Id);
            Assert.That(task.State, Is.EqualTo(TaskState.InProgress));

            TaskListSystem.OnAchieveProgressChanged(root, config.AchieveItemId, 1f, completed: true);
            Assert.That(task.State, Is.EqualTo(TaskState.Claimable));

            bool granted = TaskListSystem.Claim(root, config.Id, (rid) => rid == config.RewardConfigId);
            Assert.That(granted, Is.True);
            Assert.That(task.State, Is.EqualTo(TaskState.Claimed));

            Assert.That(TaskListSystem.Reset(root, config.Id, true), Is.True);
            Assert.That(task.State, Is.EqualTo(TaskState.Inactive));
        }
    }
}
