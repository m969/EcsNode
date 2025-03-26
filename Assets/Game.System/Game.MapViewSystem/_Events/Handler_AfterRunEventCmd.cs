using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ECS.Unity;
using ET;

namespace ECSGame
{
    public class Handler_AfterRunEventCmd : ACommandHandler<AfterRunEventCmd>
    {
        protected override async ET.ETTask Handle(EcsNode ecsNode, AfterRunEventCmd cmd)
        {
            if (cmd.EventRun is FireEvent)
            {
                SoundSystem.PlayClip(SoundType.OnceFire);
            }
        }
    }
}
