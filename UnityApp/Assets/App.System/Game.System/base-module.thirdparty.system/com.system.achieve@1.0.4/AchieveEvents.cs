using ECS;

namespace ECSGame.AchieveModule
{
    /// <summary>
    /// 达成完成事件（对外订阅）
    /// </summary>
    public interface IAchieveCompletedHandler : IDispatch
    {
        void OnAchieveCompletedHandle(EcsEntity entity, AchieveItem item);
    }
}
