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
           { LoginRequest.MessageTypeId, typeof(LoginRequest) },
           { LoginResult.MessageTypeId, typeof(LoginResult) },
        };
    }

    [MessagePack.MessagePackObject]
    public class LoginRequest
    {
        public const int MessageTypeId = 1;

        [MessagePack.Key(0)]
        public string Account { get; set; }
        [MessagePack.Key(1)]
        public string Password { get; set; }
    }

    [MessagePack.MessagePackObject]
    public class LoginResult
    {
        public const int MessageTypeId = 2;

        [MessagePack.Key(0)]
        public bool Success { get; set; }
        [MessagePack.Key(1)]
        public string Message { get; set; }
    }
}