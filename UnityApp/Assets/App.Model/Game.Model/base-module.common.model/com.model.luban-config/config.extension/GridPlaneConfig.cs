using System.Collections.Generic;

namespace cfg.data
{
    public partial class GridPlaneConfig
    {
        public static Tables Tables { get; set; }

        public static Dictionary<int, GridPlaneConfig> DataMap => Tables.TbGridPlane.DataMap;
        public static List<GridPlaneConfig> DataList => Tables.TbGridPlane.DataList;

        public static GridPlaneConfig GetOrDefault(int key)
        {
            return Tables.TbGridPlane.GetOrDefault(key);
        }
    }

    public class GridPlaneConfigWrap : ECSGame.Module.GridBased.IGridPlaneConfig
    {
        public GridPlaneConfig Config { get; set; }

        public static GridPlaneConfigWrap Create(GridPlaneConfig config)
        {
            return new GridPlaneConfigWrap { Config = config };
        }

        public int Id => Config.Id;

        public string Key => Config.Key;

        public int Width => Config.Width;

        public int Height => Config.Height;

        public float CellSize => Config.CellSize;

        //public ECSGame.Module.GridBased.GridCellState InitialState => ((ECSGame.Module.GridBased.GridCellState)Config.InitialState);
    }
}
