using ECS;
using ECSGame;
using System;
using System.Reflection;

namespace ECSUnity
{
    public class UnitySceneSystem : AComponentSystem<UnityApp, UnitySceneComponent>,
    IAwake<UnityApp, UnitySceneComponent>
    {
        public void Awake(UnityApp app, UnitySceneComponent component)
        {
            var systemAssembly = app.GetComponent<ReloadComponent>().SystemAssembly;
            // var uiStage = UISystem.Create(systemAssembly);
            // uiStage.Init();
            // EcsDomain.AddNode(uiStage);
            // EcsDomain.UIStage = uiStage;
        }
    }
}