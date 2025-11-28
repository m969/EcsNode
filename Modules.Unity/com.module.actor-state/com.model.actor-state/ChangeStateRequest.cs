using ECS;

namespace ECSGame.ActorStateModule
{
    /// <summary>
    /// 切换状态请求结构
    /// </summary>
    public struct ChangeStateRequest
    {
        /// <summary>
        /// 目标状态类型
        /// </summary>
        public ActorStateType StateType;
        /// <summary>
        /// 开启或关闭该状态
        /// </summary>
        public bool Enable;
        /// <summary>
        /// 是否强制切换（忽略条件检查）
        /// </summary>
        public bool Force;
    }
}
