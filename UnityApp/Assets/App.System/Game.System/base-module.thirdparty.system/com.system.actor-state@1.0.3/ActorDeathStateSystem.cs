using ECS;

namespace ECSGame.ActorStateModule
{
    /// <summary>
    /// 角色死亡状态系统
    /// </summary>
    public class ActorDeathStateSystem : AComponentSystem<EcsEntity, ActorDeathState>, IEnterHandler, IExitHandler
    {
        /// <summary>
        /// 进入死亡状态逻辑
        /// </summary>
        /// <param name="component">组件</param>
        public void OnEnter(EcsComponent component)
        {
            var entity = component.Entity;
            // 逻辑
        }

        /// <summary>
        /// 退出死亡状态逻辑
        /// </summary>
        /// <param name="component">组件</param>
        public void OnExit(EcsComponent component)
        {
            var entity = component.Entity;
            // 逻辑
        }
    }
}
