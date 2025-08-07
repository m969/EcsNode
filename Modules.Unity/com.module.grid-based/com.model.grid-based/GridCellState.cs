namespace ECSGame.Module.GridBased
{
    /// <summary>
    /// 格子状态枚举
    /// </summary>
    public enum GridCellState
    {
        /// <summary>空置</summary>
        Empty = 0,
        /// <summary>占用</summary>
        Occupied = 1,
        /// <summary>被锁定</summary>
        Locked = 2,
        /// <summary>被选中</summary>
        Selected = 3
    }
}
