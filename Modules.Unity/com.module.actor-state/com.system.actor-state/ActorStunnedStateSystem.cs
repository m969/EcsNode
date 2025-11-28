using ECS;

namespace ECSGame.ActorStateModule
{
    /// <summary>
    /// 角色眩晕状态系统
    /// </summary>
    public class ActorStunnedStateSystem : AComponentSystem<EcsEntity, ActorStunnedStateComponent>
    {
        /// <summary>
        /// 进入眩晕状态逻辑
        /// </summary>
        /// <param name="entity">实体</param>
        public static void OnEnter(EcsEntity entity)
        {
            // 逻辑
        }

        /// <summary>
        /// 退出眩晕状态逻辑
        /// </summary>
        /// <param name="entity">实体</param>
        public static void OnExit(EcsEntity entity)
        {
            // 逻辑
        }

        /// <summary>
        /// 眩晕状态持续逻辑（如倒计时）
        /// </summary>
        /// <param name="entity">实体</param>
        public static void OnUpdate(EcsEntity entity)
        {
            // 逻辑
        }
    }
}
