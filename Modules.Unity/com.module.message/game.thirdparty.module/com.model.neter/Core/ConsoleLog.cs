using System;

namespace ECSGame.Module.Neter
{
    /// <summary>
    /// 控制台日志类，用于调试输出
    /// </summary>
    public static class ConsoleLog
    {
        /// <summary>
        /// 输出调试信息
        /// </summary>
        public static void Debug(string message)
        {
            Console.WriteLine($"[DEBUG] {message}");
        }

        /// <summary>
        /// 输出信息
        /// </summary>
        public static void Info(string message)
        {
            Console.WriteLine($"[INFO] {message}");
        }

        /// <summary>
        /// 输出警告
        /// </summary>
        public static void Warning(string message)
        {
            Console.WriteLine($"[WARN] {message}");
        }

        /// <summary>
        /// 输出错误
        /// </summary>
        public static void Error(string message, Exception? ex = null)
        {
            Console.WriteLine($"[ERROR] {message}");
            if (ex != null)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                Console.WriteLine($"[ERROR] {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 输出网络相关信息
        /// </summary>
        public static void Network(string message)
        {
            Console.WriteLine($"[NET] {message}");
        }

        /// <summary>
        /// 输出Actor相关信息
        /// </summary>
        public static void Actor(string message)
        {
            Console.WriteLine($"[ACTOR] {message}");
        }
    }
} 