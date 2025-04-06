/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Login
{
    public partial class UI_LoginWindow : GComponent
    {
        public GTextField m_nFrame1;
        public GTextField m_nFrame2;
        public GButton m_nBtn;
        public const string URL = "ui://g9o3wgyhpggt0";

        public static UI_LoginWindow CreateInstance()
        {
            return (UI_LoginWindow)UIPackage.CreateObject("Login", "LoginWindow");
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