using ECS;
using System;

namespace ECSGame.AttackModule
{
    // 配置提供接口：外部实现并通过 Dispatch 提供配置
    public interface IAttackConfigProvider : IDispatch
    {
        IAttackConfig GetConfig(EcsEntity owner, int configId);
    }
}
