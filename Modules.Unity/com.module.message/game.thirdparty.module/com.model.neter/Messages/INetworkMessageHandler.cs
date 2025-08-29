using System.Threading.Tasks;

namespace ECSGame.Module.Neter
{
    /// <summary>
    /// 网络消息处理器接口 - 处理NetworkMessage包装的消息
    /// </summary>
    public interface INetworkMessageHandler
    {
        /// <summary>
        /// 处理网络消息
        /// </summary>
        /// <param name="networkMessage">网络消息</param>
        /// <param name="sender">发送者</param>
        /// <returns>处理结果，如果消息被处理返回true，否则返回false</returns>
        Task<bool> HandleMessageAsync(NetworkMessage networkMessage, NodeInfo sender);
    }
}
