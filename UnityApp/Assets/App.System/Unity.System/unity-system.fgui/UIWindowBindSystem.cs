using System.Reflection;
using ECS.Fody;
using FairyGUI;

public class UIWindowBindSystem
{
    public static void BindClickEvent(GComponent window)
    {
        var methods = window.GetType().GetMethods();
        foreach (var item in methods)
        {
            var attribute = item.GetCustomAttribute<AfterClickAttribute>();
            if (attribute != null)
            {
                var targetBtn = attribute.TargetButtonName;
                var arr = targetBtn.Split('_');
                if (arr.Length > 1)
                {
                    targetBtn = arr[1];
                }
                window.GetChild(targetBtn).onClick.Add((e) =>
                {
                    item.Invoke(window, new object[] { e });
                });
            }
        }
    }
}