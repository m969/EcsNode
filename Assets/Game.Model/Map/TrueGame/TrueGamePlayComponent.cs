using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ECSGame
{
	public class TrueGamePlayComponent : EcsComponent
	{
		public readonly Dictionary<long, List<IFramePlay>> FramePlays = new();
	} 
}
