using ECS;

namespace ECSGame.AchieveModule
{
    /// <summary>
    /// 达成完成事件（对外订阅）
    /// </summary>
    public interface IOnAchieveCompleted : IDispatch
    {
        void OnAchieveCompleted(EcsEntity entity, AchieveItem item);
    }
}
