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
            MoveSystem.SetSpeed(entity, 2);
            FireSystem.SetSpeed(entity, 5);
        }

        public void Update(Actor entity)
        {
            //if (entity.GetComponent<MoveComponent>() is { } component)
            //{
            //    MoveSystem.Update(entity, component);
            //}
        }

        public static Actor CreateActor(TrueGame game)
        {
            var actor = game.AddChild<Actor>(beforeAwake: x => x.Type = 1);
            actor.AddComponent<MoveComponent>();
            actor.AddComponent<HealthComponent>();
            actor.AddComponent<TransformComponent>();
            actor.AddComponent<FireComponent>();
            return actor;
        }
    } 
}
