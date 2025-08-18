using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;

namespace ECSGame
{
    /// <summary>
    /// 领域事件：玩家登录（聚合名+业务动作）（Player+Login）
    /// </summary>
    public class PlayerLoginEventHandler : AEventRun<TrueWorld, PlayerLoginEvent>
    {
        protected override async ETTask Run(TrueWorld game, PlayerLoginEvent loginEvent)
        {
            // 这里可以添加玩家登录的逻辑处理
            // 例如：验证玩家身份、加载玩家数据等
            // 目前只是一个示例，实际逻辑需要根据游戏需求来实现
            //await LoginSessionSystem.AskAsync(loginSession, loginRequest);//
        }
    }
}
