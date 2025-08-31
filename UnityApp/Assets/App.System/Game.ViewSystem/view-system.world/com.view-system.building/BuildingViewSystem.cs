using ECS;
using ECSGame;
using ECSGame.Module.Building;
using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;
using UnityEngine.UI;

namespace ECSUnity
{
    public class BuildingViewSystem : AEntitySystem<BuildingEntity>,
        IAwake<BuildingEntity>,
        IInit<BuildingEntity>,
        IAfterInit<BuildingEntity>
    {
        public void Awake(BuildingEntity buildingEntity)
        {
        }

        public void Init(BuildingEntity buildingEntity)
        {

        }

        public void AfterInit(BuildingEntity buildingEntity)
        {
            CreateView(buildingEntity);
        }

        public static void CreateView(BuildingEntity buildingEntity)
        {
            var modelObj = GameObject.Instantiate(Resources.Load<GameObject>("Building"));
            ModelViewSystem.SetModel(buildingEntity, modelObj);
        }
    }
}
