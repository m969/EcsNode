using ECS;
using ECSGame;
using System;
using System.Reflection;

namespace ECSUnity
{
    public class UnitySoundSystem : AComponentSystem<UnityApp, UnitySoundComponent>,
    IAwake<UnityApp, UnitySoundComponent>
    {
        public void Awake(UnityApp app, UnitySoundComponent component)
        {
            var systemAssembly = app.GetComponent<ReloadComponent>().SystemAssembly;
            var soundMaster = SoundSystem.Create(systemAssembly);
            soundMaster.Init();
            UnityAppStatic.SoundMaster = soundMaster;
        }
    }
}