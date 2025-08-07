using ECS;
using System;

namespace ECSGame.Module.GridBased
{
    /// <summary>
    /// 格子坐标转换组件，继承自EcsComponent，提供格子坐标与世界坐标的转换功能。
    /// </summary>
    public class GridConverterComponent : EcsComponent
    {
            /// <summary>关联的网格区域ID</summary>
            public long GridPlaneId { get; set; }
            /// <summary>网格区域世界坐标位置</summary>
            public (float x, float y) Position { get; set; }
            /// <summary>格子大小</summary>
            public float CellSize { get; set; }
    }
}
