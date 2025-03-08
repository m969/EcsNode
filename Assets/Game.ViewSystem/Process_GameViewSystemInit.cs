using ECS;
using System.Collections;
using System.Collections.Generic;
using System;

public class Process_GameViewSystemInit
{
    public static void Init(EcsNode ecsNode)
    {
        Debug.Log($"Process_GameViewSystemInit Init");

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
        //_ = MoveSystem.MoveAsync(actor, Vector3.zero);
    }

    public static void Reload(EcsNode ecsNode)
    {
        Debug.Log($"Process_GameViewSystemInit Reload");
    }
}
