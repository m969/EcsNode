using ECS;
using ECSGame;
using System;
using System.Reflection;

namespace ECSUnity
{
    public class UnityAppSystem : AEntitySystem<UnityApp>,
    IInit<UnityApp>
    {
        public static UnityApp Create(Assembly systemAssembly)
        {
            var app = EcsNodeSystem.Create<UnityApp>(EcsType.UnityApp, systemAssembly);
            UnityAppStatic.App = app;
            return app;
        }

        public void Init(UnityApp entity)
        {

        }
    }
}