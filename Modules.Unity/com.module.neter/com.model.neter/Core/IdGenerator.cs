using System;
using System.Threading;

namespace ECSGame.Module.Neter
{
    /// <summary>
    /// ID生成器 - 基于雪花算法生成分布式唯一ID
    /// </summary>
    public static class IdGenerator
    {
        private static readonly object _lock = new object();
        private static long _lastTimestamp = -1L;
        private static long _sequence = 0L;

        // 开始时间戳 (2023-01-01)
        private const long TWEPOCH = 1672531200000L;
        
        // 机器ID所占的位数
        private const int WORKER_ID_BITS = 10;
        
        // 数据中心ID所占的位数
        private const int DATACENTER_ID_BITS = 5;
        
        // 序列号所占的位数
        private const int SEQUENCE_BITS = 7;
        
        // 机器ID的最大值
        private const long MAX_WORKER_ID = -1L ^ (-1L << WORKER_ID_BITS);
        
        // 数据中心ID的最大值
        private const long MAX_DATACENTER_ID = -1L ^ (-1L << DATACENTER_ID_BITS);
        
        // 序列号的最大值
        private const long SEQUENCE_MASK = -1L ^ (-1L << SEQUENCE_BITS);
        
        // 机器ID左移位数
        private const int WORKER_ID_SHIFT = SEQUENCE_BITS;
        
        // 数据中心ID左移位数
        private const int DATACENTER_ID_SHIFT = SEQUENCE_BITS + WORKER_ID_BITS;
        
        // 时间戳左移位数
        private const int TIMESTAMP_LEFT_SHIFT = SEQUENCE_BITS + WORKER_ID_BITS + DATACENTER_ID_BITS;
        
        // 工作机器ID
        private static long _workerId;
        
        // 数据中心ID
        private static long _datacenterId;
        
        static IdGenerator()
        {
            // 初始化机器ID和数据中心ID
            _workerId = (long)(new Random().NextDouble() * MAX_WORKER_ID);
            _datacenterId = (long)(new Random().NextDouble() * MAX_DATACENTER_ID);
            
            Console.WriteLine($"ID生成器初始化: WorkerId={_workerId}, DatacenterId={_datacenterId}");
        }
        
        /// <summary>
        /// 设置机器ID和数据中心ID
        /// </summary>
        /// <param name="workerId">机器ID</param>
        /// <param name="datacenterId">数据中心ID</param>
        public static void Configure(long workerId, long datacenterId)
        {
            if (workerId > MAX_WORKER_ID || workerId < 0)
            {
                throw new ArgumentException($"Worker ID 超出范围，必须在 0 到 {MAX_WORKER_ID} 之间");
            }
            if (datacenterId > MAX_DATACENTER_ID || datacenterId < 0)
            {
                throw new ArgumentException($"Datacenter ID 超出范围，必须在 0 到 {MAX_DATACENTER_ID} 之间");
            }
            
            _workerId = workerId;
            _datacenterId = datacenterId;
        }
        
        /// <summary>
        /// 获取下一个ID
        /// </summary>
        /// <returns>分布式唯一ID</returns>
        public static long GetNextId()
        {
            lock (_lock)
            {
                long timestamp = GetCurrentTimestamp();
                
                // 如果当前时间小于上一次ID生成的时间戳，说明系统时钟回退过，应当抛出异常
                if (timestamp < _lastTimestamp)
                {
                    throw new InvalidOperationException(
                        $"系统时钟回退，拒绝生成ID。时间差: {_lastTimestamp - timestamp} 毫秒");
                }
                
                // 如果是同一时间生成的，则进行序列号递增
                if (_lastTimestamp == timestamp)
                {
                    _sequence = (_sequence + 1) & SEQUENCE_MASK;
                    // 序列号已经达到最大值，等待下一个毫秒
                    if (_sequence == 0)
                    {
                        timestamp = WaitNextMillis(_lastTimestamp);
                    }
                }
                else
                {
                    // 时间戳改变，重置序列号
                    _sequence = 0L;
                }
                
                // 保存最后的时间戳
                _lastTimestamp = timestamp;
                
                // 生成并返回ID
                return ((timestamp - TWEPOCH) << TIMESTAMP_LEFT_SHIFT) |
                       (_datacenterId << DATACENTER_ID_SHIFT) |
                       (_workerId << WORKER_ID_SHIFT) |
                       _sequence;
            }
        }
        
        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <returns>毫秒时间戳</returns>
        private static long GetCurrentTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        
        /// <summary>
        /// 等待下一个毫秒
        /// </summary>
        /// <param name="lastTimestamp">上次生成ID的时间戳</param>
        /// <returns>新的时间戳</returns>
        private static long WaitNextMillis(long lastTimestamp)
        {
            long timestamp = GetCurrentTimestamp();
            while (timestamp <= lastTimestamp)
            {
                timestamp = GetCurrentTimestamp();
            }
            return timestamp;
        }
    }
} 