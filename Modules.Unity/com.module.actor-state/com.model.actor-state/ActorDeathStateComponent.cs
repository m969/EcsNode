using ECS;

namespace ECSGame.ActorStateModule
{
    /// <summary>
    /// 角色死亡状态组件
    /// 标记角色处于死亡状态，存储死亡状态相关数据。
    /// </summary>
    public class ActorDeathStateComponent : EcsComponent
    {
        /// <summary>
        /// 死亡时刻的时间戳。
        /// </summary>
        public float DeathTime;
    }
}
