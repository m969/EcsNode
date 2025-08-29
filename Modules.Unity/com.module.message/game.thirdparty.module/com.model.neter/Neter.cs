using ECS;
using ET;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ECSGame.Module.Neter
{
    public interface IHandleNetworkMessage
    {
        ETTask<bool> HandleNetworkMessage(object neter, NeterSession session, NetworkMessage message);
    }

    /// <summary>
    /// Neter - 代表一个可以进行网络通讯的进程
    /// </summary>
    public class Neter : EcsNode /*INetEventListener, IDeliveryEventListener, IDisposable*/
    {
        /// <summary>
        /// 节点唯一标识
        /// </summary>
        public string NodeId { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        public NodeType NodeType { get; set; }

        /// <summary>
        /// 本地监听端口
        /// </summary>
        public int LocalPort { get; set; }

        /// <summary>
        /// 节点状态
        /// </summary>
        public NodeStatus Status { get; set; } = NodeStatus.Offline;

        /// <summary>
        /// LiteNetLib网络管理器
        /// </summary>
        public NetManager NetManager { get; set; }

        /// <summary>
        /// 已连接的节点
        /// </summary>
        public ConcurrentDictionary<string, NodeInfo> ConnectedNodes { get; set; }

        public ConcurrentDictionary<long, NeterSession> ConnectedSessions { get; set; }
        public ConcurrentDictionary<long, NeterSession> ConnectingSessions { get; set; }

        /// <summary>
        /// 消息处理器
        /// </summary>
        //public ConcurrentDictionary<Type, IMessageHandler> MessageHandlers { get; private set; }

        /// <summary>
        /// 节点配置
        /// </summary>
        public NodeConfig Config { get; set; }

        //public object? NeterApp { get; set; }

        public IHandleNetworkMessage? HandleNetworkMessageSystem { get; set; }

        /// <summary>
        /// 网络统计信息
        /// </summary>
        public NetworkStatistics NetworkStats { get; set; }

        public Timer _heartbeatTimer;
        public Timer _discoveryTimer;
        public CancellationTokenSource CancellationTokenSource;

        /// <summary>
        /// 节点事件
        /// </summary>
        public event Action<NodeInfo>? NodeConnected;
        public event Action<NodeInfo>? NodeDisconnected;
        public event Action<string, NetworkMessage>? MessageReceived;

        // 本地节点信息缓存，用于创建会话时作为LocalNode
        public NodeInfo LocalNodeInfo;

        public NeterNetEventListener NeterNetEventListener { get; set; }

        public Neter(ushort ecsTypeId) : base(ecsTypeId)
        {

        }

        public readonly ConcurrentDictionary<uint, NeterSession> PendingRequestSessions = new();

        // helpers for system layer
        internal void SetLocalPort(int port) => LocalPort = port;
        internal void SetStatus(NodeStatus status) => Status = status;
    }
}