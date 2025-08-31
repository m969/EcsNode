using FairyGUI;
using FairyGUI.Utils;
using ECSUnity;

namespace LoginUI
{
    public partial class UI_LoginMenuWindow : IUIWindow
    {
        public void Awake()
        {
            m_nBtn.onClick.Add(OnClick);
        }

        public void OnClick()
        {
            ConsoleLog.Debug("UI_LoginMenuWindow OnClick2");
        }

        public void OnShow()
        {
        }

        public void OnHide()
        {
        }
    }
}