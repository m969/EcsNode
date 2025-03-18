using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;

namespace ECSGame
{
    public class Handler_EntityCreateCmd : ACommandHandler<EntityCreateCmd>
    {
        protected override async ET.ETTask Handle(EntityCreateCmd cmd)
        {
            cmd.Entity.AddComponent<EntityViewComponent>();
            if (cmd.Entity is Item)
            {
                //EntityViewSystem.SetScale(cmd.Entity, TSVector.one * 0.2f);
            }
        }
    }
}
