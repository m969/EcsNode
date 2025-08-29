using System;
using System.Collections.Generic;
using MessagePack;

namespace ECSGame.Module.Neter
{
    /// <summary>
    /// 网络消息包装类 - 用于包装和传输各种类型的消息
    /// </summary>
    [MessagePackObject]
    public class NetworkMessage
    {
        /// <summary>
        /// 消息唯一ID
        /// </summary>
        [Key(0)]
        public uint MessageId { get; set; }

        /// <summary>
        /// 消息类型ID - 用于标识被包装的消息类型
        /// </summary>
        [Key(1)]
        public uint MessageTypeId { get; set; }

        /// <summary>
        /// 发送时间戳
        /// </summary>
        [Key(2)]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 发送者节点ID
        /// </summary>
        [Key(3)]
        public string SenderId { get; set; } = "";

        /// <summary>
        /// 接收者节点ID
        /// </summary>
        [Key(4)]
        public string ReceiverId { get; set; } = "";

        /// <summary>
        /// 消息优先级
        /// </summary>
        [Key(5)]
        public byte Priority { get; set; } = 0;

        /// <summary>
        /// 是否需要确认
        /// </summary>
        [Key(6)]
        public bool RequireAck { get; set; } = false;

        /// <summary>
        /// 消息头部 - 可包含任意元数据
        /// </summary>
        [Key(7)]
        public Dictionary<string, object> Headers { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// 消息内容的序列化数据
        /// </summary>
        [Key(8)]
        public byte[] RawContent { get; set; } = Array.Empty<byte>();

        [Key(9)]
        public long SessionId { get; set; }

        [IgnoreMember]
        private object _cachedContent;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public NetworkMessage() { }

        ///// <summary>
        ///// 创建包装特定消息对象的NetworkMessage
        ///// </summary>
        ///// <typeparam name="T">消息类型</typeparam>
        ///// <param name="message">消息对象</param>
        ///// <returns>网络消息包装器</returns>
        //public static NetworkMessage Create<T>(T message) where T : class
        //{
        //    // 获取消息类型ID
        //    uint typeId = 0;

        //    // 尝试从已注册的类型中获取类型ID
        //    if (UintMessageSerializer.TypeToTypeIdMap.TryGetValue(typeof(T), out var id))
        //    {
        //        typeId = id;
        //    }

        //    // 序列化消息对象
        //    var content = MessagePackSerializer.Serialize(message);

        //    // 创建网络消息包装器
        //    var networkMessage = new NetworkMessage
        //    {
        //        MessageTypeId = typeId,
        //        RawContent = content,
        //    };

        //    // 缓存消息内容
        //    networkMessage._cachedContent = message;

        //    return networkMessage;
        //}

        ///// <summary>
        ///// 获取包装的消息对象
        ///// </summary>
        ///// <typeparam name="T">期望的消息类型</typeparam>
        ///// <returns>解包后的消息对象</returns>
        //public T GetContent<T>() where T : class
        //{
        //    // 如果已有解析好的内容且类型匹配，直接返回
        //    if (_cachedContent is T content)
        //    {
        //        return content;
        //    }

        //    // 否则尝试从原始数据解析
        //    if (RawContent == null || RawContent.Length == 0)
        //    {
        //        return null;
        //    }

        //    try
        //    {
        //        var result = MessagePackSerializer.Deserialize<T>(RawContent);
        //        // 缓存解析结果
        //        _cachedContent = result;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Instance.Error($"Error deserializing message content to {typeof(T).Name}: {ex.Message}");
        //        return null;
        //    }
        //}

        ///// <summary>
        ///// 根据消息类型ID获取包装的消息对象
        ///// </summary>
        ///// <returns>解包后的消息对象</returns>
        //public object GetContent()
        //{
        //    // 如果已有解析好的内容，直接返回
        //    if (_cachedContent != null)
        //    {
        //        return _cachedContent;
        //    }

        //    if (RawContent == null || RawContent.Length == 0 || MessageTypeId == 0)
        //    {
        //        return null;
        //    }

        //    try
        //    {
        //        // 尝试查找对应的类型
        //        if (UintMessageSerializer.TypeIdToTypeMap.TryGetValue(MessageTypeId, out var messageType))
        //        {
        //            var content = MessagePackSerializer.Deserialize(messageType, RawContent);
        //            // 缓存解析结果
        //            _cachedContent = content;
        //            return content;
        //        }
        //        else
        //        {
        //            Logger.Instance.Warning($"Unknown message type ID: {MessageTypeId}");
        //            return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Instance.Error($"Error deserializing message content: {ex.Message}");
        //        return null;
        //    }
        //}

        ///// <summary>
        ///// 设置消息内容
        ///// </summary>
        ///// <typeparam name="T">内容类型</typeparam>
        ///// <param name="content">内容对象</param>
        //public void SetContent<T>(T content) where T : class
        //{
        //    if (content == null)
        //    {
        //        RawContent = Array.Empty<byte>();
        //        _cachedContent = null;
        //        MessageTypeId = 0;
        //        return;
        //    }

        //    try
        //    {
        //        var contentType = content.GetType();

        //        // 更新类型ID
        //        if (UintMessageSerializer.TypeToTypeIdMap.TryGetValue(contentType, out var typeId))
        //        {
        //            MessageTypeId = typeId;
        //        }

        //        // 序列化内容
        //        RawContent = MessagePackSerializer.Serialize(content);

        //        // 缓存对象
        //        _cachedContent = content;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Instance.Error($"Error serializing content: {ex.Message}");
        //        RawContent = Array.Empty<byte>();
        //        _cachedContent = null;
        //    }
        //}

        /// <summary>
        /// 设置消息头部值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void SetHeader(string key, object value)
        {
            Headers[key] = value;
        }

        /// <summary>
        /// 获取消息头部值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>头部值</returns>
        public T GetHeader<T>(string key, T defaultValue = default)
        {
            if (Headers.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }
    }
}