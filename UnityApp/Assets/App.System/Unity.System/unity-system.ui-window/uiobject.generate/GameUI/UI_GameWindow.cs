/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace GameUI
{
    public partial class UI_GameWindow : ECSUnity.UIPanel
    {
        public GButton m_nTopAvatar;
        public GButton m_nTopResources;
        public GButton m_nTopMail;
        public GButton m_nTopActivity;
        public GButton m_nTopShop;
        public GButton m_nTopSettings;
        public GGraph m_nLeftPanelRect;
        public GList m_nLeftMenu;
        public GGraph m_nRightPanelRect;
        public GList m_nRightMenu;
        public GList m_nBottomMenu;
        public GButton m_nBuildBtn;
        public GButton m_nDispatchBtn;
        public const string URL = "ui://m4ix6whfonq40";

        public static UI_GameWindow CreateInstance()
        {
            return (UI_GameWindow)UIPackage.CreateObject("GameUI", "GameWindow");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_nTopAvatar = (GButton)GetChildAt(0);
            m_nTopResources = (GButton)GetChildAt(1);
            m_nTopMail = (GButton)GetChildAt(2);
            m_nTopActivity = (GButton)GetChildAt(3);
            m_nTopShop = (GButton)GetChildAt(4);
            m_nTopSettings = (GButton)GetChildAt(5);
            m_nLeftPanelRect = (GGraph)GetChildAt(6);
            m_nLeftMenu = (GList)GetChildAt(7);
            m_nRightPanelRect = (GGraph)GetChildAt(8);
            m_nRightMenu = (GList)GetChildAt(9);
            m_nBottomMenu = (GList)GetChildAt(10);
            m_nBuildBtn = (GButton)GetChildAt(11);
            m_nDispatchBtn = (GButton)GetChildAt(12);
        }
    }
}