using ECS;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine.UIElements;

namespace ECSGame
{
    public class TrueWorldPlaySystem : AComponentSystem<TrueWorld, TrueWorldPlayComponent>,
        IAwake<TrueWorld, TrueWorldPlayComponent>,
        IInit<TrueWorld, TrueWorldPlayComponent>
    {
        public void Awake(TrueWorld game, TrueWorldPlayComponent component)
        {
        }

        public void Init(TrueWorld game, TrueWorldPlayComponent component)
        {
        }

        public static IFramePlay FillFramePlay(TrueWorld game, TrueWorldPlayComponent component, StatePlayType playType, EcsEntity entity, long determineFrame)
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
        public static void CreateFramePlays(TrueWorld game, TrueWorldPlayComponent component, long determineFrame)
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
        public static void PlayFramePlays(TrueWorld game, TrueWorldPlayComponent component, long determineFrame)
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

        public static void FrameUpdate(TrueWorld trueWorld, long determineFrame)
        {
            if (!trueWorld.TryGetComponent<TrueWorldPlayComponent>(out var component))
            {
                return;
            }

            if (!component.FramePlays.ContainsKey(determineFrame))
            {
                component.FramePlays[determineFrame] = new List<IFramePlay>();
            }

            CreateFramePlays(trueWorld, component, determineFrame);
            PlayFramePlays(trueWorld, component, determineFrame);
        }
    }
}
