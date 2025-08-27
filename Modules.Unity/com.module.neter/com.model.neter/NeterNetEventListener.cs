using ET;
using LiteNetLib;
using LiteNetLib.Utils;
using MessagePack;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ECSGame.Module.Neter
{
    /// <summary>
    /// LiteNetLib 事件监听器（系统层）
    /// </summary>
    public class NeterNetEventListener : INetEventListener
    {
        public Neter Neter { get; private set; }

        public NeterNetEventListener(Neter neter)
        {
            Neter = neter ?? throw new ArgumentNullException(nameof(neter));
            ConsoleLog.Debug("NeterNetEventListener initialized");
        }

        public void OnPeerConnected(NetPeer peer)
        {
            try
            {
                ConsoleLog.Debug($"Peer connected: {peer.RemoteId} from {peer}");
                Neter.NetworkStats.ConnectionCount++;

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

                Neter.ConnectedNodes.TryAdd(tempNodeId, nodeInfo);

                //var nodeInfoMessage = new NodeInfoMessage
                //{
                //    NodeType = Neter.NodeType,
                //    MessageId = 0
                //};
                //var networkMessage = NetworkMessage.Create(nodeInfoMessage);
                //networkMessage.SenderId = Neter.NodeId;

                //var data = MessageSerializer.Serialize(networkMessage);
                //peer.Send(data, DeliveryMethod.ReliableOrdered);
            }
            catch (Exception ex)
            {
                ConsoleLog.Error($"OnPeerConnected error: {ex.Message}");
            }
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            try
            {
                ConsoleLog.Debug($"Peer disconnected: {peer.RemoteId}, Reason: {disconnectInfo.Reason}");
                Neter.NetworkStats.ConnectionCount--;

                NodeInfo? nodeToRemove = null;
                foreach (var kv in Neter.ConnectedNodes)
                {
                    if (kv.Value.Peer == peer)
                    {
                        nodeToRemove = kv.Value;
                        Neter.ConnectedNodes.TryRemove(kv.Key, out _);
                        break;
                    }
                }

                if (nodeToRemove != null)
                {
                    NeterSystem.RaiseNodeDisconnected(Neter, nodeToRemove);
                }
            }
            catch (Exception ex)
            {
                ConsoleLog.Error($"OnPeerDisconnected error: {ex.Message}");
            }
        }

        public void OnNetworkError(IPEndPoint endPoint, System.Net.Sockets.SocketError socketError)
        {
            ConsoleLog.Error($"Network error from {endPoint}: {socketError}");
            Neter.NetworkStats.ErrorCount++;
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            try
            {
                Neter.NetworkStats.MessagesReceived++;
                Neter.NetworkStats.BytesReceived += reader.AvailableBytes;

                var data = reader.GetRemainingBytes();
                reader.Recycle();

                var message = MessagePackSerializer.Deserialize<NetworkMessage>(data);
                if (message == null)
                {
                    ConsoleLog.Error("Failed to deserialize message as NetworkMessage");
                    return;
                }

                if (message.SessionId != 0 && !Neter.ConnectedSessions.ContainsKey(message.SessionId))
                {
                    NodeInfo? remoteNode = null;
                    foreach (var node in Neter.ConnectedNodes.Values)
                    {
                        if (node.Peer == peer)
                        {
                            remoteNode = node;
                            break;
                        }
                    }

                    if (remoteNode != null)
                    {
                        var autoSession = NeterSessionSystem.CreateSession(Neter, peer);
                        Neter.ConnectedSessions.TryAdd(autoSession.Id, autoSession);
                        ConsoleLog.Debug($"Auto-created session. SessionId={autoSession.Id}, Remote={remoteNode.NodeId}");
                    }
                    else
                    {
                        Logger.Instance.Warning($"Unable to auto-create session: remote node not found for peer {peer.RemoteId}");
                    }
                }

                if (Neter.PendingRequestSessions.TryRemove(message.MessageId, out var reqSession))
                {
                    NeterSessionSystem.HandleResponseMessage(reqSession, message);
                    return;
                }

                if (message.SessionId != 0 && Neter.ConnectedSessions.TryGetValue(message.SessionId, out var session))
                {
                    NeterSystem.HandleMessage(Neter, message);
                }
                else
                {
                    ConsoleLog.Debug($"No session found for message {message.MessageId} with SessionId {message.SessionId}");
                }
            }
            catch (Exception ex)
            {
                ConsoleLog.Error($"Error processing received message: {ex.Message}");
            }
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            try
            {
                if (messageType == UnconnectedMessageType.Broadcast)
                {
                    var msg = MessagePackSerializer.Deserialize<NetworkMessage>(reader.GetRemainingBytes());
                    if (msg == null) return;
                    //var discovery = msg.GetContent<DiscoveryMessage>();
                    //if (discovery != null && discovery.NodeId != Neter.NodeId)
                    //{
                    //    _ = Task.Run(async () =>
                    //    {
                    //        try
                    //        {
                    //            var targetEndPoint = new IPEndPoint(remoteEndPoint.Address, discovery.Port);
                    //            await NeterSystem.ConnectToNodeAsync(Neter, targetEndPoint);
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            ConsoleLog.Error($"Discovery connect error: {ex.Message}");
                    //        }
                    //    });
                    //}
                }
            }
            catch (Exception ex)
            {
                ConsoleLog.Error($"Error processing unconnected message: {ex.Message}");
            }
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            foreach (var node in Neter.ConnectedNodes.Values)
            {
                if (node.Peer == peer)
                {
                    node.Latency = latency;
                    break;
                }
            }
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            try
            {
                if (Neter.ConnectedNodes.Count < Neter.Config.MaxConnections)
                {
                    request.Accept();
                }
                else
                {
                    request.Reject();
                }
            }
            catch (Exception ex)
            {
                ConsoleLog.Error($"OnConnectionRequest error: {ex.Message}");
                request.Reject();
            }
        }

        public void OnMessageDelivered(NetPeer peer, object userData)
        {
            Neter.NetworkStats.MessagesDelivered++;
        }
    }
}
