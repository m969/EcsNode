using ECS;
using ECS.Fody;
using ECSGame;
using ECSGame.Module.Building;
using ECSGame.Module.GridBased;
using ECSUnity;
using FairyGUI;
using FairyGUI.Utils;
using System.Reflection;

namespace LoginUI
{
    public partial class UI_HomePageWindow : IUIWindow
    {
        public void Awake()
        {
            UIWindowBindSystem.BindClickEvent(this);
        }

        //[AfterClick(nameof(m_nBuildingBtn))]
        //public void BuildingBtnClicked(EventContext eventContext)
        //{

        //}

        public void OnHide()
        {
        }

        public void OnShow()
        {
        }

        public void GameUIRefill(TrueWorld game)
        {
            m_nDetermineFrame.text = $"确定帧:{game.DetermineFrame}";
        }

        public void ActorUIRefill(Actor actor)
        {
            var actorPlay = actor.GetComponent<FramePlayComponent>();
            m_nAdvancedFrame.text = $"预测帧:{actorPlay.AlreadyPredictFrame}";
            m_nConflictFrame.text = $"最新冲突帧:{actorPlay.ConflictFrame}";
            m_nConflictFrameCount.text = $"冲突帧数量:{actorPlay.ConflictFrameCount}";
            m_nConflictPlay.text = $"冲突类型:{actorPlay.ConflictType}";
        }

        public void Update()
        {

        }
    }
}