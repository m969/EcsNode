using ECS;
using ET;
using System;

namespace ECSGame.Module.Message
{
    public interface IMessageLoginRequestSystem : IDispatch
    {
        void LoginRequestHandle(EcsEntity caller, LoginRequest request, ETTask<LoginResult> task);
    }

    public static class MessageRequestExtensions
    {
        public static ETTask<LoginResult> LoginRequestAsync(
            this EcsEntity entity,
            LoginRequest request)
        {
            var task = ETTask<LoginResult>.Create();
            // 派发给所有实现 IMessageLoginRequestSystem 的系统
            entity.Dispatch<IMessageLoginRequestSystem>(sys => sys.LoginRequestHandle(entity, request, task));
            return task;
        }
    }
}