using ECS;
using ECSGame;
using System;
using System.Reflection;
using FairyGUI;

namespace ECSUnity
{
    public class UnityUISystem : AComponentSystem<UnityApp, UnityUIComponent>,
    IAwake<UnityApp, UnityUIComponent>
    {
        public void Awake(UnityApp app, UnityUIComponent component)
        {
            var systemAssembly = app.GetComponent<ReloadComponent>().SystemAssembly;
            var uiStage = UISystem.Create(systemAssembly);
            uiStage.Init();
            EcsDomain.AddNode(uiStage);
            EcsDomain.UIStage = uiStage;

            var groot = GRoot.inst;
            groot.SetContentScaleFactor(1280, 720);
            UnityAppRun.ReloadUI();
        }
    }
}