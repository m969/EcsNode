using ECS;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSGame
{
    public class GameSystem : AEntitySystem<Game>,
        IInit<Game>,
        IUpdate<Game>,
        IFixedUpdate<Game>,
        IEventHandle<InputEvent>
    {
        public static Game Create(Assembly systemAssembly)
        {
            var game = EcsNodeSystem.Create<Game>(EcsType.Game, systemAssembly);
            return game;
        }

        public void Init(Game game)
        {

        }

        /// <summary>
        /// 游戏核心循环，每帧更新游戏状态。
        /// </summary>
        /// <param name="entity"></param>
        public void Update(Game entity)
        {
            if (entity.Type == ((int)GameType.TrueGameDemo))
            {
                GameTrueWorldSystem.Update(entity);
            }
            else
            {
                GameWorldSystem.Update(entity);
            }
        }

        public void FixedUpdate(Game entity)
        {
            if (entity.Type == ((int)GameType.TrueGameDemo))
            {
                GameTrueWorldSystem.FixedUpdate(entity);
            }
            else
            {
                GameWorldSystem.FixedUpdate(entity);
            }
        }


        public void OnHandleEvent(EcsNode ecsNode, InputEvent eventContext)
        {
            ConsoleLog.Debug("GameSystem OnHandleEvent: InputEvent " + eventContext.InputType);
            var myActor = AppStatic.MyActor;
            var trueWorld = GameTrueWorldSystem.GetTrueWorld(ecsNode.As<Game>());
            var advanceFrame = trueWorld.DetermineFrame + TrueWorld.ForecastFrame;

            if (eventContext.InputType == InputType.Fire)
            {
                var input = new InputData()
                {
                    Frame = advanceFrame,
                    PlayerId = myActor.Id,
                    InputType = InputType.Fire,
                    InputVector = eventContext.Direction.ToTSVector(),
                };

                ActorAdvancePlaySystem.AddLocalPlayerInput(myActor, input, advanceFrame);
            }
        }
    }
}
