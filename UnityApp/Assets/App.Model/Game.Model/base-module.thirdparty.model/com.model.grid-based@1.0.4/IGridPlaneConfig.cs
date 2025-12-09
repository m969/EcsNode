using ECS;
using System;
using System.Collections.Generic;

namespace ECSGame.Module.GridBased
{
    /// <summary>
    /// 网格区域配置接口，唯一标识Id，辅助名称Key，包含网格尺寸、格子大小等参数。
    /// </summary>
    public interface IGridPlaneConfig
    {
        /// <summary>网格区域配置唯一标识</summary>
        int Id { get; }
        /// <summary>网格区域配置名称</summary>
        string Key { get; }
        /// <summary>网格区域宽度（格子数量）</summary>
        int Width { get; }
        /// <summary>网格区域高度（格子数量）</summary>
        int Height { get; }
        /// <summary>单个格子的大小（世界单位）</summary>
        float CellSize { get; }
    }
}