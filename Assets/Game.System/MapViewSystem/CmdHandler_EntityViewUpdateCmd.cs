using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using UnityEngine.UIElements;

namespace ECSGame
{
    public class CmdHandler_EntityViewUpdateCmd : ACommandHandler<EntityViewUpdateCmd>
    {
        protected override async ET.ETTask Handle(EntityViewUpdateCmd cmd)
        {
            if (cmd.Component is TrueTransformComponent transformComponent)
            {
                var viewComp = cmd.Entity.GetComponent<ActorViewComponent>();
                viewComp.ViewObj.transform.rotation = transformComponent.Rotation.ToQuaternion();
                if (cmd.Args is FramePlay_Move movePlay)
                {
                    viewComp.ViewObj.transform.position = movePlay.AfterPosition.ToVector();
                }
            }
        }
    }
}
