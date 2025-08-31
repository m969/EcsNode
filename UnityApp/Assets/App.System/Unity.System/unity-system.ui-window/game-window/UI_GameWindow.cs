using ECS;
using ECSGame;
using ECSGame.Module.Building;
using ECSGame.Module.GridBased;
using ECSUnity;
using FairyGUI;

namespace GameUI
{
    public partial class UI_GameWindow : IUIWindow
    {
        public void Awake()
        {
            m_nLeftMenu.onClickItem.Set((context) =>
            {
                //ConsoleLog.Debug("LeftMenu Clicked: " + ((GObject)context.data).name);
                var index = m_nLeftMenu.GetChildIndex((GObject)context.data);
                if (index == 4)
                {
                    var world = EcsDomain.World;
                    var gridPlane = GridPlaneListSystem.GetGridPlaneByConfigId(world, 1001);
                    var selectedCellId = GridPlaneSelectionSystem.GetSelectCell(gridPlane);
                    var gridCell = GridCellListSystem.GetCellById(gridPlane, selectedCellId);
                    var position = new Vector2Int(gridCell.X, gridCell.Y);
                    var building = BuildingSystem.Create(world, 1, position, EcsDomain.Player.Id);
                    building.AddComponent<TransformComponent>();
                    TransformSystem.ChangePosition(building, new TrueSync.TSVector(position.x, 0, position.y));
                    building.Init();
                }
            });
        }

        public void OnHide()
        {
        }

        public void OnShow()
        {
        }


    }
}