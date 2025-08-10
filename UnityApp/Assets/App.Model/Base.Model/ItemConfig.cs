using System.Collections.Generic;

namespace cfg.data
{
    public partial class ItemConfig
    {
        public static Tables Tables { get; set; }

        public static Dictionary<int, ItemConfig> DataMap => Tables.TbItem.DataMap;
        public static List<ItemConfig> DataList => Tables.TbItem.DataList;

        public static ItemConfig GetOrDefault(int key)
        {
            return Tables.TbItem.GetOrDefault(key);
        }
    }
}
