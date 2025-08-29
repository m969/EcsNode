using ECS;
using ET;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ECSGame.Module.Neter
{
    /// <summary>
    /// Neter 实体系统：承载 Neter 的业务方法
    /// </summary>
    public class NeterSystem : AEntitySystem<Neter>, IAwake<Neter>, IInit<Neter>, IAfterInit<Neter>, IUpdate<Neter>, IEnable<Neter>, IDisable<Neter>, IDestroy<Neter>
    {
        public void Awake(Neter entity) { }
        public void Init(Neter entity) { }
        public void AfterInit(Neter entity) { }
        public void Enable(Neter entity) { }
        public void Disable(Neter entity) { }
        public void Destroy(Neter entity) { Dispose(entity); }

        /// <summary>
        /// 创建 Neter 实体（指定节点ID）
        /// </summary>
        /// <param name="ecsTypeId">ECS 类型ID（由框架类型映射提供）</param>
        /// <param name="nodeId">节点唯一标识</param>
        /// <param name="nodeType">节点类型</param>
        /// <param name="config">节点配置（可选）</param>
        /// <returns>Neter 实体</returns>
        public static Neter Create(ushort ecsTypeId, string nodeId, NodeType nodeType, Type[] types, NodeConfig? config = null)
        {
            var neter = new Neter(ecsTypeId);

            // 初始化MessagePack序列化器
            //MessagePackInit.Initialize();

            neter.NodeId = string.IsNullOrEmpty(nodeId) ? Guid.NewGuid().ToString() : nodeId;
            neter.NodeType = nodeType;
            neter.Config = config ?? new NodeConfig();

            neter.ConnectedNodes = new ConcurrentDictionary<string, NodeInfo>();
            neter.ConnectedSessions = new ConcurrentDictionary<long, NeterSession>();
            neter.ConnectingSessions = new ConcurrentDictionary<long, NeterSession>();
            //MessageHandlers = new ConcurrentDictionary<Type, IMessageHandler>();
            neter.NetworkStats = new NetworkStatistics();
            neter.CancellationTokenSource = new CancellationTokenSource();

            neter.NeterNetEventListener = new NeterNetEventListener(neter);

            // 初始化LiteNetLib
            neter.NetManager = new NetManager(neter.NeterNetEventListener);
            ConfigureNetManager(neter);

            // 初始化组件
            //MessageRouter = new MessageRouter(this);

            neter.RegisterDrives(types);
            neter.RegisterSystems(types);
            return neter;
        }

        /// <summary>
        /// 启动网络节点
        /// </summary>
        public static async Task<bool> StartAsync(Neter entity, int port)
        {
            try
            {
                entity.SetLocalPort(port);

                bool started = entity.NetManager.Start(port);
                if (!started)
                {
                    Logger.Instance.Error($"Failed to start NetManager on port {port}");
                    return false;
                }

                try { entity.SetLocalPort(entity.NetManager.LocalPort); } catch { }

                entity.SetStatus(NodeStatus.Online);

                entity.LocalNodeInfo = new NodeInfo
                {
                    NodeId = entity.NodeId,
                    NodeType = entity.NodeType,
                    Status = NodeStatus.Online,
                    ConnectedTime = DateTime.UtcNow,
                    LastHeartbeat = DateTime.UtcNow,
                    EndPoint = new IPEndPoint(IPAddress.Loopback, entity.LocalPort)
                };

                //await StartServiceDiscoveryAsync(entity);

                Logger.Instance.Info($"Neter [{entity.NodeId}] started successfully on port {entity.LocalPort}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Failed to start Neter", ex);
                return false;
            }
        }

        /// <summary>
        /// 停止网络节点
        /// </summary>
        public static async Task StopAsync(Neter entity)
        {
            try
            {
                entity.SetStatus(NodeStatus.Offline);

                entity._heartbeatTimer?.Change(Timeout.Infinite, Timeout.Infinite);
                entity._discoveryTimer?.Change(Timeout.Infinite, Timeout.Infinite);

                entity.NetManager?.Stop();

                entity.CancellationTokenSource.Cancel();

                Logger.Instance.Info($"Neter [{entity.NodeId}] stopped");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Error stopping Neter", ex);
            }
        }

        /// <summary>
        /// 连接到远程节点
        /// </summary>
        public static async Task<NodeInfo> ConnectToNodeAsync(Neter entity, IPEndPoint endPoint, string? connectionKey = null)
        {
            try
            {
                Logger.Instance.Network($"Attempting to connect to {endPoint}...");

                var peer = entity.NetManager.Connect(endPoint, connectionKey ?? entity.Config.DefaultConnectionKey);
                if (peer == null)
                {
                    Logger.Instance.Error("Failed to create peer connection");
                    return null;
                }

                int timeout = 5000;
                int elapsed = 0;

                while (peer.ConnectionState != ConnectionState.Connected && elapsed < timeout)
                {
                    entity.NetManager.PollEvents();
                    await Task.Delay(100);
                    elapsed += 100;
                }

                if (peer.ConnectionState == ConnectionState.Connected)
                {
                    var tempNodeId = $"Remote-{peer.Id}";
                    var nodeInfo = new NodeInfo
                    {
                        NodeId = tempNodeId,
                        EndPoint = new IPEndPoint(peer.Address, peer.Port),
                        NodeType = NodeType.Unknown,
                        Status = NodeStatus.Online,
                        ConnectedTime = DateTime.UtcNow,
                        Peer = peer
                    };

                    entity.ConnectedNodes.TryAdd(tempNodeId, nodeInfo);

                    await Task.Delay(2000);
                    for (int i = 0; i < 10; i++)
                    {
                        entity.NetManager.PollEvents();
                        await Task.Delay(100);
                    }

                    var updatedNode = entity.ConnectedNodes.Values.FirstOrDefault(n => n.Peer == peer);
                    if (updatedNode != null)
                    {
                        //entity.RaiseNodeConnected(updatedNode);
                        return updatedNode;
                    }

                    return nodeInfo;
                }
                else
                {
                    Logger.Instance.Error($"Connection failed. Final state: {peer.ConnectionState}");
                    if (peer.ConnectionState != ConnectionState.Disconnected)
                    {
                        entity.NetManager.DisconnectPeer(peer);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Exception during connection to {endPoint}", ex);
                return null;
            }
        }

        public static void RaiseNodeDisconnected(Neter entity, NodeInfo node)
        {

        }

        /// <summary>
        /// 轮询网络并推进连接状态
        /// </summary>
        public void Update(Neter entity)
        {
            entity.NetManager?.PollEvents();

            foreach (var session in entity.ConnectingSessions.Values)
            {
                var peer = session.RemotePeer;
                if (peer == null)
                {
                    continue;
                }
                if (peer.ConnectionState == ConnectionState.Connected)
                {
                    Logger.Instance.Info($"session {session.Id} 已建立");
                    NeterSessionSystem.SetConnectResult(session);
                    entity.ConnectedSessions.TryAdd(session.Id, session);
                }
            }

            foreach (var session in entity.ConnectedSessions.Values)
            {
                entity.ConnectingSessions.TryRemove(session.Id, out var _);
            }
        }

        /// <summary>
        /// 内部：处理普通消息，派发到应用层
        /// </summary>
        public static void HandleMessage(Neter entity, NetworkMessage message)
        {
            if (entity.ConnectedSessions.TryGetValue(message.SessionId, out var session))
            {
                entity.HandleNetworkMessageSystem?.HandleNetworkMessage(entity, session, message);
            }
        }

        /// <summary>
        /// 内部：处理响应消息
        /// </summary>
        public static bool HandleResponseMessage(Neter entity, NetworkMessage message)
        {
            if (entity.PendingRequestSessions.TryRemove(message.MessageId, out var session))
            {
                return NeterSessionSystem.HandleResponseMessage(session, message);
            }
            return false;
        }

        /// <summary>
        /// NetManager 参数配置
        /// </summary>
        public static void ConfigureNetManager(Neter entity)
        {
            entity.NetManager.UpdateTime = entity.Config.UpdateTime;
            entity.NetManager.PingInterval = entity.Config.PingInterval;
            entity.NetManager.DisconnectTimeout = entity.Config.DisconnectTimeout;
            entity.NetManager.UnconnectedMessagesEnabled = true;
            entity.NetManager.BroadcastReceiveEnabled = true;
            entity.NetManager.AutoRecycle = true;
        }

        /// <summary>
        /// 发送局域网发现广播
        /// </summary>
        //public static async Task StartServiceDiscoveryAsync(Neter entity)
        //{
        //    var discoveryMessage = new DiscoveryMessage
        //    {
        //        NodeId = entity.NodeId,
        //        NodeType = entity.NodeType,
        //        Port = entity.LocalPort,
        //        Services = new List<string>(),
        //    };

        //    var writer = new NetDataWriter();
        //    writer.Put(MessageSerializer.Serialize(NetworkMessage.Create(discoveryMessage)));

        //    for (int port = 9000; port <= 9010; port++)
        //    {
        //        if (port != entity.LocalPort)
        //        {
        //            try { entity.NetManager.SendUnconnectedMessage(writer, new IPEndPoint(IPAddress.Loopback, port)); } catch { }
        //        }
        //    }

        //    for (int i = 1; i < 255; i++)
        //    {
        //        try { entity.NetManager.SendUnconnectedMessage(writer, new IPEndPoint(IPAddress.Parse($"192.168.1.{i}"), entity.LocalPort)); } catch { }
        //    }

        //    await Task.CompletedTask;
        //}

        /// <summary>
        /// 根据 Peer 查找节点
        /// </summary>
        public static NodeInfo FindNodeByPeer(Neter entity, NetPeer peer)
        {
            foreach (var node in entity.ConnectedNodes.Values)
            {
                if (node.Peer == peer)
                {
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        /// 连接接入判断
        /// </summary>
        public static bool ShouldAcceptConnection(Neter entity, ConnectionRequest request)
        {
            if (entity.ConnectedNodes.Count >= entity.Config.MaxConnections)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 负载估算
        /// </summary>
        public static float CalculateNodeLoad(Neter entity)
        {
            float connectionLoad = (float)entity.ConnectedNodes.Count / entity.Config.MaxConnections;
            float messageLoad = (float)Math.Min(1.0f, entity.NetworkStats.MessagesPerSecond / 1000.0f);
            return (connectionLoad + messageLoad) / 2.0f;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public static void Dispose(Neter entity)
        {
            _ = StopAsync(entity);
            entity._heartbeatTimer?.Dispose();
            entity._discoveryTimer?.Dispose();
            entity.CancellationTokenSource?.Dispose();
            entity.NetManager?.Stop();
        }
    }
}
