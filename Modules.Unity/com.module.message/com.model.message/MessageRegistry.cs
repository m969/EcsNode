using System;
using System.Collections.Generic;

namespace ECSGame.Module.Message
{
    /// <summary>
    /// 业务消息类型注册表
    /// </summary>
    public static class MessageRegistry
    {
        // 业务消息类型注册表
        public static readonly Dictionary<int, Type> BusinessMessageTypes = new Dictionary<int, Type>
        {
           { 1, typeof(LoginRequest) },
           { 2, typeof(LoginResult) },
        };
    }

    public class LoginRequest
    {
        public string Account { get; set; }
        public string Password { get; set; }
    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}