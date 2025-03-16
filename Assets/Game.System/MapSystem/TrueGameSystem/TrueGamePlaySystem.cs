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

        public static IFramePlay FillFramePlay(TrueGame game, TrueGamePlayComponent component, StatePlayType playType, EcsEntity entity)
        {
            IFramePlay framePlay = null;

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
                        framePlay = new FramePlay_Move()
                        {
                            EntityId = entity.Id,
                            Position = beforePos,
                            AfterPosition = afterPos
                        };
                        AddFramePlay(game, component, framePlay);
                    }
                    break;
                case StatePlayType.Fire:
                    //{
                    //    var fireComp = entity.GetComponent<FireComponent>();
                    //    framePlay = new FramePlay_Fire()
                    //    {
                    //        EntityId = entity.Id,
                    //        Direction = fireComp.TrueDirection,
                    //    };
                    //    AddFramePlay(game, component, framePlay);
                    //}
                    break;
                default:
                    break;
            }

            return framePlay;
        }

        public static void AddFramePlay(TrueGame game, TrueGamePlayComponent component, IFramePlay framePlay)
        {
            var frame = game._currentFrame;
            component.FramePlays[frame].Add(framePlay);
        }

        /// <summary>
        /// 根据游戏状态创建运行帧
        /// </summary>
        public static void CreateFramePlays(TrueGame game, TrueGamePlayComponent component)
        {
            var actors = game.Id2Children.Values;
            foreach (var entity in actors)
            {
                if (entity.GetComponent<MoveComponent>() is { } moveComponent)
                {
                    if (moveComponent.TrueDirection != TSVector.zero)
                    {
                        FillFramePlay(game, component, StatePlayType.Move, entity);
                    }
                }

                //if (entity.GetComponent<FireComponent>() is { } fireComponent)
                //{
                //    if (fireComponent.FireState)
                //    {
                //        FillFramePlay(game, component, StatePlayType.Fire, entity);
                //    }
                //}
            }
        }

        /// <summary>
        /// 播放运行帧序列改变游戏状态
        /// </summary>
        public static void PalyFramePlays(TrueGame game, TrueGamePlayComponent component)
        {
            var frame = game._currentFrame;

            foreach (var framePlay in component.FramePlays[frame])
            {
                var actor = game.GetChild<EcsEntity>(framePlay.EntityId);

                if (framePlay is FramePlay_Move movePlay)
                {
                    MoveSystem.SetMovePosition(actor, movePlay.AfterPosition);
                }

                //if (framePlay is FramePlay_Fire firePlay)
                //{
                //    FireSystem.Shoot(game, (Actor)actor, firePlay.Direction);
                //}
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
