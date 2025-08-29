using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using MessagePack;
using LiteNetLib;

namespace ECSGame.Module.Neter
{
    /// <summary>
    /// 节点信息
    /// </summary>
    [MessagePackObject]
    public class NodeInfo
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        [Key(0)]
        public string NodeId { get; set; } = "";

        /// <summary>
        /// 节点类型
        /// </summary>
        [Key(1)]
        public NodeType NodeType { get; set; }

        /// <summary>
        /// 节点终端点
        /// </summary>
        [Key(2)]
        public IPEndPoint? EndPoint { get; set; }

        /// <summary>
        /// 节点状态
        /// </summary>
        [Key(3)]
        public NodeStatus Status { get; set; }

        /// <summary>
        /// 连接时间
        /// </summary>
        [Key(4)]
        public DateTime ConnectedTime { get; set; }

        /// <summary>
        /// 最后心跳时间
        /// </summary>
        [Key(5)]
        public DateTime LastHeartbeat { get; set; }

        /// <summary>
        /// 延迟（毫秒）
        /// </summary>
        [Key(6)]
        public int Latency { get; set; }

        /// <summary>
        /// 节点负载
        /// </summary>
        [Key(7)]
        public float Load { get; set; }

        /// <summary>
        /// 节点属性
        /// </summary>
        [Key(8)]
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// 关联的NetPeer
        /// </summary>
        [IgnoreMember]
        public NetPeer? Peer { get; set; }

        /// <summary>
        /// 连接持续时间
        /// </summary>
        [IgnoreMember]
        public TimeSpan ConnectionDuration => DateTime.UtcNow - ConnectedTime;

        /// <summary>
        /// 是否在线
        /// </summary>
        [IgnoreMember]
        public bool IsOnline => Status == NodeStatus.Online && Peer != null && Peer.ConnectionState == ConnectionState.Connected;

        public NodeInfo() { }

        public static NodeInfo Create(string nodeId, NodeType nodeType, IPEndPoint endPoint)
        {
            var nodeInfo = new NodeInfo
            {
                NodeId = nodeId,
                NodeType = nodeType,
                Status = NodeStatus.Online,
                ConnectedTime = DateTime.UtcNow,
                EndPoint = endPoint,
            };
            return nodeInfo;
        }

        public override string ToString()
        {
            return $"Node[{NodeId}] {NodeType} at {EndPoint} (Status: {Status}, Load: {Load:P})";
        }
    }
}