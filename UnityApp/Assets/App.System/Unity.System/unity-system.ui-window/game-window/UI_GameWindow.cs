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
            var startBuildEvent = new PlayerStartBuildEvent();
            EventBus.Send(startBuildEvent);
            CurrentBuildingId = startBuildEvent.BuildingId;
        }

        [AfterClick(nameof(m_nDispatchBtn))]
        public void StartDispatch(EventContext eventContext)
        {
            ConsoleLog.Debug("StartDispatch Clicked");
            EventBus.Send(new PlayerStartDispatchEvent(){ BuildingId = CurrentBuildingId });
        }
    }
}