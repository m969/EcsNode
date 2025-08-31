/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace LoginUI
{
    public partial class UI_LoginMenuWindow : ECSUnity.UIPanel
    {
        public GTextField m_nFrame1;
        public GTextField m_nFrame2;
        public GButton m_nBtn;
        public const string URL = "ui://g9o3wgyhpggt0";

        public static UI_LoginMenuWindow CreateInstance()
        {
            return (UI_LoginMenuWindow)UIPackage.CreateObject("LoginUI", "LoginMenuWindow");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_nFrame1 = (GTextField)GetChildAt(0);
            m_nFrame2 = (GTextField)GetChildAt(1);
            m_nBtn = (GButton)GetChildAt(2);
        }
    }
}