using UnityEngine;

namespace ECSGame.ChaseModule
{
    /// <summary>描述追踪实体的运动学数据。</summary>
    public struct ChaseKinematics
    {
        /// <summary>当前运动方向向量。</summary>
        public Vector3 Direction { get; set; }

        /// <summary>当前运动速度标量。</summary>
        public float Speed { get; set; }

        /// <summary>当前速度向量。</summary>
        public Vector3 Velocity => Direction.normalized * Speed;
    }
}
