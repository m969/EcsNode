using ECS;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

public class TrueGamePlaySystem : AEcsComponentSystem<TrueGame, TrueGamePlayComponent>,
    IAwake<TrueGame, TrueGamePlayComponent>,
    IInit<TrueGame, TrueGamePlayComponent>
{
    public void Awake(TrueGame game, TrueGamePlayComponent component)
    {
    }

    public void Init(TrueGame game, TrueGamePlayComponent component)
    {
    }

    public static void PlayFrame(TrueGame game, IFramePlay framePlay)
    {
        if (framePlay is FramePlay_Move movePlay)
        {
            var actor = game.GetChild<Actor>(movePlay.EntityId);
            MoveSystem.SetMovePosition(actor, movePlay.AfterPosition);
        }
    }

    public static void Update(TrueGame game, TrueGamePlayComponent component)
    {
        //ConsoleLog.Log($"TrueGamePlaySystem Update");
        var frame = game._currentFrame;

        var actors = game.Id2Children.Values;

        foreach (var item in actors)
        {
            if (item.GetComponent<MoveComponent>().TrueDirection != TSVector.zero)
            {
                //ConsoleLog.Debug($"TrueGamePlaySystem MoveFrame");
                var framePlay = MoveSystem.MoveFrame(item as Actor);
                PlayFrame(game, framePlay);

                if (!component.FramePlays.ContainsKey(frame))
                {
                    component.FramePlays[frame] = new List<IFramePlay>();
                }
                component.FramePlays[frame].Add(framePlay);
            }
        }
    }
}
