using ECS.Fody;
using ECSGame;
using ECSUnity;
using FairyGUI;
using FairyGUI.Utils;
using System.Reflection;

namespace Login
{
    public partial class UI_HomePageWindow : IUIWindow
    {
        public void Awake()
        {
            UIWindowBindSystem.BindClickEvent(this);
            m_nBuildingBtn.onDragStart.Add((e) =>
            {
                ConsoleLog.Debug("BuildingBtn onDragStart");
            });
            m_nBuildingBtn.onDragEnd.Add((e) =>
            {
                ConsoleLog.Debug("BuildingBtn onDragEnd");
            });
            m_nBuildingBtn.onDragMove.Add((e) =>
            {
                ConsoleLog.Debug("BuildingBtn onDragMove");
            });
        }

        [AfterClick(nameof(m_nBuildingBtn))]
        public void BuildingBtnClicked(EventContext eventContext)
        {
            ConsoleLog.Debug("BuildingBtnClicked");
        }

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