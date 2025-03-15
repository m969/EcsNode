using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;

namespace ECSGame
{
	public class ActorViewComponent : EcsComponent
	{
		public GameObject ViewObj { get; set; }
	} 
}
