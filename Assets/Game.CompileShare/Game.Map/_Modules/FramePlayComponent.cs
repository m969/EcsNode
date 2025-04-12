using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
	public class FramePlayComponent : EcsComponent<Actor>
	{
        // 本地播放运行帧
        public readonly Dictionary<long, List<IFramePlay>> FramePlays = new();
        // 本地预测播放运行帧
        public readonly Dictionary<long, List<IFramePlay>> PredictionFramePlays = new();

        public readonly Dictionary<long, List<PlayerInput>> DetermineFrameInputs = new();

        public readonly Dictionary<long, List<PlayerInput>> AdvanceFrameInputs = new();

        public long DetermineFrame;
        public long AlreadyPredictFrame;

        public long ConflictFrame;
        public long ConflictFrameCount;
        public string ConflictType;
    }
}
