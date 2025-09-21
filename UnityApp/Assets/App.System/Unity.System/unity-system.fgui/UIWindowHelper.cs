using System.Reflection;
using ECS.Fody;
using FairyGUI;

/// <summary>
/// UI不走ECS流程，没有System，所以UI共用的静态逻辑归类到静态Helper类
/// </summary>
public static class UIWindowHelper
{
    public static void BindClickEvent(GComponent window)
    {
        var methods = window.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
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
                    if (item.GetParameters().Length == 0)
                        item.Invoke(window, null);
                    else
                        item.Invoke(window, new object[] { e });
                });
            }
        }
    }
}