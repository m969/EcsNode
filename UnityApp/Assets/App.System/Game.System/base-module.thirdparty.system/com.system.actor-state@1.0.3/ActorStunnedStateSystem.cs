using ECS;

namespace ECSGame.ActorStateModule
{
    /// <summary>
    /// 角色眩晕状态系统
    /// </summary>
    public class ActorStunnedStateSystem : AComponentSystem<EcsEntity, ActorStunnedState>, IEnterHandler, IExitHandler
    {
        /// <summary>
        /// 进入眩晕状态逻辑
        /// </summary>
        /// <param name="component">组件</param>
        public void OnEnter(EcsComponent component)
        {
            var entity = component.Entity;
            // 逻辑
        }

        /// <summary>
        /// 退出眩晕状态逻辑
        /// </summary>
        /// <param name="component">组件</param>
        public void OnExit(EcsComponent component)
        {
            var entity = component.Entity;
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
