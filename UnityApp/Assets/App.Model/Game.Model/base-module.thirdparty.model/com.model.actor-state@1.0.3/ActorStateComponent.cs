using ECS;
using System.Collections.Generic;

namespace ECSGame.ActorStateModule
{
    /// <summary>
    /// 角色状态组件
    /// 用于管理角色的状态，记录当前激活的状态以及处理状态切换。
    /// </summary>
    public class ActorStateComponent : EcsComponent
    {
        /// <summary>
        /// 当前激活的状态字典，Key为状态类型，Value为对应的状态数据组件（若有）。
        /// </summary>
        public Dictionary<ActorStateType, EcsComponent> CurrentStates = new Dictionary<ActorStateType, EcsComponent>();
        
        /// <summary>
        /// 默认状态，初始化时进入。
        /// </summary>
        public ActorStateType DefaultState;
    }
}
