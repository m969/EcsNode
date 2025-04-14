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
            MoveSystem.SetSpeed(entity, 10);
            MoveSystem.SetStopSpeed(entity, 5);
            FireSystem.SetSpeed(entity, 5);

            if (entity.GetComponent<AIComponent>() is { } component)
            {
                component.Enable = true;
            }
        }

        public void Update(Actor entity)
        {

        }

        public static Actor Create(TrueGame game, long actorId)
        {
            var actor = game.AddChild<Actor>(actorId, beforeAwake: x => x.Type = 1);
            actor.AddComponent<TransformComponent>();
            actor.AddComponent<CollisionComponent>();
            actor.AddComponent<MoveComponent>();
            actor.AddComponent<HealthComponent>();
            actor.AddComponent<FireComponent>();
            return actor;
        }
    } 
}
