//using System;
//using System.Collections.Generic;
//using MessagePack;

//namespace ECSGame.Module.Neter
//{
//    /// <summary>
//    /// 消息类型ID定义 - 使用uint进行类型映射
//    /// </summary>
//    public static class MessageTypeIds
//    {
//        // 基础网络消息类型 (0-99)
//        public const uint NetworkMessage = 0;
//        public const uint NodeInfo = 1;
//        public const uint Heartbeat = 2;
//        public const uint Discovery = 3;
//        public const uint NodeOffline = 4;
//        public const uint CustomData = 5;
//        public const uint RpcCall = 6;
//        public const uint RpcResponse = 7;

//        // 代理消息类型 (100-199)
//        public const uint ProxyRegistration = 100;
//        public const uint ProxyUnregistration = 101;
//        public const uint ProxyMessageRequest = 102;
//        public const uint ProxyBroadcastRequest = 103;

//        // Actor消息类型 (200-299)
//        public const uint ActorMessageEnvelope = 200;
//        public const uint ActorSystemMessage = 201;
//        public const uint ActorResponseMessage = 202;
//        public const uint ActorRequestMessage = 203;
//        public const uint ActorNotificationMessage = 204;
//        public const uint ActorQueryMessage = 205;
//        public const uint ActorCommandMessage = 206;
//        public const uint ActorEventMessage = 207;

//        // Actor示例消息类型 (300-399)
//        public const uint SimpleTest = 300;
//        public const uint SimpleTestResponse = 301;

//        // 自定义消息类型 (1000+)
//        // 用户可以从1000开始定义自己的消息类型
//        public const uint CustomMessageStart = 1000;
//    }

//    /// <summary>
//    /// 使用uint映射的消息序列化器
//    /// </summary>
//    public static class UintMessageSerializer
//    {
//        // 类型ID到Type的映射表
//        internal static readonly Dictionary<uint, Type> TypeIdToTypeMap = new()
//        {
//            // 基础网络消息
//            { MessageTypeIds.NetworkMessage, typeof(NetworkMessage) },
//            { MessageTypeIds.NodeInfo, typeof(NodeInfoMessage) },
//            { MessageTypeIds.Heartbeat, typeof(HeartbeatMessage) },
//            { MessageTypeIds.Discovery, typeof(DiscoveryMessage) },
//            { MessageTypeIds.NodeOffline, typeof(NodeOfflineMessage) },
//            { MessageTypeIds.CustomData, typeof(CustomDataMessage) },
//            { MessageTypeIds.RpcCall, typeof(RpcCallMessage) },
//            { MessageTypeIds.RpcResponse, typeof(RpcResponseMessage) },

//            // Actor消息
//            //{ MessageTypeIds.ActorMessageEnvelope, typeof(Actor.ActorMessageEnvelope) }
//        };

//        // Type到类型ID的映射表（用于序列化）
//        internal static readonly Dictionary<Type, uint> TypeToTypeIdMap = new();

//        // 静态构造函数，初始化反向映射
//        static UintMessageSerializer()
//        {
//            foreach (var kvp in TypeIdToTypeMap)
//            {
//                TypeToTypeIdMap[kvp.Value] = kvp.Key;
//            }
//        }

//        public static void RegisterMessageType<T>(uint typeId) where T : class
//        {
//            RegisterMessageType(typeId, typeof(T));
//        }

//        /// <summary>
//        /// 注册自定义消息类型
//        /// </summary>
//        /// <param name="typeId">类型ID（建议从MessageTypeIds.CustomMessageStart开始）</param>
//        /// <param name="messageType">消息类型</param>
//        public static void RegisterMessageType(uint typeId, Type messageType)
//        {
//            if (TypeIdToTypeMap.ContainsKey(typeId))
//            {
//                throw new ArgumentException($"Message type ID {typeId} is already registered");
//            }

//            if (TypeToTypeIdMap.ContainsKey(messageType))
//            {
//                throw new ArgumentException($"Message type {messageType.Name} is already registered");
//            }

//            TypeIdToTypeMap[typeId] = messageType;
//            TypeToTypeIdMap[messageType] = typeId;
//        }

//        /// <summary>
//        /// 反序列化消息
//        /// </summary>
//        /// <param name="data">字节数组</param>
//        /// <returns>消息对象</returns>
//        public static NetworkMessage? Deserialize(byte[] data)
//        {
//            try
//            {
//                var networkMessage = MessagePackSerializer.Deserialize<NetworkMessage>(data);
//                return networkMessage;
//            }
//            catch (Exception ex)
//            {
//                Logger.Instance.Error($"Error deserializing message", ex);
//                return null;
//            }
//        }

//        /// <summary>
//        /// 获取消息类型ID
//        /// </summary>
//        /// <param name="data">字节数组</param>
//        /// <returns>类型ID，失败返回null</returns>
//        public static uint? GetMessageTypeId(byte[] data)
//        {
//            try
//            {
//                var container = MessagePackSerializer.Deserialize<MessageContainer>(data);
//                return container?.TypeId;
//            }
//            catch
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// 获取注册的消息类型信息
//        /// </summary>
//        /// <returns>类型ID到类型名称的映射</returns>
//        public static Dictionary<uint, string> GetRegisteredTypes()
//        {
//            var result = new Dictionary<uint, string>();
//            foreach (var kvp in TypeIdToTypeMap)
//            {
//                result[kvp.Key] = kvp.Value.Name;
//            }
//            return result;
//        }

//        /// <summary>
//        /// 检查消息类型是否已注册
//        /// </summary>
//        /// <param name="messageType">消息类型</param>
//        /// <returns>是否已注册</returns>
//        public static bool IsRegistered(Type messageType)
//        {
//            return TypeToTypeIdMap.ContainsKey(messageType);
//        }

//        /// <summary>
//        /// 检查类型ID是否已使用
//        /// </summary>
//        /// <param name="typeId">类型ID</param>
//        /// <returns>是否已使用</returns>
//        public static bool IsTypeIdUsed(uint typeId)
//        {
//            return TypeIdToTypeMap.ContainsKey(typeId);
//        }
//    }

//    /// <summary>
//    /// 消息容器 - 使用uint类型ID
//    /// </summary>
//    [MessagePackObject]
//    public class MessageContainer
//    {
//        /// <summary>
//        /// 消息类型ID
//        /// </summary>
//        [Key(0)]
//        public uint TypeId { get; set; }

//        /// <summary>
//        /// 序列化后的消息数据
//        /// </summary>
//        [Key(1)]
//        public byte[] MessageData { get; set; } = Array.Empty<byte>();

//        public MessageContainer() { }

//        public override string ToString()
//        {
//            string typeName = MessageTypeIdExtensions.GetTypeName(TypeId) ?? $"Unknown({TypeId})";
//            return $"MessageContainer: Type={typeName}, DataLength={MessageData?.Length ?? 0}";
//        }
//    }

//    /// <summary>
//    /// 消息类型ID扩展方法
//    /// </summary>
//    public static class MessageTypeIdExtensions
//    {
//        /// <summary>
//        /// 获取消息的类型ID
//        /// </summary>
//        /// <param name="message">消息对象</param>
//        /// <returns>类型ID，未注册返回null</returns>
//        public static uint? GetTypeId(this object message)
//        {
//            if (message == null) return null;
//            var type = message.GetType();
//            return UintMessageSerializer.TypeToTypeIdMap.TryGetValue(type, out var typeId) ? typeId : null;
//        }

//        /// <summary>
//        /// 获取类型ID对应的类型名称
//        /// </summary>
//        /// <param name="typeId">类型ID</param>
//        /// <returns>类型名称，未注册返回null</returns>
//        public static string? GetTypeName(uint typeId)
//        {
//            return UintMessageSerializer.TypeIdToTypeMap.TryGetValue(typeId, out var type)
//                ? type.Name
//                : null;
//        }
//    }
//}