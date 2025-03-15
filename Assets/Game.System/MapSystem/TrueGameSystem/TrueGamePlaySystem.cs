using ECS;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine.UIElements;

namespace ECSGame
{
    public class TrueGamePlaySystem : AComponentSystem<TrueGame, TrueGamePlayComponent>,
IAwake<TrueGame, TrueGamePlayComponent>,
IInit<TrueGame, TrueGamePlayComponent>
    {
        public void Awake(TrueGame game, TrueGamePlayComponent component)
        {
        }

        public void Init(TrueGame game, TrueGamePlayComponent component)
        {
        }

        public static IFramePlay FillFramePlay(TrueGame game, StatePlayType playType, EcsEntity entity)
        {
            switch (playType)
            {
                case StatePlayType.None:
                    break;
                case StatePlayType.Move:
                    {
                        var moveComp = entity.GetComponent<MoveComponent>();
                        var transComp = entity.GetComponent<TrueTransformComponent>();
                        var beforePos = transComp.Position;
                        var afterPos = transComp.Position + moveComp.TrueDirection * FP.FromFloat(moveComp.Speed * 0.002f);

                        var framePlay = new FramePlay_Move()
                        {
                            EntityId = entity.Id,
                            Position = beforePos,
                            AfterPosition = afterPos
                        };

                        return framePlay;
                    }
                default:
                    break;
            }

            return null;
        }

        public static void CreateFramePlays(TrueGame game, TrueGamePlayComponent component)
        {
            var frame = game._currentFrame;
            var actors = game.Id2Children.Values;
            foreach (var entity in actors)
            {
                if (entity.GetComponent<MoveComponent>().TrueDirection != TSVector.zero)
                {
                    var framePlay = FillFramePlay(game, StatePlayType.Move, entity);
                    component.FramePlays[frame].Add(framePlay);
                }
            }
        }

        public static void PalyFramePlays(TrueGame game, TrueGamePlayComponent component)
        {
            var frame = game._currentFrame;

            foreach (var framePlay in component.FramePlays[frame])
            {
                var actor = game.GetChild<Actor>(framePlay.EntityId);
                EcsComponent component2 = null;

                if (framePlay is FramePlay_Move movePlay)
                {
                    var transComp = actor.GetComponent<TrueTransformComponent>();
                    var beforePos = transComp.Position;
                    transComp.Position = movePlay.AfterPosition;
                    component2 = transComp;
                }

                EventSystem.Dispatch(actor.EcsNode, new EntityViewUpdateCmd()
                {
                    Entity = actor,
                    Component = component2,
                    Args = framePlay,
                });
            }
        }

        public static void FrameUpdate(TrueGame game, TrueGamePlayComponent component)
        {
            //ConsoleLog.Log($"TrueGamePlaySystem Update");
            var frame = game._currentFrame;
            if (!component.FramePlays.ContainsKey(frame))
            {
                component.FramePlays[frame] = new List<IFramePlay>();
            }

            CreateFramePlays(game, component);
            PalyFramePlays(game, component);
        }
    } 
}
