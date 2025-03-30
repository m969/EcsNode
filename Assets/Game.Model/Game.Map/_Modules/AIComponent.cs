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
		public int AIBehaviour {  get; set; }
		public EcsEntity Entity { get; set; }
		public AINode PreNode { get; set; }
		public IAIAction AIAction { get; set; }
		//public Action<AINode> StartAction {  get; set; }
		//public Action<AINode> RunAction {  get; set; }
        //public AINode NextNode { get; set; }
    }

    public class AIComponent : EcsComponent
	{
		public long ActionQueueIndex {  get; set; }
		public Dictionary<long, AIActionQueue> AIActionQueues { get; set; } = new();
		public Dictionary<long, Queue<AINode>> NodeMap { get; set; } = new();
		public Dictionary<long, AINode> RunningNodes { get; set; } = new();

		public TSVector MoveDirection { get; set; }
		public long DetermineFrame { get; set; }
    }
}
