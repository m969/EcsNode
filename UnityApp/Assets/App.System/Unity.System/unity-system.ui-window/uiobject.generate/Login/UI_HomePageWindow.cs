/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Login
{
    public partial class UI_HomePageWindow : ECSUnity.UIPanel
    {
        public GTextField m_nDetermineFrame;
        public GTextField m_nAdvancedFrame;
        public GTextField m_nConflictFrame;
        public GTextField m_nConflictFrameCount;
        public GButton m_nBtn;
        public GTextField m_nPredictionFrame;
        public GTextField m_nConflictPlay;
        public GButton m_nBuildingBtn;
        public const string URL = "ui://g9o3wgyhrsu72";

        public static UI_HomePageWindow CreateInstance()
        {
            return (UI_HomePageWindow)UIPackage.CreateObject("Login", "HomePageWindow");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_nDetermineFrame = (GTextField)GetChildAt(0);
            m_nAdvancedFrame = (GTextField)GetChildAt(1);
            m_nConflictFrame = (GTextField)GetChildAt(2);
            m_nConflictFrameCount = (GTextField)GetChildAt(3);
            m_nBtn = (GButton)GetChildAt(4);
            m_nPredictionFrame = (GTextField)GetChildAt(5);
            m_nConflictPlay = (GTextField)GetChildAt(6);
            m_nBuildingBtn = (GButton)GetChildAt(7);
        }
    }
}