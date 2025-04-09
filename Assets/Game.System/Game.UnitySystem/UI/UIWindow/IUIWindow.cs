using FairyGUI;
using FairyGUI.Utils;

namespace ECSUnity
{
    public abstract class UIPanel : GComponent
    {
        public void Show()
        {
            visible = true;
            if (this is IUIWindow window)
            {
                window.OnShow();
            }
        }

        public void Hide()
        {
            visible = false;
            if (this is IUIWindow window)
            {
                window.OnHide();
            }
        }
    }

    public interface IUIWindow
    {
        void Awake();
        void OnShow();
        void OnHide();
    }
}