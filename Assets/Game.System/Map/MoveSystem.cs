using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrueSync;

public class MoveSystem : AEcsComponentSystem<Actor, MoveComponent>,
    IAwake<Actor, MoveComponent>,
    IInit<Actor, MoveComponent>
{
    public void Awake(Actor actor, MoveComponent moveComponent)
    {
        ConsoleLog.Debug($"MoveSystem Awake {actor.GetType().Name} {moveComponent.GetType().Name}");
    }

    public void Init(Actor actor, MoveComponent moveComponent)
    {
        ConsoleLog.Debug($"MoveSystem Init {actor.GetType().Name} {moveComponent.GetType().Name}");
    }
     
    public static void Update(Actor actor, MoveComponent moveComponent)
    {
        //Debug.Log($"MoveSystem Update {actor.GetType().Name} {moveComponent.GetType().Name}");
    }

    public async static Task MoveAsync(Actor actor, TSVector target)
    {
        var moveComp = actor.GetComponent<MoveComponent>();
    }

    public static void ChangeMove(Actor actor, TSVector target)
    {
        //ConsoleLog.Debug($"MoveSystem ChangeMove {target}");
        var moveComp = actor.GetComponent<MoveComponent>();
        moveComp.TrueDirection = target;
    }

    public static IFramePlay MoveFrame(Actor actor)
    {
        var moveComp = actor.GetComponent<MoveComponent>();
        var transComp = actor.GetComponent<TrueTransformComponent>();
        var beforePos = transComp.Position;
        var afterPos = transComp.Position + moveComp.TrueDirection;

        var framePlay = new FramePlay_Move()
        {
            EntityId = actor.Id,
            Position = beforePos,
            AfterPosition = afterPos
        };

        return framePlay;
    }

    public static void SetMovePosition(Actor actor, TSVector position)
    {
        //ConsoleLog.Log($"SetMovePosition: {position}");
        var transComp = actor.GetComponent<TrueTransformComponent>();
        var beforePos = transComp.Position;
        transComp.Position = position;

        EventSystem.Dispatch(actor.EcsNode, new MoveCmd()
        {
            Actor = actor,
            Pos = beforePos,
            AfterPos = position
        });
    }
}
