using ECS;
using System.Collections.Generic;

namespace ECSGame.ActorStateModule
{
    /// <summary>
    /// 角色状态系统
    /// </summary>
    public class ActorStateSystem : AComponentSystem<EcsEntity, ActorStateComponent>
    {
        /// <summary>
        /// 请求改变状态
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="request">请求</param>
        public static void RequestChangeState(EcsEntity entity, ChangeStateRequest request)
        {
            var component = entity.GetComponent<ActorStateComponent>();
            if (component == null) return;

            bool isActive = component.CurrentStates.ContainsKey(request.StateType);

            if (request.Enable)
            {
                if (isActive && !request.Force) return;

                if (isActive && request.Force)
                {
                    ExitState(entity, component, request.StateType);
                }
                
                EnterState(entity, component, request.StateType);
            }
            else
            {
                if (!isActive && !request.Force) return;
                
                ExitState(entity, component, request.StateType);
            }
        }

        private static void EnterState(EcsEntity entity, ActorStateComponent stateComponent, ActorStateType stateType)
        {
            EcsComponent? specificComponent = null;

            switch (stateType)
            {
                case ActorStateType.Alive:
                    entity.AddComponent<ActorAliveStateComponent>();
                    specificComponent = entity.GetComponent<ActorAliveStateComponent>();
                    ActorAliveStateSystem.OnEnter(entity);
                    break;
                case ActorStateType.Death:
                    entity.AddComponent<ActorDeathStateComponent>();
                    specificComponent = entity.GetComponent<ActorDeathStateComponent>();
                    ActorDeathStateSystem.OnEnter(entity);
                    break;
                case ActorStateType.Stunned:
                    entity.AddComponent<ActorStunnedStateComponent>();
                    specificComponent = entity.GetComponent<ActorStunnedStateComponent>();
                    ActorStunnedStateSystem.OnEnter(entity);
                    break;
            }

            if (specificComponent != null)
            {
                stateComponent.CurrentStates[stateType] = specificComponent;
            }
            
            entity.Dispatch<IStateEnterHandler>(x => x.OnStateEnterHandle(entity, stateType));
        }

        private static void ExitState(EcsEntity entity, ActorStateComponent stateComponent, ActorStateType stateType)
        {
            if (!stateComponent.CurrentStates.ContainsKey(stateType)) return;

            switch (stateType)
            {
                case ActorStateType.Alive:
                    ActorAliveStateSystem.OnExit(entity);
                    entity.RemoveComponent<ActorAliveStateComponent>();
                    break;
                case ActorStateType.Death:
                    ActorDeathStateSystem.OnExit(entity);
                    entity.RemoveComponent<ActorDeathStateComponent>();
                    break;
                case ActorStateType.Stunned:
                    ActorStunnedStateSystem.OnExit(entity);
                    entity.RemoveComponent<ActorStunnedStateComponent>();
                    break;
            }

            stateComponent.CurrentStates.Remove(stateType);
            
            entity.Dispatch<IStateExitHandler>(x => x.OnStateExitHandle(entity, stateType));
        }

        /// <summary>
        /// 查询状态是否激活
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="stateType">状态类型</param>
        /// <returns>是否激活</returns>
        public static bool IsStateActive(EcsEntity entity, ActorStateType stateType)
        {
            var component = entity.GetComponent<ActorStateComponent>();
            if (component == null) return false;
            return component.CurrentStates.ContainsKey(stateType);
        }

        /// <summary>
        /// 设置默认状态
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="stateType">状态类型</param>
        public static void SetDefaultState(EcsEntity entity, ActorStateType stateType)
        {
            var component = entity.GetComponent<ActorStateComponent>();
            if (component == null) return;
            component.DefaultState = stateType;
            
            RequestChangeState(entity, new ChangeStateRequest { StateType = stateType, Enable = true, Force = true });
        }

        /// <summary>
        /// 每帧更新状态逻辑
        /// </summary>
        /// <param name="entity">实体</param>
        public static void OnUpdate(EcsEntity entity)
        {
            var component = entity.GetComponent<ActorStateComponent>();
            if (component == null) return;

            var keys = new List<ActorStateType>(component.CurrentStates.Keys);
            foreach (var stateType in keys)
            {
                switch (stateType)
                {
                    case ActorStateType.Alive:
                        ActorAliveStateSystem.OnUpdate(entity);
                        break;
                    case ActorStateType.Death:
                        break;
                    case ActorStateType.Stunned:
                        ActorStunnedStateSystem.OnUpdate(entity);
                        break;
                }
            }
            
            entity.Dispatch<IStateUpdateHandler>(x => x.OnStateUpdateHandle(entity));
        }
    }
}
