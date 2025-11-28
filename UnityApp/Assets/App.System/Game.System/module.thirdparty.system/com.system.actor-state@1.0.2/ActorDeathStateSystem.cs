using ECS;

namespace ECSGame.ActorStateModule
{
    /// <summary>
    /// 角色死亡状态系统
    /// </summary>
    public class ActorDeathStateSystem : AComponentSystem<EcsEntity, ActorDeathStateComponent>
    {
        /// <summary>
        /// 进入死亡状态逻辑
        /// </summary>
        /// <param name="entity">实体</param>
        public static void OnEnter(EcsEntity entity)
        {
            // 逻辑
        }

        /// <summary>
        /// 退出死亡状态逻辑
        /// </summary>
        /// <param name="entity">实体</param>
        public static void OnExit(EcsEntity entity)
        {
            // 逻辑
        }
    }
}
