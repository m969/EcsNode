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

        public static IFramePlay FillFramePlay(TrueGame game, TrueGamePlayComponent component, StatePlayType playType, EcsEntity entity, long determineFrame)
        {
            IFramePlay framePlay = null;

            if (playType == StatePlayType.Move)
            {
                var moveComp = entity.GetComponent<MoveComponent>();
                var transComp = entity.GetComponent<TransformComponent>();
                var beforePos = transComp.Position;
                var afterPos = transComp.Position + moveComp.TrueDirection * FP.FromFloat(moveComp.Speed * MoveSystem.SpeedAdaptive);
                framePlay = new FramePlay_Move()
                {
                    EntityId = entity.Id,
                    Position = beforePos,
                    AfterPosition = afterPos
                };
            }

            if (framePlay != null)
            {
                component.FramePlays[determineFrame].Add(framePlay);
            }

            return framePlay;
        }

        /// <summary>
        /// 根据游戏状态创建运行帧
        /// </summary>
        public static void CreateFramePlays(TrueGame game, TrueGamePlayComponent component, long determineFrame)
        {
            var actors = game.Id2Children.Values;
            foreach (var entity in actors)
            {
                if (entity is Actor) continue;
                if (entity.GetComponent<MoveComponent>() is { } moveComponent)
                {
                    if (moveComponent.TrueDirection != TSVector.zero)
                    {
                        FillFramePlay(game, component, StatePlayType.Move, entity, determineFrame);
                    }
                }
            }
        }

        /// <summary>
        /// 播放运行帧序列改变游戏状态
        /// </summary>
        public static void PlayFramePlays(TrueGame game, TrueGamePlayComponent component, long determineFrame)
        {
            foreach (var framePlay in component.FramePlays[determineFrame])
            {
                var actor = game.GetChild<EcsEntity>(framePlay.EntityId);

                if (framePlay is FramePlay_Move movePlay)
                {
                    MoveSystem.SetMovePosition(actor, movePlay.AfterPosition);
                }
            }
        }

        public static void FrameUpdate(TrueGame game, TrueGamePlayComponent component, long determineFrame)
        {
            if (!component.FramePlays.ContainsKey(determineFrame))
            {
                component.FramePlays[determineFrame] = new List<IFramePlay>();
            }

            CreateFramePlays(game, component, determineFrame);
            PlayFramePlays(game, component, determineFrame);
        }
    }
}
