using ECS;
using System.Collections;
using System.Collections.Generic;

public class EcsNodeSystem : AEcsEntitySystem<EcsNode>,
    IAwake<EcsNode>,
    IInit<EcsNode>,
    IUpdate<EcsNode>
{
    public void Awake(EcsNode entity)
    {
    }

    public void Init(EcsNode entity)
    {
    }

    public void Update(EcsNode entity)
    {
        if (entity.GetComponent<EventComponent>() is { } component)
        {
            EventSystem.Update(entity, component);
        }
    }
}
