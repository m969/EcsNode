using ECS;
using ECSGame.ActorStateModule;
using ECSGame.Module.Building;
using ECSUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSGame
{
    public class ActorViewSystem : AEntitySystem<Actor>,
        IAwake<Actor>,
        IInit<Actor>,
        IAfterInit<Actor>,
        IUpdate<Actor>,
        IStateEnterHandler
    {
        public void Awake(Actor entity)
        {
            entity.AddComponent<HudUIComponent>();
        }

        public void Init(Actor entity)
        {

        }

        public void AfterInit(Actor entity)
        {
            CreateView(entity);
        }

        public static void CreateView(Actor entity)
        {
            if (AppStatic.GameType == GameType.TrueGameDemo)
            {
                var modelObj = GameObject.Instantiate(Resources.Load<GameObject>("CubeActor"));
                ModelViewSystem.SetModel(entity, modelObj);
            }
            if (AppStatic.GameType == GameType.SimulationGameDemo)
            {
                if (entity.Type == ActorType.Hero)
                {
                    var modelObj = GameObject.Instantiate(Resources.Load<GameObject>("Hero"));
                    ModelViewSystem.SetModel(entity, modelObj);
                }
                if (entity.Type == ActorType.Monster)
                {
                    var modelObj = GameObject.Instantiate(Resources.Load<GameObject>("Monster"));
                    ModelViewSystem.SetModel(entity, modelObj);
                }
            }
        }

        public void Update(Actor entity)
        {
            EntityViewSystem.Update(entity);
        }


        public static void PlayDieTween(EcsEntity entity)
        {
            var modelObj = ModelViewSystem.GetModel(entity);
            modelObj.transform.GetChild(0).GetComponent<MoveTween>().enabled = true;
        }

        public void OnStateEnterHandle(EcsEntity entity, ActorStateType stateType)
        {
            if (stateType == ActorStateType.Death)
            {
                // 处理角色死亡逻辑
                // 停止所有动作，播放死亡动画等
                AnimationSystem.Play(entity as Actor, AnimationState.Die);
                PlayDieTween(entity);
            }
        }
    }
}
