using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ECSGame
{
	public class TrueGamePlayForecastComponent : EcsComponent
	{
        // 预测播放运行帧
        public readonly Dictionary<long, List<IFramePlay>> FramePlays = new();
    }
}
