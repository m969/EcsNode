using ECS;
using ECSGame.Module.GridBased;
using ECSGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ECSUnity
{
    public class GridCellViewSystem : AEntitySystem<GridCell>,
        IAwake<GridCell>,
        IInit<GridCell>,
        IOnGridCellClick,
        IOnGridCellEnter,
        IOnGridCellExit
    {
        public void Awake(GridCell gridCell)
        {
        }

        public void Init(GridCell gridCell)
        {

        }

        public static void Create(GridPlane gridCell)
        {
            //var modelObj = GameObject.Instantiate(Resources.Load<GameObject>("GridPlaneCanvas"));
            //ModelViewSystem.SetModelObj(entity, modelObj);

            //modelObj.transform.parent = GameObject.Find("/Scene/PlaneCanvasGroup").transform;
            //modelObj.transform.localPosition = Vector3.zero;
        }

        public static void Update(GridCell gridCell)
        {
            EntityViewSystem.Update(gridCell);
        }

        public void OnGridCellClick(EcsEntity entity)
        {
            ConsoleLog.Debug(entity.Id + " OnGridCellClick.");
        }

        public void OnGridCellEnter(EcsEntity entity)
        {
            var modelObj = ModelViewSystem.GetModel(entity);
            var color = modelObj.GetComponent<Image>().color;
            modelObj.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.5f);
        }

        public void OnGridCellExit(EcsEntity entity)
        {
            var modelObj = ModelViewSystem.GetModel(entity);
            var color = modelObj.GetComponent<Image>().color;
            modelObj.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0);
        }
    }
}
