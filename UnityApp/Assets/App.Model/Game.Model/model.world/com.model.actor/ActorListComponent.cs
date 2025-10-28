using ECS;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
    public class ActorListComponent : EcsComponent
    {
        public List<Actor> ActorList = new List<Actor>();
        public Dictionary<long, Actor> ActorDict = new Dictionary<long, Actor>();
    }
}
