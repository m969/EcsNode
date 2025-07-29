using ECSGame;
using ECSUnity;
using FairyGUI;
using FairyGUI.Utils;
using System.Reflection;

namespace Login
{
    public interface HomePage_BtnClickHandler
    {
        void OnBtnClick();
    }

    public partial class UI_HomePageWindow : IUIWindow,
        HomePage_BtnClickHandler
    {
        public void ButtonClickEventBind(UI_HomePageWindow window)
        {
            if (window is HomePage_BtnClickHandler handler)
            {
                window.m_nBtn.onClick.Set(handler.OnBtnClick);
            }
        }

        public void Awake()
        {
            ButtonClickEventBind(this);
        }

        public void OnBtnClick()
        {

        }

        public void OnHide()
        {
        }

        public void OnShow()
        {
        }

        public void GameUIRefill(TrueGame game)
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