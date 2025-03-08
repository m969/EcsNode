using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using TrueSync;

public class ActorViewSystem : AEcsComponentSystem<Actor, ActorViewComponent>,
    IInit<Actor, ActorViewComponent>
{
    public void Init(Actor entity, ActorViewComponent component)
    {
        var viewObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        component.ViewObj = viewObj;
        viewObj.transform.position = Vector3.zero;
    }

    public static void SetMovePosition(Actor actor, TSVector position)
    {
        var viewComp = actor.GetComponent<ActorViewComponent>();
        viewComp.ViewObj.transform.position = position.ToVector();
    }

    public static void Update(Actor entity)
    {

    }
}
