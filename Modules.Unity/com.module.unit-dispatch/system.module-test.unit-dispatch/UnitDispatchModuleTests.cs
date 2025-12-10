using System;
using ECS;
using NUnit.Framework;

namespace ECSGame.UnitDispatchModule.Tests
{
    [TestFixture]
    public class UnitDispatchModuleTests
    {
        private EcsNode ecsNode = null!;

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
        public void Create_AddsDispatchAgentComponent_WithDefaults()
        {
            var entity = ecsNode.AddChild<EcsEntity>(e => { });

            var agent = entity.AddComponent<DispatchAgentComponent>(c => 
            {
                c.ConfigId = 42;
            });

            Assert.That(agent, Is.Not.Null);
            Assert.That(agent.ConfigId, Is.EqualTo(42));
            Assert.That(agent.CurrentDispatchCount, Is.EqualTo(0));
            Assert.That(agent.TargetEntityId, Is.EqualTo(0));
            Assert.That(agent.RemainingTimeout, Is.EqualTo(0f));
            Assert.That(agent.State, Is.EqualTo(DispatchState.Idle));
        }

        [Test]
        public void StartDispatch_Succeeds_WhenIdle()
        {
            var entity = ecsNode.AddChild<EcsEntity>(e => { });
            var agent = entity.AddComponent<DispatchAgentComponent>(c =>
            {
                c.ConfigId = 1;
                c.State = DispatchState.Idle;
            });

            long targetId = 100L;
            float timeout = 5f;
            int count = 2;

            var ok = DispatchAgentSystem.StartDispatch(entity, targetId, count, timeout);

            Assert.That(ok, Is.True);
            Assert.That(agent.CurrentDispatchCount, Is.EqualTo(count));
            Assert.That(agent.TargetEntityId, Is.EqualTo(targetId));
            Assert.That(agent.RemainingTimeout, Is.EqualTo(timeout));
            Assert.That(agent.State, Is.EqualTo(DispatchState.Dispatching));
        }

        [Test]
        public void StartDispatch_Fails_WhenAlreadyDispatching()
        {
            var entity = ecsNode.AddChild<EcsEntity>(e => { });
            var agent = entity.AddComponent<DispatchAgentComponent>(c =>
            {
                c.ConfigId = 1;
                c.State = DispatchState.Dispatching;
                c.TargetEntityId = 99;
                c.RemainingTimeout = 10f;
            });

            var ok = DispatchAgentSystem.StartDispatch(entity, 200L, 1, 3f);

            Assert.That(ok, Is.False);
            // 失败时不应修改既有数据
            Assert.That(agent.TargetEntityId, Is.EqualTo(99));
            Assert.That(agent.State, Is.EqualTo(DispatchState.Dispatching));
        }

        [Test]
        public void CancelDispatch_ClearsState_And_SetsCancelled()
        {
            var entity = ecsNode.AddChild<EcsEntity>(e => { });
            var agent = entity.AddComponent<DispatchAgentComponent>(c =>
            {
                c.State = DispatchState.Dispatching;
                c.TargetEntityId = 55;
                c.RemainingTimeout = 12f;
            });

            DispatchAgentSystem.CancelDispatch(entity);

            Assert.That(agent.State, Is.EqualTo(DispatchState.Cancelled));
            Assert.That(agent.TargetEntityId, Is.EqualTo(0));
            Assert.That(agent.RemainingTimeout, Is.EqualTo(0));
        }

        [Test]
        public void CompleteDispatch_SetsCompleted()
        {
            var entity = ecsNode.AddChild<EcsEntity>(e => { });
            var agent = entity.AddComponent<DispatchAgentComponent>(c =>
            {
                c.State = DispatchState.Dispatching;
                c.TargetEntityId = 55;
            });

            DispatchAgentSystem.CompleteDispatch(entity);

            Assert.That(agent.State, Is.EqualTo(DispatchState.Completed));
            Assert.That(agent.TargetEntityId, Is.EqualTo(0));
        }

        [Test]
        public void Tick_UpdatesTimeout()
        {
            var entity = ecsNode.AddChild<EcsEntity>(e => { });
            var agent = entity.AddComponent<DispatchAgentComponent>(c =>
            {
                c.State = DispatchState.Dispatching;
                c.RemainingTimeout = 10f;
            });

            DispatchAgentSystem.Tick(entity, 1f);

            Assert.That(agent.RemainingTimeout, Is.EqualTo(9f));
            Assert.That(agent.State, Is.EqualTo(DispatchState.Dispatching));
        }

        [Test]
        public void Tick_TriggersTimeout_WhenZero()
        {
            var entity = ecsNode.AddChild<EcsEntity>(e => { });
            var agent = entity.AddComponent<DispatchAgentComponent>(c =>
            {
                c.State = DispatchState.Dispatching;
                c.RemainingTimeout = 0.5f;
            });

            DispatchAgentSystem.Tick(entity, 1f);

            Assert.That(agent.RemainingTimeout, Is.LessThanOrEqualTo(0));
            Assert.That(agent.State, Is.EqualTo(DispatchState.Timeout));
        }
    }
}
