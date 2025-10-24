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
            var game = EcsNodeSystem.Create<UnityApp>(EcsType.UnityApp, systemAssembly);
            return game;
        }

        public void Init(UnityApp entity)
        {

        }
    }
}