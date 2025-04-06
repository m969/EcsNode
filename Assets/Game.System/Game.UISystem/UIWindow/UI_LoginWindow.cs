using FairyGUI;
using FairyGUI.Utils;

namespace Login
{
    public partial class UI_LoginWindow
    {
        public void Awake()
        {
            m_nBtn.onClick.Add(OnClick);
        }

        public void OnClick()
        {
            ConsoleLog.Debug("UI_LoginWindow OnClick2");
        }
    }
}