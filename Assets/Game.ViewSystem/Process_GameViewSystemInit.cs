using ECS;
using System.Collections;
using System.Collections.Generic;
using System;

public class Process_GameViewSystemInit
{
    public static void Init(EcsNode ecsNode)
    {
        ConsoleLog.Debug($"Process_GameViewSystemInit Init");

        ecsNode.AddComponent<EventComponent>();
        ecsNode.Init();

        var game = ecsNode.AddChild<TrueGame>();
        game.AddComponent<TrueGameExecuteComponent>();
        game.AddComponent<TrueGamePlayComponent>();
        game.AddComponent<PlayerInputComponent>();
        game.Init();

        var actor = game.AddChild<Actor>(beforeAwake: x => x.Type = 1);
        actor.AddComponent<MoveComponent>();
        actor.AddComponent<HealthComponent>();
        actor.AddComponent<TrueTransformComponent>();
        actor.AddComponent<ActorViewComponent>();
        actor.Init();

        game.MyActor = actor;
        //_ = MoveSystem.MoveAsync(actor, Vector3.zero);
    }

    public static void Reload(EcsNode ecsNode)
    {
        ConsoleLog.Debug($"Process_GameViewSystemInit Reload");
    }
}
