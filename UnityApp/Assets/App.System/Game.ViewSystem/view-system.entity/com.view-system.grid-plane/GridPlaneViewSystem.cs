using ECS;
using ECSGame;
using ECSGame.Module.GridBased;
using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;
using UnityEngine.UI;

namespace ECSUnity
{
    public class GridPlaneViewSystem : AEntitySystem<GridPlane>,
        IAwake<GridPlane>,
        IInit<GridPlane>,
        IAfterInit<GridPlane>
    {
        public void Awake(GridPlane gridPlane)
        {
        }

        public void Init(GridPlane gridPlane)
        {

        }

        public void AfterInit(GridPlane gridPlane)
        {
            Create(gridPlane);
        }

        public static void Create(GridPlane gridPlane)
        {
            var modelObj = GameObject.Instantiate(Resources.Load<GameObject>("GridPlaneCanvas"));
            ModelViewSystem.SetModel(gridPlane, modelObj);

            modelObj.GetComponent<Canvas>().worldCamera = Camera.main;

            modelObj.transform.parent = GameObject.Find("/Scene/PlaneCanvasGroup").transform;
            modelObj.transform.localPosition = Vector3.zero;

            var gridPlaneTrans = modelObj.transform.Find("GridPlane");
            var cellObjPrefab = modelObj.transform.Find("GridPlane/GridRect");
            cellObjPrefab.gameObject.SetActive(false);
            var cellList = GridCellListSystem.GetEmptyCells(gridPlane);
            foreach (var gridCell in cellList)
            {
                gridCell.AddComponent<TransformComponent>();
                var cellPosition = gridPlaneTrans.TransformPoint(new Vector3(gridCell.X * 10, gridCell.Y * 10, 0)).ToTSVector();
                TransformSystem.ChangePosition(gridCell, cellPosition);
                var cellClone = GameObject.Instantiate(cellObjPrefab.gameObject, gridPlaneTrans);
                cellClone.SetActive(true);
                cellClone.name = $"Cell_{gridCell.X}_{gridCell.Y}";
                cellClone.GetComponent<GridCellControl>().GridCellEntity = gridCell;
                var color = cellClone.GetComponent<Image>().color;
                cellClone.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0);
                ModelViewSystem.SetModel(gridCell, cellClone);
            }
        }

        public static void Update(GridPlane gridPlane)
        {
            EntityViewSystem.Update(gridPlane);
        }
    }
}
