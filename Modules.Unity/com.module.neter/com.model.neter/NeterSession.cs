using ECS;
using ET;
using LiteNetLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Collections.Specialized.BitVector32;

namespace ECSGame.Module.Neter
{
    /// <summary>
    /// Neter会话，用于管理与远程Neter进程的通信
    /// </summary>
    public class NeterSession : EcsEntity
    {
        /// <summary>
        /// 网络服务组件
        /// </summary>
        public Neter Neter { get; set; }

        public new long Id { get; set; }

        public IPEndPoint RemotePoint { get; set; }

        /// <summary>
        /// 远程节点信息
        /// </summary>
        public NetPeer RemotePeer { get; set; }

        /// <summary>
        /// 消息队列
        /// </summary>
        private Queue<NetworkMessage> _messageQueue = new Queue<NetworkMessage>();

        /// <summary>
        /// 会话是否已连接
        /// </summary>
        public bool IsConnected => RemotePeer != null && RemotePeer.ConnectionState == ConnectionState.Connected;

        /// <summary>
        /// 最后活动时间
        /// </summary>
        public DateTime LastActiveTime { get; internal set; } = DateTime.Now;

        /// <summary>
        /// 断开连接回调
        /// </summary>
        public Action<NeterSession>? OnDisconnected { get; set; }

        /// <summary>
        /// 接收消息回调
        /// </summary>
        public Action<NeterSession, NetworkMessage>? OnMessageReceived { get; set; }

        /// <summary>
        /// 挂起的请求表（MessageId -> TaskCompletionSource）
        /// </summary>
        internal readonly ConcurrentDictionary<long, ETTask<NetworkMessage>> PendingRequests = new();

        internal ETTask? PendingConnectTask { get; set; }

        internal uint RequestCounter;
    }
}