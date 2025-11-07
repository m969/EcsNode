using ECS;
using System;

namespace ECSGame.ResourceDataModule
{
    /// <summary>
    /// 资源变更事件接口（增/减均触发；批量逐条触发）。
    /// </summary>
    public interface IResourceChanged : IDispatch
    {
        void OnResourceChanged(EcsEntity entity, ResourceType type, int delta, int newValue, ResourceChangeType changeType, string reason);
    }

    /// <summary>
    /// 资源同步完成事件接口（加载或持久化写回完成）。
    /// </summary>
    public interface IResourceSynced : IDispatch
    {
        void OnResourceSynced(EcsEntity entity, DateTime syncTime);
    }
}
