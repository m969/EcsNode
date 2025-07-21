using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
	public class CollisionComponent : EcsComponent
	{
		public uint Layer { get; set; }
	} 
}
