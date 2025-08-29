//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using MessagePack;
//using LiteNetLib;
//using LiteNetLib.Utils;

//namespace ECSGame.Module.Neter
//{
//    /// <summary>
//    /// 消息处理器接口
//    /// </summary>
//    public interface IMessageHandler
//    {
//        /// <summary>
//        /// 处理消息
//        /// </summary>
//        /// <param name="message">消息</param>
//        /// <param name="sender">发送者信息</param>
//        /// <param name="app">应用程序实例</param>
//        /// <returns>处理结果</returns>
//        Task<bool> HandleAsync(NetworkMessage message, NodeInfo sender);
//    }

//    /// <summary>
//    /// 泛型消息处理器接口
//    /// </summary>
//    /// <typeparam name="T">消息类型</typeparam>
//    public interface IMessageHandler<T> : IMessageHandler where T : class
//    {
//        /// <summary>
//        /// 处理特定类型的消息
//        /// </summary>
//        /// <param name="message">解包后的消息内容</param>
//        /// <param name="networkMessage">原始网络消息</param>
//        /// <param name="sender">发送者信息</param>
//        /// <param name="app">应用程序实例</param>
//        /// <returns>处理结果</returns>
//        Task<bool> HandleAsync(T message, NetworkMessage networkMessage, NodeInfo sender);
//    }

//    /// <summary>
//    /// 消息序列化器 - 使用uint类型ID进行高效序列化
//    /// </summary>
//    public static class MessageSerializer
//    {
//        /// <summary>
//        /// 序列化消息
//        /// </summary>
//        /// <param name="message">消息对象</param>
//        /// <returns>序列化后的字节数组</returns>
//        public static byte[] Serialize(NetworkMessage message)
//        {
//            return MessagePackSerializer.Serialize(message);
//        }

//        /// <summary>
//        /// 反序列化消息
//        /// </summary>
//        /// <typeparam name="T">消息类型</typeparam>
//        /// <param name="data">字节数组</param>
//        /// <returns>消息对象</returns>
//        public static T? Deserialize<T>(byte[] data) where T : class
//        {
//            var networkMessage = UintMessageSerializer.Deserialize(data) as NetworkMessage;
//            return networkMessage?.GetContent<T>();
//        }

//        /// <summary>
//        /// 反序列化消息
//        /// </summary>
//        /// <param name="data">字节数组</param>
//        /// <returns>消息对象</returns>
//        public static NetworkMessage? Deserialize(byte[] data)
//        {
//            return UintMessageSerializer.Deserialize(data) as NetworkMessage;
//        }

//        /// <summary>
//        /// 获取消息类型ID
//        /// </summary>
//        /// <param name="data">字节数组</param>
//        /// <returns>类型ID</returns>
//        public static uint? GetMessageTypeId(byte[] data)
//        {
//            return UintMessageSerializer.GetMessageTypeId(data);
//        }

//        /// <summary>
//        /// 获取消息类型
//        /// </summary>
//        /// <param name="data">字节数组</param>
//        /// <returns>消息类型</returns>
//        public static Type? GetMessageType(byte[] data)
//        {
//            var typeId = UintMessageSerializer.GetMessageTypeId(data);
//            if (typeId.HasValue && UintMessageSerializer.TypeIdToTypeMap.TryGetValue(typeId.Value, out var type))
//            {
//                return type;
//            }
//            return null;
//        }

//        /// <summary>
//        /// 注册自定义消息类型
//        /// </summary>
//        /// <param name="typeId">类型ID</param>
//        /// <param name="messageType">消息类型</param>
//        public static void RegisterMessageType(uint typeId, Type messageType)
//        {
//            UintMessageSerializer.RegisterMessageType(typeId, messageType);
//        }
//    }

//    #region 基础消息类型

//    /// <summary>
//    /// 节点信息消息
//    /// </summary>
//    [MessagePackObject]
//    public class NodeInfoMessage
//    {
//        [Key(0)]
//        public NodeType NodeType { get; set; }

//        [Key(1)]
//        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

//        [Key(2)]
//        public int MessageId { get; set; }

//        public NodeInfoMessage() { }
//    }

//    /// <summary>
//    /// 心跳消息
//    /// </summary>
//    [MessagePackObject]
//    public class HeartbeatMessage
//    {
//        [Key(0)]
//        public string NodeId { get; set; } = "";

//        [Key(1)]
//        public float Load { get; set; }

//        [Key(2)]
//        public int ConnectionCount { get; set; }

//        [Key(3)]
//        public Dictionary<string, object> Metrics { get; set; } = new Dictionary<string, object>();

//        public HeartbeatMessage() { }
//    }

//    /// <summary>
//    /// 发现消息
//    /// </summary>
//    [MessagePackObject]
//    public class DiscoveryMessage
//    {
//        [Key(0)]
//        public string NodeId { get; set; } = "";

//        [Key(1)]
//        public NodeType NodeType { get; set; }

//        [Key(2)]
//        public int Port { get; set; }

//        [Key(3)]
//        public List<string> Services { get; set; } = new List<string>();

//        public DiscoveryMessage() { }
//    }

//    /// <summary>
//    /// 节点离线消息
//    /// </summary>
//    [MessagePackObject]
//    public class NodeOfflineMessage
//    {
//        [Key(0)]
//        public string NodeId { get; set; } = "";

//        [Key(1)]
//        public NodeType NodeType { get; set; }

//        [Key(2)]
//        public string Reason { get; set; } = "";

//        public NodeOfflineMessage() { }
//    }

//    /// <summary>
//    /// 自定义数据消息
//    /// </summary>
//    [MessagePackObject]
//    public class CustomDataMessage
//    {
//        [Key(0)]
//        public string DataType { get; set; } = "";

//        [Key(1)]
//        public byte[] Data { get; set; } = Array.Empty<byte>();

//        [Key(2)]
//        public Dictionary<string, object> Headers { get; set; } = new Dictionary<string, object>();

//        public CustomDataMessage() { }
//    }

//    /// <summary>
//    /// RPC调用消息
//    /// </summary>
//    [MessagePackObject]
//    public class RpcCallMessage
//    {
//        [Key(0)]
//        public string CallId { get; set; } = "";

//        [Key(1)]
//        public string ServiceName { get; set; } = "";

//        [Key(2)]
//        public string MethodName { get; set; } = "";

//        [Key(3)]
//        public object[] Parameters { get; set; } = Array.Empty<object>();

//        public RpcCallMessage() { }
//    }

//    /// <summary>
//    /// RPC响应消息
//    /// </summary>
//    [MessagePackObject]
//    public class RpcResponseMessage
//    {
//        [Key(0)]
//        public string CallId { get; set; } = "";

//        [Key(1)]
//        public bool Success { get; set; } = true;

//        [Key(2)]
//        public object? Result { get; set; }

//        [Key(3)]
//        public string ErrorMessage { get; set; } = "";

//        [Key(4)]
//        public string StackTrace { get; set; } = "";

//        public RpcResponseMessage() { }
//    }
//    #endregion
//}