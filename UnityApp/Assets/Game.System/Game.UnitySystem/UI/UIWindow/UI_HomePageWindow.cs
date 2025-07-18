using ECSGame;
using ECSUnity;
using FairyGUI;
using FairyGUI.Utils;
using System.Reflection;

namespace Login
{
    public partial class UI_HomePageWindow : IUIWindow
    {
        private partial class UI_HomePageWindow_EventBinder
        {
            public UI_HomePageWindow Window {  get; set; }  

            public void SetBind(UI_HomePageWindow window)
            {
                Window = window;
                window.m_nBtn.onClick.Set(nBtn_clicked);
            }

            public partial void nBtn_clicked();
        }

        private partial class UI_HomePageWindow_EventBinder
        {
            public partial void nBtn_clicked()
            {
                Window.OnClick();
            }
        }

        private UI_HomePageWindow_EventBinder Binder { get; set; }
        public TrueGame TrueGame { get; set; }

        public void Awake()
        {
            Binder = new UI_HomePageWindow_EventBinder();
            Binder.SetBind(this);
        }

        public void OnClick()
        {
            //ConsoleLog.Debug("UI_HomePageWindow OnClick2");
        }

        public void OnHide()
        {
        }

        public void OnShow()
        {
        }

        public void Update()
        {
            m_nDetermineFrame.text = $"确定帧:{TrueGame.DetermineFrame}";
            if (TrueGame.OtherActor != null)
            {
                var actorPlay = TrueGame.OtherActor.GetComponent<FramePlayComponent>();
                m_nAdvancedFrame.text = $"预测帧:{actorPlay.AlreadyPredictFrame}";
                m_nConflictFrame.text = $"最新冲突帧:{actorPlay.ConflictFrame}";
                m_nConflictFrameCount.text = $"冲突帧数量:{actorPlay.ConflictFrameCount}";
                m_nConflictPlay.text = $"冲突类型:{actorPlay.ConflictType}";
            }
            else
            {
                m_nAdvancedFrame.text = string.Empty;
                m_nConflictFrame.text = string.Empty;
                m_nConflictFrameCount.text = string.Empty;
                m_nConflictPlay.text = string.Empty;
            }
        }
    }
}