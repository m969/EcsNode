using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;

namespace ECSGame
{
    /// <summary>
    /// 领域事件：玩家注册（聚合名+业务动作）（Player+Register）
    /// </summary>
    public class PlayerRegisterEvent : AEventRun<EcsEntity>
    {
        protected override async ETTask Run(EcsEntity entity)
        {

        }
    }
}
