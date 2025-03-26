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
        // 本地预测播放运行帧
        public readonly Dictionary<long, List<IFramePlay>> FramePlays = new();

        // 本地输入先行表现播放

        // 服务器权威播放运行帧
    }
}
