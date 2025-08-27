//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using MessagePack;

//namespace ECSGame.Module.Neter
//{
//    /// <summary>
//    /// 网络消息处理器 - 处理NetworkMessage包装的消息
//    /// </summary>
//    public class NetworkMessageHandler : INetworkMessageHandler
//    {
//        private readonly Neter _neter;
//        private readonly Dictionary<uint, Func<object, NetworkMessage, NodeInfo, Task<bool>>> _handlers;

//        public NetworkMessageHandler(Neter neter)
//        {
//            _neter = neter;
//            _handlers = new Dictionary<uint, Func<object, NetworkMessage, NodeInfo, Task<bool>>>();
//        }

//        /// <summary>
//        /// 处理网络消息
//        /// </summary>
//        /// <param name="networkMessage">网络消息</param>
//        /// <param name="sender">发送者</param>
//        /// <returns>处理结果</returns>
//        public async Task<bool> HandleMessageAsync(NetworkMessage networkMessage, NodeInfo sender)
//        {
//            try
//            {
//                if (networkMessage.MessageTypeId == 0)
//                {
//                    Logger.Instance.Warning("Received message with invalid type ID");
//                    return false;
//                }

//                // 反序列化出具体消息类型
//                var content = networkMessage.GetContent();
//                if (content == null)
//                {
//                    Logger.Instance.Warning($"Failed to extract content from message type {networkMessage.MessageTypeId}");
//                    return false;
//                }

//                if (_handlers.TryGetValue(networkMessage.MessageTypeId, out var handler))
//                {
//                    return await handler(content, networkMessage, sender);
//                }
//                else if (_neter.MessageHandlers.TryGetValue(UintMessageSerializer.TypeIdToTypeMap[networkMessage.MessageTypeId], out var messageHandler))
//                {
//                    // 获取应用实例
//                    object app = _neter;
//                    var appProperty = _neter.GetType().GetProperty("App");
//                    if (appProperty != null)
//                    {
//                        var appInstance = appProperty.GetValue(_neter);
//                        if (appInstance != null)
//                        {
//                            app = appInstance;
//                        }
//                    }

//                    // 尝试用反序列化后的类型调用HandleAsync
//                    var method = messageHandler.GetType().GetMethod("HandleAsync", new[] { content.GetType(), typeof(NetworkMessage), typeof(NodeInfo), app.GetType() });
//                    if (method != null)
//                    {
//                        var task = (Task<bool>)method.Invoke(messageHandler, new object[] { content, networkMessage, sender, app });
//                        return await task;
//                    }

//                    // 兜底调用原始接口
//                    // 尝试找到合适的HandleAsync方法
//                    var basicMethod = messageHandler.GetType().GetMethod("HandleAsync", new[] { typeof(NetworkMessage), typeof(NodeInfo), app.GetType() });
//                    if (basicMethod != null)
//                    {
//                        var task = (Task<bool>)basicMethod.Invoke(messageHandler, new object[] { networkMessage, sender, app });
//                        return await task;
//                    }

//                    // 如果找不到合适的方法，尝试不带app参数的版本
//                    var simpleMethod = messageHandler.GetType().GetMethod("HandleAsync", new[] { typeof(NetworkMessage), typeof(NodeInfo) });
//                    if (simpleMethod != null)
//                    {
//                        var task = (Task<bool>)simpleMethod.Invoke(messageHandler, new object[] { networkMessage, sender });
//                        return await task;
//                    }

//                    Logger.Instance.Warning($"No suitable HandleAsync method found for message handler {messageHandler.GetType().Name}");
//                    return false;
//                }

//                Logger.Instance.Warning($"No handler registered for message type {networkMessage.MessageTypeId}");
//                return false;
//            }
//            catch (Exception ex)
//            {
//                Logger.Instance.Error($"Error handling network message: {ex.Message}", ex);
//                return false;
//            }
//        }
//    }
//}
