using ECS;
using ECSUnity;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    public class ActorPlaySystem : AComponentSystem<Actor, FramePlayComponent>,
IAwake<Actor, FramePlayComponent>
    {
        public void Awake(Actor actor, FramePlayComponent component)
        {

        }

        public static void FrameUpdate(Actor actor, FramePlayComponent component, long determineFrame)
        {
            component.DetermineFrame = determineFrame;

            ActorDeterminePlaySystem.DetermineCreate(actor, determineFrame);
            if (UnityStatic.MyActor == actor)
            {
                // 本地先行
                ActorAdvancePlaySystem.LocalAdvanceCreate(actor, determineFrame);
            }
            else
            {
                // 他人预测
                ActorPredictPlaySystem.PredictCreate(actor, determineFrame);
            }
        }
    }
}
