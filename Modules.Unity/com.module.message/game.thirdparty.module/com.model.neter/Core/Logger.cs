using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;

namespace ECSGame.Module.Neter
{
    /// <summary>
    /// 日志类，支持同时输出到控制台和文件
    /// </summary>
    public class Logger : IDisposable
    {
        private static Logger? _instance;
        private readonly string _logDirectory;
        private readonly string _currentLogFile;
        private readonly ConcurrentQueue<string> _messageQueue;
        private readonly Task _processTask;
        private volatile bool _isRunning;
        private readonly object _lock = new object();
        private readonly int _maxFileSizeBytes;
        private readonly int _maxLogFiles;

        private Logger(string logDirectory, int maxFileSizeBytes = 10 * 1024 * 1024, int maxLogFiles = 10)
        {
            _logDirectory = logDirectory;
            _maxFileSizeBytes = maxFileSizeBytes;
            _maxLogFiles = maxLogFiles;
            _messageQueue = new ConcurrentQueue<string>();
            _isRunning = true;

            // 确保日志目录存在
            Directory.CreateDirectory(_logDirectory);

            // 创建当前日志文件名
            _currentLogFile = Path.Combine(_logDirectory, $"neter_{DateTime.Now:yyyyMMdd_HHmmss}.log");

            // 启动处理线程
            _processTask = Task.Run(ProcessLogQueue);
        }

        /// <summary>
        /// 获取Logger实例
        /// </summary>
        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(Logger))
                    {
                        _instance ??= new Logger(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs"));
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 记录调试信息
        /// </summary>
        public void Debug(string message)
        {
            var logMessage = FormatMessage("DEBUG", message);
            Console.WriteLine(logMessage);
            _messageQueue.Enqueue(logMessage);
        }

        /// <summary>
        /// 记录信息
        /// </summary>
        public void Info(string message)
        {
            var logMessage = FormatMessage("INFO", message);
            Console.WriteLine(logMessage);
            _messageQueue.Enqueue(logMessage);
        }

        /// <summary>
        /// 记录警告
        /// </summary>
        public void Warning(string message)
        {
            var logMessage = FormatMessage("WARN", message);
            Console.WriteLine(logMessage);
            _messageQueue.Enqueue(logMessage);
        }

        /// <summary>
        /// 记录错误
        /// </summary>
        public void Error(string message, Exception? ex = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine(message);
            if (ex != null)
            {
                sb.AppendLine($"Exception: {ex.Message}");
                sb.AppendLine($"StackTrace: {ex.StackTrace}");
            }
            var logMessage = FormatMessage("ERROR", sb.ToString());
            Console.WriteLine(logMessage);
            _messageQueue.Enqueue(logMessage);
        }

        /// <summary>
        /// 记录网络相关信息
        /// </summary>
        public void Network(string message)
        {
            var logMessage = FormatMessage("NET", message);
            Console.WriteLine(logMessage);
            _messageQueue.Enqueue(logMessage);
        }

        /// <summary>
        /// 记录Actor相关信息
        /// </summary>
        public void Actor(string message)
        {
            var logMessage = FormatMessage("ACTOR", message);
            Console.WriteLine(logMessage);
            _messageQueue.Enqueue(logMessage);
        }

        private string FormatMessage(string level, string message)
        {
            return $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {message}";
        }

        private async Task ProcessLogQueue()
        {
            while (_isRunning)
            {
                try
                {
                    if (_messageQueue.TryDequeue(out string? message))
                    {
                        await WriteToFile(message);
                    }
                    else
                    {
                        await Task.Delay(100); // 没有消息时等待一段时间
                    }
                }
                catch (Exception ex)
                {
                    // 这里不使用Console.WriteLine，因为这是内部错误
                    File.AppendAllText(_currentLogFile, $"Error processing log message: {ex.Message}{Environment.NewLine}");
                }
            }

            // 处理剩余的消息
            while (_messageQueue.TryDequeue(out string? message))
            {
                try
                {
                    await WriteToFile(message);
                }
                catch
                {
                    // 忽略关闭时的错误
                }
            }
        }

        private async Task WriteToFile(string message)
        {
            try
            {
                // 检查文件大小
                var fileInfo = new FileInfo(_currentLogFile);
                if (fileInfo.Exists && fileInfo.Length >= _maxFileSizeBytes)
                {
                    await RollLogFile();
                }

                // 写入日志
                await File.AppendAllTextAsync(_currentLogFile, message + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // 这里不使用Console.WriteLine，因为这是内部错误
                File.AppendAllText(_currentLogFile, $"Error writing to log file: {ex.Message}{Environment.NewLine}");
            }
        }

        private async Task RollLogFile()
        {
            try
            {
                // 获取所有日志文件
                var logFiles = Directory.GetFiles(_logDirectory, "neter_*.log")
                    .OrderByDescending(f => f)
                    .ToList();

                // 如果超过最大文件数，删除最旧的文件
                while (logFiles.Count >= _maxLogFiles)
                {
                    var oldestFile = logFiles.Last();
                    File.Delete(oldestFile);
                    logFiles.RemoveAt(logFiles.Count - 1);
                }

                // 创建新的日志文件
                var newLogFile = Path.Combine(_logDirectory, $"neter_{DateTime.Now:yyyyMMdd_HHmmss}.log");
                using (var fs = File.Create(newLogFile))
                {
                    // 创建新文件
                }
            }
            catch (Exception ex)
            {
                // 这里不使用Console.WriteLine，因为这是内部错误
                File.AppendAllText(_currentLogFile, $"Error rolling log file: {ex.Message}{Environment.NewLine}");
            }
        }

        public void Dispose()
        {
            if (_isRunning)
            {
                _isRunning = false;
                _processTask.Wait(TimeSpan.FromSeconds(5)); // 等待处理完剩余消息
            }
        }
    }
} 