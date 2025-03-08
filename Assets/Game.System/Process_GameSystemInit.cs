using ECS;
using System.Collections;
using System.Collections.Generic;
using System;

public class Process_GameSystemInit
{
    public static void Init(EcsNode ecsNode)
    {
        Debug.Log($"Process_GameSystemInit Init");
    }

    public static void Reload(EcsNode ecsNode)
    {
        Debug.Log($"Process_GameSystemInit Reload");
    }
}
