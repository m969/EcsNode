using ECS;
using System;
using System.Collections.Generic;

namespace ECSGame.Module.Message
{
    /// <summary>
    /// 在NeterMessageComponent初始化后派发，允许外部向id->type映射中追加/覆盖映射。
    /// </summary>
    public interface IOnRegisterNeterMessageTypes : IDispatch
    {
        void OnRegisterNeterMessageTypes(EcsEntity entity, IDictionary<int, Type> id2Type);
    }
}
