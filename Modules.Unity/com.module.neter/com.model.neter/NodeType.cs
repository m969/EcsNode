using System;

namespace ECSGame.Module.Neter
{
    /// <summary>
    /// 节点类型
    /// </summary>
    public enum NodeType
    {
        /// <summary>
        /// 未知节点
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 网关节点
        /// </summary>
        Gateway = 1,

        /// <summary>
        /// 登录节点
        /// </summary>
        Login = 2,

        /// <summary>
        /// 游戏节点
        /// </summary>
        Game = 3,

        /// <summary>
        /// 数据库节点
        /// </summary>
        DB = 4,

        /// <summary>
        /// 主控节点
        /// </summary>
        Master = 5,

        /// <summary>
        /// 日志节点
        /// </summary>
        Log = 6,

        /// <summary>
        /// 服务节点
        /// </summary>
        Service = 7,

        /// <summary>
        /// 数据节点
        /// </summary>
        Data = 8,

        /// <summary>
        /// 客户端节点
        /// </summary>
        Client = 9,

        /// <summary>
        /// 路由节点
        /// </summary>
        Router = 10,

        /// <summary>
        /// Actor中继服务器节点
        /// </summary>
        RelayServer = 11
    }
}