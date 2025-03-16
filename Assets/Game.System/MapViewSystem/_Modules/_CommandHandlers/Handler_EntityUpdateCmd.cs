using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;

namespace ECSGame
{
    public class Handler_EntityUpdateCmd : ACommandHandler<EntityUpdateCmd>
    {
        protected override async ET.ETTask Handle(EntityUpdateCmd cmd)
        {
            if (cmd.Entity.GetComponent<EntityViewComponent>() is { } viewComp && viewComp.ViewObj != null)
            {
                if (cmd.ChangeComponent is TrueTransformComponent transformComponent)
                {
                    viewComp.ViewObj.transform.rotation = transformComponent.Rotation.ToQuaternion();
                }

                if (cmd.ChangeComponent is MoveComponent moveComponent)
                {
                    viewComp.ViewObj.transform.position = cmd.Entity.GetComponent<TrueTransformComponent>().Position.ToVector();
                }
            }
        }
    }
}
