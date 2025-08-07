using ECS;
using System.Collections.Generic;

namespace ECSGame.Module.GridBased
{
    /// <summary>
    /// 网格区域实体，继承自EcsEntity，包含网格尺寸、格子集合等属性。
    /// </summary>
    public class GridPlane : EcsEntity
    {
        /// <summary>网格宽度</summary>
        public int Width { get; set; }
        /// <summary>网格高度</summary>
        public int Height { get; set; }
        /// <summary>网格配置ID</summary>
        public int ConfigId { get; set; }
        /// <summary>格子大小</summary>
        public float CellSize { get; set; }
        /// <summary>网格区域世界坐标位置</summary>
        public (float x, float y) Position { get; set; }
    }
}
