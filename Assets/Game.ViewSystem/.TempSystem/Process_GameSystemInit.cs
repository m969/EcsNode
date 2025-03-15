using ECS;
using System.Collections;
using System.Collections.Generic;
using System;

public class Process_GameSystemInit
{
    public static void Init(EcsNode ecsNode)
    {
        ConsoleLog.Debug($"Process_GameSystemInit Init");
        ecsNode.EcsUpdate = new EcsNodeSystem();
    }

    public static void Reload(EcsNode ecsNode)
    {
        ConsoleLog.Debug($"Process_GameSystemInit Reload");
        ecsNode.EcsUpdate = new EcsNodeSystem();
    }
}
