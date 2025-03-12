using ECS;
using System.Collections;
using System.Collections.Generic;

public class EcsNodeSystem : IUpdate<EcsNode>
{
    public void Update(EcsNode entity)
    {
        if (entity.GetComponent<EventComponent>() is { } component)
        {
            EventSystem.Update(entity, component);
        }
    }
}
