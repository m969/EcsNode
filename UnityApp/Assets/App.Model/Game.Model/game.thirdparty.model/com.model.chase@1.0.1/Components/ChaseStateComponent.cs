using ECS;

namespace ECSGame.ChaseModule
{
    /// <summary>记录追踪运行态状态与运动学信息。</summary>
    public class ChaseStateComponent : EcsComponent
    {
        /// <summary>当前追踪状态。</summary>
        public ChaseState State { get; set; }

        /// <summary>当前追踪距离。</summary>
        public float CurrentDistance { get; set; }

        /// <summary>当前运动学数据。</summary>
        public ChaseKinematics Kinematics { get; set; }
    }
}
