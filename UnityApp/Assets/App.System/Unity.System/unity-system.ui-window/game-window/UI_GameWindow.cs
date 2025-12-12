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
            var building = WorldSystem.CreateBuildingFromGrid(world);
            CurrentBuildingId = building.Id;

            // 创建英雄单位派遣实体
            // var heroDispatcher = UnitDispatcherSystem.Create(building, 1002);
            // heroDispatcher.Timeout = Time.time + 5f; // 5秒后超时
            // heroDispatcher.AddComponent<DispatchRuleComponent>();
            // heroDispatcher.AddComponent<DispatchStateComponent>();
            // UnitDispatcherListSystem.AddDispatcher(building, heroDispatcher);
            // CurrentDispatcherId = heroDispatcher.Id;
        }

        [AfterClick(nameof(m_nDispatchBtn))]
        public void StartDispatch(EventContext eventContext)
        {
            ConsoleLog.Debug("StartDispatch Clicked");
            var world = EcsDomain.World;
            var building = world.GetChild<BuildingEntity>(CurrentBuildingId);
            DispatchAgentSystem.StartDispatch(building, 1002, 1, 5f);
            // var heroDispatcher = UnitDispatcherListSystem.GetDispatcher(building, CurrentDispatcherId) as UnitDispatcher;
            // UnitDispatcherSystem.StartDispatch(heroDispatcher, 1);
        }
    }
}