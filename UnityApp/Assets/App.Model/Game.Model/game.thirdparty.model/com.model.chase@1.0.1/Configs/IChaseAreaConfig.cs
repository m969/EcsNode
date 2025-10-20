using System.Collections.Generic;
using UnityEngine;

namespace ECSGame.ChaseModule
{
    /// <summary>定义追踪行为的区域边界与越界策略。</summary>
    public interface IChaseAreaConfig
    {
        /// <summary>区域类型定义。</summary>
        AreaType Type { get; }

        /// <summary>区域中心坐标。</summary>
        Vector3 Center { get; }

        /// <summary>圆形区域半径。</summary>
        float Radius { get; }

        /// <summary>多边形区域顶点列表。</summary>
        List<Vector3> Points { get; }

        /// <summary>越界处理策略。</summary>
        OutOfAreaStrategy Strategy { get; }

        /// <summary>回退策略的步长。</summary>
        float RollbackStep { get; }
    }
}
