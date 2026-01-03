/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace MainUI
{
    public partial class UI_MainWindow : ECSUnity.UIPanel
    {
        public GGraph m_nLeftPanelRect;
        public GGraph m_nRightPanelRect;
        public GButton m_nBattleBtn;
        public const string URL = "ui://m4ix6whfonq40";

        public static UI_MainWindow CreateInstance()
        {
            return (UI_MainWindow)UIPackage.CreateObject("MainUI", "MainWindow");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_nLeftPanelRect = (GGraph)GetChildAt(0);
            m_nRightPanelRect = (GGraph)GetChildAt(1);
            m_nBattleBtn = (GButton)GetChildAt(2);
        }
    }
}