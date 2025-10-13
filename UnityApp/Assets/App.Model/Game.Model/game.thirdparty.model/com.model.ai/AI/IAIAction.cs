using ECS;
using ET;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    public interface IAIAction
    {
        void Awake(AINode aiNode);
        void Update(AINode aiNode);
    }
}
