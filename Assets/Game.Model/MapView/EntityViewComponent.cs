using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;

namespace ECSGame
{
	public class EntityViewComponent : EcsComponent
	{
		public GameObject ViewObj { get; set; }
	} 
}
