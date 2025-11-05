using ECS;
using ECS.Fody;
using ECSGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSUnity
{
    [SystemGameFilter((int)GameType.TrueGameDemo)]
    public class TrueWorldFrameUpdateSystem : AEntitySystem<TrueWorld>,
        IOnFrameUpdate
    {
        public void OnFrameUpdate(TrueWorld trueWorld, long determineFrame)
        {
            TrueGameInputSystem.OnFrameUpdate(trueWorld, determineFrame);
        }
    }
}
