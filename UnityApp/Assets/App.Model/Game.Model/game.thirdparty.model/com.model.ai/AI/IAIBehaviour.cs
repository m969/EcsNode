using ECS;
using ET;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    public interface IAIBehaviour
    {
        public AINode StartBehaviour(EcsEntity entity, long behaviourId);
        public void MoveNext(AINode aiNode);
    }
}
