using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;

public class CmdHandler_MoveCmd : ACommandHandler<MoveCmd>
{
    protected override async ET.ETTask Handle(MoveCmd cmd)
    {
        //ConsoleLog.Debug($"CmdHandler_MoveCmd {cmd.Pos} {cmd.AfterPos}");

        ActorViewSystem.SetMovePosition(cmd.Actor, cmd.AfterPos);
    }
}
