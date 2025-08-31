using ECS;
using ECSGame.Module.GridBased;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ECSGame
{
    public class GridPlaneSelectionSystem : AComponentSystem<GridPlane, GridPlaneSelectionComponent>,
        IAwake<GridPlane, GridPlaneSelectionComponent>,
        IInit<GridPlane, GridPlaneSelectionComponent>
    {
        public void Awake(GridPlane gridPlane, GridPlaneSelectionComponent component)
        {
        }

        public void Init(GridPlane gridPlane, GridPlaneSelectionComponent component)
        {
        }

        public static void SetSelectCell(GridPlane gridPlane, long selectionCell)
        {
            gridPlane.GetComponent<GridPlaneSelectionComponent>().SelectCellId = selectionCell;
        }

        public static long GetSelectCell(GridPlane gridPlane)
        {
            return gridPlane.GetComponent<GridPlaneSelectionComponent>().SelectCellId;
        }
    }
}
