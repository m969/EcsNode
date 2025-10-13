using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    public class AINode
    {
        public long Id { get; set; }
        public long BehaviourId { get; set; }
        //public int AIBehaviour { get; set; }
        public EcsEntity Entity { get; set; }
        public AIComponent AIComponent { get; set; }
        public AINode PreNode { get; set; }
        public IAIAction AIAction { get; set; }
    }

    public class AIComponent : EcsComponent
    {
        public long ActionQueueIndex { get; set; }
        //public Dictionary<long, AIActionQueue> AIActionQueues { get; set; } = new();
        public Dictionary<long, IAIBehaviour> AIBehaviours { get; set; } = new();
        public Dictionary<long, Queue<AINode>> Behaviour2NodeQueue { get; set; } = new();
        //public Dictionary<long, AINode> RunningNodes { get; set; } = new();

        public TSVector MoveDirection { get; set; }
        public long DetermineFrame { get; set; }

        public float IdleTime { get; set; }
        public float AttackTime { get; set; }
    }
}
