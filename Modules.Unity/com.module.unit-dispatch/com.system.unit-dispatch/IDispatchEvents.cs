using ECS;
using System;

namespace ECSGame.UnitDispatchModule
{
    /// <summary>
    /// 开始派遣事件。通过 <c>entity.Dispatch&lt;IOnDispatchStarted&gt;</c> 调用派发。
    /// </summary>
    public interface IOnDispatchStarted : IDispatch
    {
        /// <summary>
        /// 派遣开始时回调。
        /// </summary>
        /// <param name="entity">派遣实体。</param>
        /// <param name="count">本次新增派遣数量。</param>
        void OnDispatchStarted(EcsEntity entity, int count);
    }

    /// <summary>
    /// 取消派遣事件。通过 <c>entity.Dispatch&lt;IOnDispatchCancelled&gt;</c> 调用派发。
    /// </summary>
    public interface IOnDispatchCancelled : IDispatch
    {
        /// <summary>
        /// 派遣被取消时回调。
        /// </summary>
        /// <param name="entity">派遣实体。</param>
        void OnDispatchCancelled(EcsEntity entity);
    }

    /// <summary>
    /// 派遣完成事件。通过 <c>entity.Dispatch&lt;IOnDispatchCompleted&gt;</c> 调用派发。
    /// </summary>
    public interface IOnDispatchCompleted : IDispatch
    {
        /// <summary>
        /// 派遣完成时回调。
        /// </summary>
        /// <param name="entity">派遣实体。</param>
        void OnDispatchCompleted(EcsEntity entity);
    }

    /// <summary>
    /// 派遣超时事件。通过 <c>entity.Dispatch&lt;IOnDispatchTimeout&gt;</c> 调用派发。
    /// </summary>
    public interface IOnDispatchTimeout : IDispatch
    {
        /// <summary>
        /// 派遣超时时回调。
        /// </summary>
        /// <param name="entity">派遣实体。</param>
        void OnDispatchTimeout(EcsEntity entity);
    }
}
