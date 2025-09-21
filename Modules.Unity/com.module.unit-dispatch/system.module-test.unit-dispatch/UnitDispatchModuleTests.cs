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
        public void Create_AddsDispatchUnitComponent_WithDefaults()
        {
            var entity = ecsNode.AddChild<EcsEntity>(e => { });

            var comp = UnitDispatcherSystem.Create(entity, 42);

            Assert.That(comp, Is.Not.Null);
            Assert.That(comp.ConfigId, Is.EqualTo(42));
            Assert.That(comp.DispatchCount, Is.EqualTo(0));
            Assert.That(comp.TargetEntityId, Is.EqualTo(0));
            Assert.That(comp.Timeout, Is.EqualTo(0f));
        }

        [Test]
        public void StartDispatch_Succeeds_WhenIdle()
        {
            var entity = ecsNode.AddChild<EcsEntity>(e => { });
            var unit = UnitDispatcherSystem.Create(entity, 1);
            entity.AddComponent<DispatchStateComponent>(c =>
            {
                c.State = DispatchState.Idle;
                c.TargetEntityId = 0;
                c.RemainingTimeout = 0f;
            });

            unit.TargetEntityId = 100L;
            unit.Timeout = 5f;
            var ok = UnitDispatcherSystem.StartDispatch(unit, 2);

            Assert.That(ok, Is.True);
            Assert.That(unit.DispatchCount, Is.EqualTo(2));
            Assert.That(unit.TargetEntityId, Is.EqualTo(100L));
            Assert.That(unit.Timeout, Is.EqualTo(5f));

            var state = entity.GetComponent<DispatchStateComponent>();
            Assert.That(state.State, Is.EqualTo(DispatchState.Dispatching));
            Assert.That(state.TargetEntityId, Is.EqualTo(100L));
            Assert.That(state.RemainingTimeout, Is.EqualTo(5f));
        }

        [Test]
        public void StartDispatch_Fails_WhenAlreadyDispatching()
        {
            var entity = ecsNode.AddChild<EcsEntity>(e => { });
            var unit = UnitDispatcherSystem.Create(entity, 1);
            entity.AddComponent<DispatchStateComponent>(c =>
            {
                c.State = DispatchState.Dispatching;
                c.TargetEntityId = 99;
                c.RemainingTimeout = 10f;
            });

            unit.TargetEntityId = 200L;
            unit.Timeout = 3f;
            var ok = UnitDispatcherSystem.StartDispatch(unit, 1);

            Assert.That(ok, Is.False);
            // 失败时不应覆盖执行实体既有数据
            Assert.That(unit.TargetEntityId, Is.EqualTo(200L));
        }

        [Test]
        public void CancelDispatch_ClearsState_And_SetsCancelled()
        {
            var entity = ecsNode.AddChild<EcsEntity>(e => { });
            var exec = UnitDispatcherSystem.Create(entity, 1);
            entity.AddComponent<DispatchStateComponent>(c =>
            {
                c.State = DispatchState.Dispatching;
                c.TargetEntityId = 55;
                c.RemainingTimeout = 12f;
            });

            UnitDispatcherSystem.CancelDispatch(exec);

            var state = entity.GetComponent<DispatchStateComponent>();
            Assert.That(state.State, Is.EqualTo(DispatchState.Cancelled));
            Assert.That(state.TargetEntityId, Is.EqualTo(0));
            Assert.That(state.RemainingTimeout, Is.EqualTo(0f));
        }

        [Test]
        public void CompleteDispatch_SetsCompleted_OnlyWhenDispatching()
        {
            var entity = ecsNode.AddChild<EcsEntity>(e => { });
            var exec2 = UnitDispatcherSystem.Create(entity, 1);
            entity.AddComponent<DispatchStateComponent>(c =>
            {
                c.State = DispatchState.Dispatching;
                c.TargetEntityId = 77;
                c.RemainingTimeout = 3f;
            });

            UnitDispatcherSystem.CompleteDispatch(exec2);

            var state = entity.GetComponent<DispatchStateComponent>();
            Assert.That(state.State, Is.EqualTo(DispatchState.Completed));
            Assert.That(state.TargetEntityId, Is.EqualTo(0));
            Assert.That(state.RemainingTimeout, Is.EqualTo(0f));
        }

        [Test]
        public void Tick_TriggersTimeout_WhenRemainingTimeoutExpires()
        {
            var entity = ecsNode.AddChild<EcsEntity>(e => { });
            UnitDispatcherSystem.Create(entity, 1);
            entity.AddComponent<DispatchStateComponent>(c =>
            {
                c.State = DispatchState.Dispatching;
                c.TargetEntityId = 88;
                c.RemainingTimeout = 1.0f;
            });

            DispatchStateSystem.Tick(entity, 1.5f);

            var state = entity.GetComponent<DispatchStateComponent>();
            Assert.That(state.State, Is.EqualTo(DispatchState.Timeout));
            Assert.That(state.RemainingTimeout, Is.LessThanOrEqualTo(0f));
        }

        [Test]
        public void CanDispatch_ReturnsFalse_ForNonPositiveCount()
        {
            var entity = ecsNode.AddChild<EcsEntity>(e => { });
            entity.AddComponent<DispatchRuleComponent>(c => { c.ConfigId = 1; });

            Assert.That(DispatchStateSystem.CanDispatch(entity, 0), Is.False);
            Assert.That(DispatchStateSystem.CanDispatch(entity, -1), Is.False);
            Assert.That(DispatchStateSystem.CanDispatch(entity, 1), Is.True);
        }
    }
}
