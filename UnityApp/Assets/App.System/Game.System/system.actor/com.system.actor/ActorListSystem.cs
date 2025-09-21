using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    public class ActorListSystem : AComponentSystem<EcsEntity, ActorListComponent>
    {
        public static void AddActor(EcsEntity world, Actor actor)
        {
            world.GetComponent<ActorListComponent>().ActorList.Add(actor);
            world.GetComponent<ActorListComponent>().ActorDict.Add(actor.Id, actor);
        }

        public static void RemoveActor(EcsEntity world, Actor actor)
        {
            world.GetComponent<ActorListComponent>().ActorList.Remove(actor);
            world.GetComponent<ActorListComponent>().ActorDict.Remove(actor.Id);
        }
    }
}
