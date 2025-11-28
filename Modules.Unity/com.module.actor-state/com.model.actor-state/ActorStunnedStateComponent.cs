using ECS;

namespace ECSGame.ActorStateModule
{
    /// <summary>
    /// 角色眩晕状态组件
    /// 标记角色处于眩晕状态，存储眩晕状态相关数据。
    /// </summary>
    public class ActorStunnedStateComponent : EcsComponent
    {
        /// <summary>
        /// 眩晕持续时间。
        /// </summary>
        public float StunDuration;
        
        /// <summary>
        /// 眩晕开始时间。
        /// </summary>
        public float StunStartTime;
    }
}
