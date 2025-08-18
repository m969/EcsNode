using ECS;

namespace ECSGame.Module.Building
{
    /// <summary>
    /// 开始建造事件
    /// </summary>
    public interface IOnStartBuild : IDispatch
    {
        void OnStartBuild(EcsEntity entity, int targetLevel);
    }

    /// <summary>
    /// 开始升级事件
    /// </summary>
    public interface IOnUpgradeBuild : IDispatch
    {
        void OnUpgradeBuild(EcsEntity entity, int targetLevel);
    }

    /// <summary>
    /// 取消建造/升级事件
    /// </summary>
    public interface IOnCancelBuild : IDispatch
    {
        void OnCancelBuild(EcsEntity entity);
    }

    /// <summary>
    /// 拆除事件
    /// </summary>
    public interface IOnRemoveBuild : IDispatch
    {
        void OnRemoveBuild(EcsEntity entity);
    }

    /// <summary>
    /// 建造/升级完成事件
    /// </summary>
    public interface IOnBuildComplete : IDispatch
    {
        void OnBuildComplete(EcsEntity entity);
    }

    /// <summary>
    /// 状态心跳事件（每帧或固定间隔）
    /// </summary>
    public interface IOnBuildingStateTick : IDispatch
    {
        void OnBuildingStateTick(EcsEntity entity, float deltaTime);
    }
}
