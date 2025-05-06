using FairyGUI;
using FairyGUI.Utils;
using ECSUnity;
using ECSGame;

namespace Login
{
    public partial class UI_HomePageWindow : IUIWindow
    {
        public TrueGame TrueGame { get; set; }

        public void Awake()
        {
            m_nBtn.onClick.Add(OnClick);
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