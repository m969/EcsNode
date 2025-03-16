using ECS;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
    public class ActorSystem : AEntitySystem<Actor>,
IAwake<Actor>,
IInit<Actor>,
IUpdate<Actor>
    {
        public void Awake(Actor entity)
        {
        }

        public void Init(Actor entity)
        {
            MoveSystem.SetSpeed(entity, 1);
            FireSystem.SetSpeed(entity, 5);
        }

        public void Update(Actor entity)
        {
            //if (entity.GetComponent<MoveComponent>() is { } component)
            //{
            //    MoveSystem.Update(entity, component);
            //}
        }
    } 
}
