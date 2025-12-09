using ECS;

namespace ECSGame.ActorStateModule
{
    /// <summary>
    /// 角色存活状态系统
    /// </summary>
    public class ActorAliveStateSystem : AComponentSystem<EcsEntity, ActorAliveState>, IEnterHandler, IExitHandler
    {
        /// <summary>
        /// 进入存活状态逻辑
        /// </summary>
        /// <param name="component">组件</param>
        public void OnEnter(EcsComponent component)
        {
            var entity = component.Entity;
            // 逻辑
        }

        /// <summary>
        /// 退出存活状态逻辑
        /// </summary>
        /// <param name="component">组件</param>
        public void OnExit(EcsComponent component)
        {
            var entity = component.Entity;
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
