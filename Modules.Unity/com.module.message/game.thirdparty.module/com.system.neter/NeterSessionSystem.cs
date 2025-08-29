using ECS;
using ET;
using LiteNetLib;
using MessagePack;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ECSGame.Module.Neter
{
    /// <summary>
    /// NeterSession 实体系统：封装会话业务方法
    /// </summary>
    public class NeterSessionSystem : AEntitySystem<NeterSession>, IAwake<NeterSession>, IInit<NeterSession>, IAfterInit<NeterSession>, IEnable<ECSGame.Module.Neter.NeterSession>, IDisable<ECSGame.Module.Neter.NeterSession>, IDestroy<ECSGame.Module.Neter.NeterSession>
    {
        public void Awake(NeterSession entity) { }
        public void Init(NeterSession entity) { }
        public void AfterInit(NeterSession entity) { }
        public void Enable(NeterSession entity) { }
        public void Disable(NeterSession entity) { }
        public void Destroy(NeterSession entity) { }

        /// <summary>
        /// 创建会话
        /// </summary>
        public static NeterSession CreateSession(Neter entity, IPEndPoint remoteNode)
        {
            var session = entity.AddChild<NeterSession>((session) =>
            {
                session.Neter = entity;
                session.Id = IdGenerator.GetNextId();
                session.RemotePoint = remoteNode;
            });
            return session;
        }

        /// <summary>
        /// 创建会话
        /// </summary>
        public static NeterSession CreateSession(Neter entity, NetPeer remoteNode)
        {
            var session = entity.AddChild<NeterSession>((session) =>
            {
                session.Neter = entity;
                session.Id = IdGenerator.GetNextId();
                session.RemotePeer = remoteNode;
                session.RemotePoint = remoteNode;
            });
            return session;
        }

        /// <summary>
        /// 连接到远程节点
        /// </summary>
        public static ETTask ConnectAsync(NeterSession entity, string? connectionKey = null)
        {
            if (entity.RemotePoint == null)
                throw new InvalidOperationException("Remote endpoint is null");

            var key = connectionKey ?? entity.Neter.Config.DefaultConnectionKey;
            Logger.Instance.Info($"连接远程节点 {key} {entity.RemotePoint}");
            var peer = entity.Neter.NetManager.Connect(entity.RemotePoint, key);
            entity.RemotePeer = peer;

            entity.Neter.ConnectingSessions.TryAdd(entity.Id, entity);
            var task = ETTask.Create();
            entity.PendingConnectTask = task;
            entity.LastActiveTime = DateTime.Now;
            return task;
        }

        /// <summary>
        /// 设置连接完成
        /// </summary>
        public static void SetConnectResult(NeterSession entity)
        {
            Logger.Instance.Info($"连接远程节点成功 {entity.RemotePoint}");
            entity.PendingConnectTask?.SetResult();
            entity.PendingConnectTask = null;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public static void Send(NeterSession entity, NetworkMessage message)
        {
            if (!entity.IsConnected)
                throw new Exception("会话未连接");

            var data = MessagePackSerializer.Serialize(message);
            entity.RemotePeer!.Send(data, DeliveryMethod.ReliableOrdered);
            entity.LastActiveTime = DateTime.Now;
        }

        /// <summary>
        /// 关闭会话
        /// </summary>
        public static void Close(NeterSession entity)
        {
            try
            {
                entity.RemotePeer?.Disconnect();
                entity.OnDisconnected?.Invoke(entity);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"关闭会话异常: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 处理业务消息（回调）
        /// </summary>
        public static void HandleMessage(NeterSession entity, NetworkMessage message)
        {
            entity.OnMessageReceived?.Invoke(entity, message);
        }

        /// <summary>
        /// 发送请求并等待响应
        /// </summary>
        public static async Task<NetworkMessage> Ask(NeterSession entity, NetworkMessage request)
        {
            if (!entity.IsConnected)
                throw new Exception("会话未连接");

            var task = ETTask<NetworkMessage>.Create();
            var requestId = ++entity.RequestCounter;

            var networkMessage = new NetworkMessage
            {
                SessionId = entity.Id,
                MessageId = requestId,
            };

            Send(entity, networkMessage);

            entity.PendingRequests[requestId] = task;
            entity.Neter.PendingRequestSessions.TryAdd(requestId, entity);

            var responeMessage = await task;
            return responeMessage;
        }

        /// <summary>
        /// 处理响应消息
        /// </summary>
        public static bool HandleResponseMessage(NeterSession entity, NetworkMessage message)
        {
            if (!entity.PendingRequests.TryGetValue(message.MessageId, out var task))
            {
                return false;
            }
            try
            {
                task.SetResult(message);
                entity.PendingRequests.TryRemove(message.MessageId, out _);
                return true;
            }
            catch (Exception ex)
            {
                task.SetException(ex);
                return true;
            }
        }
    }
}
