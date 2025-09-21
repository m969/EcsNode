using ECS;
using ECS.Fody;
using ECSGame;
using ECSGame.Module.Building;
using ECSGame.Module.GridBased;
using ECSGame.UnitDispatchModule;
using ECSUnity;
using FairyGUI;
using UnityEngine;

namespace GameUI
{
    public partial class UI_GameWindow : IUIWindow
    {
        public long CurrentBuildingId = 0;
        public long CurrentDispatcherId = 0;

        public void Awake()
        {
            UIWindowHelper.BindClickEvent(this);

            m_nLeftMenu.onClickItem.Set((context) =>
            {
                //ConsoleLog.Debug("LeftMenu Clicked: " + ((GObject)context.data).name);
                var index = m_nLeftMenu.GetChildIndex((GObject)context.data);
                if (index == 4)
                {

                }
            });
        }

        public void OnHide()
        {
        }

        public void OnShow()
        {
        }

        [AfterClick(nameof(m_nBuildBtn))]
        public void Build(EventContext eventContext)
        {
            ConsoleLog.Debug("Build Clicked");
            var world = EcsDomain.World;
            var gridPlane = GridPlaneListSystem.GetGridPlaneByConfigId(world, 1001);
            var selectedCellId = GridPlaneSelectionSystem.GetSelectCell(gridPlane);
            var gridCell = GridCellListSystem.GetCellById(gridPlane, selectedCellId);
            var position = new ECSGame.Module.Building.Vector2Int(gridCell.X, gridCell.Y);
            var building = BuildingSystem.Create(world, 1, position, EcsDomain.Player.Id);
            building.AddComponent<TransformComponent>();
            building.AddComponent<UnitDispatcherListComponent>();
            TransformSystem.ChangePosition(building, new TrueSync.TSVector(position.x, 0, position.y));
            building.Init();
            CurrentBuildingId = building.Id;

            // 创建英雄单位派遣实体
            var heroDispatcher = UnitDispatcherSystem.Create(building, 1002);
            heroDispatcher.Timeout = Time.time + 5f; // 5秒后超时
            heroDispatcher.AddComponent<DispatchRuleComponent>();
            heroDispatcher.AddComponent<DispatchStateComponent>();
            UnitDispatcherListSystem.AddDispatcher(building, heroDispatcher);
            //UnitDispatcherSystem.StartDispatch(heroDispatcher, 1);
            CurrentDispatcherId = heroDispatcher.Id;
        }

        [AfterClick(nameof(m_nDispatchBtn))]
        public void StartDispatch(EventContext eventContext)
        {
            ConsoleLog.Debug("StartDispatch Clicked");
            var world = EcsDomain.World;
            var building = world.GetChild<BuildingEntity>(CurrentBuildingId);
            var heroDispatcher = UnitDispatcherListSystem.GetDispatcher(building, CurrentDispatcherId) as UnitDispatcher;
            UnitDispatcherSystem.StartDispatch(heroDispatcher, 1);
        }
    }
}