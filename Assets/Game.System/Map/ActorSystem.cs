using ECS;
using System.Collections;
using System.Collections.Generic;

public class ActorSystem : AEcsEntitySystem<Actor>,
    IAwake<Actor>,
    IInit<Actor>,
    IUpdate<Actor>
{
    public void Awake(Actor entity)
    {
        //Debug.Log($"ActorSystem Awake {entity.GetType().Name}");
    }

    public void Init(Actor entity)
    {
        //Debug.Log($"ActorSystem Init {entity.GetType().Name}");
    }

    public void Update(Actor entity)
    {
        //if (entity.GetComponent<MoveComponent>() is { } component)
        //{
        //    MoveSystem.Update(entity, component);
        //}
    }
}
