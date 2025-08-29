using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using MessagePack;
using LiteNetLib;

namespace ECSGame.Module.Neter
{
    /// <summary>
    /// 节点配置
    /// </summary>
    [MessagePackObject]
    public class NodeConfig
    {
        /// <summary>
        /// 网络更新时间间隔（毫秒）
        /// </summary>
        [Key(0)]
        public int UpdateTime { get; set; } = 15;

        /// <summary>
        /// Ping间隔（毫秒）
        /// </summary>
        [Key(1)]
        public int PingInterval { get; set; } = 1000;

        /// <summary>
        /// 断开连接超时（毫秒）
        /// </summary>
        [Key(2)]
        public int DisconnectTimeout { get; set; } = 5000;

        /// <summary>
        /// 心跳间隔（毫秒）
        /// </summary>
        [Key(3)]
        public int HeartbeatInterval { get; set; } = 30000;

        /// <summary>
        /// 服务发现间隔（毫秒）
        /// </summary>
        [Key(4)]
        public int DiscoveryInterval { get; set; } = 60000;

        /// <summary>
        /// 最大连接数
        /// </summary>
        [Key(5)]
        public int MaxConnections { get; set; } = 100;

        /// <summary>
        /// 默认连接密钥
        /// </summary>
        [Key(6)]
        public string DefaultConnectionKey { get; set; } = "DefaultKey";



        /// <summary>
        /// 是否启用消息压缩
        /// </summary>
        [Key(7)]
        public bool EnableCompression { get; set; } = false;

        /// <summary>
        /// 是否启用消息加密
        /// </summary>
        [Key(8)]
        public bool EnableEncryption { get; set; } = false;

        /// <summary>
        /// 消息缓冲区大小
        /// </summary>
        [Key(9)]
        public int MessageBufferSize { get; set; } = 8192;

        public NodeConfig() { }
    }

    /// <summary>
    /// 节点状态
    /// </summary>
    public enum NodeStatus
    {
        Unknown,
        Offline,
        Connecting,
        Online,
        Disconnecting,
        Error
    }
}