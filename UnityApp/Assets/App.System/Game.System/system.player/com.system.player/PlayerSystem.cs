using ECS;
using ECSGame;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECS
{
    public class PlayerSystem : AEntitySystem<Player>,
        IAwake<Player>,
        IInit<Player>
    {
        public void Awake(Player entity)
        {
        }

        public void Init(Player entity)
        {
        }

        public static Player Create(Assembly systemAssembly)
        {
            var player = EcsNodeSystem.Create<Player>(EcsType.Player, systemAssembly);
            return player;
        }
    }
}