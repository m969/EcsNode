using ECS;

namespace ECSGame.ActorStateModule
{
    /// <summary>
    /// 角色存活状态系统
    /// </summary>
    public class ActorAliveStateSystem : AComponentSystem<EcsEntity, ActorAliveStateComponent>
    {
        /// <summary>
        /// 进入存活状态逻辑
        /// </summary>
        /// <param name="entity">实体</param>
        public static void OnEnter(EcsEntity entity)
        {
            // 逻辑
        }

        /// <summary>
        /// 退出存活状态逻辑
        /// </summary>
        /// <param name="entity">实体</param>
        public static void OnExit(EcsEntity entity)
        {
            // 逻辑
        }

        /// <summary>
        /// 存活状态持续逻辑
        /// </summary>
        /// <param name="entity">实体</param>
        public static void OnUpdate(EcsEntity entity)
        {
            // 逻辑
        }
    }
}
