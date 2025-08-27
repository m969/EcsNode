using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using MessagePack;
using LiteNetLib;

namespace ECSGame.Module.Neter
{
    /// <summary>
    /// 网络统计信息
    /// </summary>
    public class NetworkStatistics
    {
        /// <summary>
        /// 发送的字节数
        /// </summary>
        public long BytesSent { get; set; }

        /// <summary>
        /// 接收的字节数
        /// </summary>
        public long BytesReceived { get; set; }

        /// <summary>
        /// 发送的消息数
        /// </summary>
        public long MessagesSent { get; set; }

        /// <summary>
        /// 接收的消息数
        /// </summary>
        public long MessagesReceived { get; set; }

        /// <summary>
        /// 成功传递的消息数
        /// </summary>
        public long MessagesDelivered { get; set; }

        /// <summary>
        /// 当前连接数
        /// </summary>
        public int ConnectionCount { get; set; }

        /// <summary>
        /// 错误计数
        /// </summary>
        public long ErrorCount { get; set; }

        /// <summary>
        /// 启动时间
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 运行时间
        /// </summary>
        public TimeSpan Uptime => DateTime.UtcNow - StartTime;

        /// <summary>
        /// 每秒消息数
        /// </summary>
        public double MessagesPerSecond
        {
            get
            {
                var totalSeconds = Uptime.TotalSeconds;
                return totalSeconds > 0 ? (MessagesReceived + MessagesSent) / totalSeconds : 0;
            }
        }

        /// <summary>
        /// 平均发送速率（字节/秒）
        /// </summary>
        public double AverageSendRate
        {
            get
            {
                var totalSeconds = Uptime.TotalSeconds;
                return totalSeconds > 0 ? BytesSent / totalSeconds : 0;
            }
        }

        /// <summary>
        /// 平均接收速率（字节/秒）
        /// </summary>
        public double AverageReceiveRate
        {
            get
            {
                var totalSeconds = Uptime.TotalSeconds;
                return totalSeconds > 0 ? BytesReceived / totalSeconds : 0;
            }
        }

        /// <summary>
        /// 消息传递成功率
        /// </summary>
        public double DeliverySuccessRate
        {
            get
            {
                return MessagesSent > 0 ? (double)MessagesDelivered / MessagesSent : 1.0;
            }
        }

        /// <summary>
        /// 重置统计信息
        /// </summary>
        public void Reset()
        {
            BytesSent = 0;
            BytesReceived = 0;
            MessagesSent = 0;
            MessagesReceived = 0;
            MessagesDelivered = 0;
            ErrorCount = 0;
            StartTime = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"Stats: Sent={BytesSent}B/{MessagesSent}M, Received={BytesReceived}B/{MessagesReceived}M, " +
                   $"Connections={ConnectionCount}, Uptime={Uptime:hh\\:mm\\:ss}, Rate={MessagesPerSecond:F1}msg/s";
        }
    }
}