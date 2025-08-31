using ECS;
using ECSGame.Module.Building;
using ECSUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace ECSGame
{
    public class ActorViewSystem : AEntitySystem<Actor>,
        IAwake<Actor>,
        IInit<Actor>,
        IAfterInit<Actor>,
        IUpdate<Actor>
    {
        public void Awake(Actor entity)
        {
        }

        public void Init(Actor entity)
        {

        }

        public void AfterInit(Actor buildingEntity)
        {
            CreateView(buildingEntity);
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
                var modelObj = GameObject.Instantiate(Resources.Load<GameObject>("Hero"));
                ModelViewSystem.SetModel(entity, modelObj);
            }
        }

        public void Update(Actor entity)
        {
            EntityViewSystem.Update(entity);
        }
    }
}
