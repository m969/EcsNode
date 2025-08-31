using ECS;
using ECSGame;
using ECSGame.Module.Building;
using ECSGame.Module.GridBased;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

namespace ECSUnity
{
    public class GridCellViewSystem : AEntitySystem<GridCell>,
        IAwake<GridCell>,
        IInit<GridCell>,
        IOnGridCellClick,
        IOnGridCellEnter,
        IOnGridCellExit,
        IOnGridCellUp
    {
        public void Awake(GridCell gridCell)
        {
        }

        public void Init(GridCell gridCell)
        {

        }

        public void OnGridCellClick(EcsEntity entity)
        {
            //entity.As<GridCell>().State = GridCellState.Selected;
            var gridPlane = entity.GetParent<GridPlane>();
            GridPlaneSelectionSystem.SetSelectCell(gridPlane, entity.Id);
        }

        public void OnGridCellEnter(EcsEntity entity)
        {
            if (IsSelected(entity.As<GridCell>()))
            {
                return;
            }
            var modelObj = ModelViewSystem.GetModel(entity);
            var color = modelObj.GetComponent<Image>().color;
            modelObj.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.5f);
        }

        public void OnGridCellExit(EcsEntity entity)
        {
            if (IsSelected(entity.As<GridCell>()))
            {
                return;
            }
            var modelObj = ModelViewSystem.GetModel(entity);
            var color = modelObj.GetComponent<Image>().color;
            modelObj.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0);
        }

        public void OnGridCellUp(EcsEntity entity)
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

        public static bool IsSelected(GridCell gridCell)
        {
            return GridPlaneSelectionSystem.GetSelectCell(gridCell.GetParent<GridPlane>()) == gridCell.Id;
        }

        public static void SetSelected(GridCell gridCell, bool selected)
        {
            var modelObj = ModelViewSystem.GetModel(gridCell);
            var color = modelObj.GetComponent<Image>().color;
            if (selected)
            {
                modelObj.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 1f);
            }
            else
            {
                modelObj.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0);
            }
        }

    }
}
