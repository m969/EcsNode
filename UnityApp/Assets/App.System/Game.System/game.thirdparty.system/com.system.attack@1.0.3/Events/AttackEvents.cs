using ECS;
using System;

namespace ECSGame.AttackModule
{
    // 事件派发接口：攻击开始
    public interface IOnAttackStart : IDispatch
    {
        void OnAttackStart(EcsEntity owner, AttackAction action);
    }

    // 事件派发接口：命中
    public interface IOnAttackHit : IDispatch
    {
        void OnAttackHit(EcsEntity owner, AttackAction action, long attackerId, long targetId, int finalDamage);
    }

    // 事件派发接口：取消
    public interface IOnAttackCancel : IDispatch
    {
        void OnAttackCancel(EcsEntity owner, AttackAction action, AttackCancelReason reason);
    }

    // 事件派发接口：结束
    public interface IOnAttackEnd : IDispatch
    {
        void OnAttackEnd(EcsEntity owner, AttackAction action);
    }
}
