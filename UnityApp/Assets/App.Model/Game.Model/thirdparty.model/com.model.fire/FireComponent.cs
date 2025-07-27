using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
	public class FireComponent : EcsComponent
	{
		public TSVector TrueDirection { get; set; }
		public bool FireState { get; set; }
		public int FireSpeed { get; set; }
	} 
}
