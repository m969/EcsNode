using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
	public class MoveComponent : EcsComponent
	{
		public int Speed { get; set; }
		public TSVector TrueDirection { get; set; }
	} 
}
