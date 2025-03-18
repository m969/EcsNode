using ECS;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ECSGame
{
    public class Process_GameSystemInit
    {
        public static void Init(EcsNode ecsNode, List<Type> types)
        {
            ConsoleLog.Debug($"Process_GameSystemInit Init");
            ecsNode.AddSystems(types.ToArray());
            ecsNode.EcsUpdate = new EcsNodeSystem();
        }

        public static void Reload(EcsNode ecsNode, List<Type> types)
        {
            ConsoleLog.Debug($"Process_GameSystemInit Reload");
            ecsNode.AddSystems(types.ToArray());
            ecsNode.EcsUpdate = new EcsNodeSystem();
        }
    } 
}
