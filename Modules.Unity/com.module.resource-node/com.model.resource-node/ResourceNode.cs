using UnityEngine;

namespace Module.ResourceNode.Model
{
    /// <summary>
    /// 资源类型枚举（粮食、木材、矿石、宝石等）
    /// </summary>
    public enum ResourceType
    {
        /// <summary>粮食</summary>
        Food,
        /// <summary>木材</summary>
        Wood,
        /// <summary>矿石</summary>
        Ore,
        /// <summary>宝石</summary>
        Gem
        // ...可扩展
    }

    /// <summary>
    /// 资源等级（基础/稀有）
    /// </summary>
    public enum ResourceTier
    {
        /// <summary>基础</summary>
        Basic,
        /// <summary>稀有</summary>
        Rare
    }

    /// <summary>
    /// 资源地块实体，包含所有资源属性，仅存储数据
    /// </summary>
    public class ResourceNode:EcsEntity
    {
        /// <summary>资源类型</summary>
        public ResourceType resourceType;
        /// <summary>资源等级</summary>
        public ResourceTier resourceTier;
        /// <summary>当前存量</summary>
        public float currentAmount;
        /// <summary>最大存量</summary>
        public float maxAmount;
        /// <summary>基础单次产量</summary>
        public float baseYield;
        /// <summary>地块采集效率</summary>
        public float efficiency;
        /// <summary>地图坐标</summary>
        public Vector2Int position;
        /// <summary>是否正被采集</summary>
        public bool isBeingCollected;
        /// <summary>离线计算用，上次采集时间</summary>
        public float lastCollectedTime;
        /// <summary>保护时间倒计时</summary>
        public float protectionTimeRemaining;
        /// <summary>所属联盟ID</summary>
        public int ownerAllianceId;
    }
}
