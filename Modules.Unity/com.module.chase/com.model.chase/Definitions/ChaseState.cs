namespace ECSGame.ChaseModule
{
    /// <summary>标识追踪流程处于的阶段。</summary>
    public enum ChaseState
    {
        /// <summary>空闲状态。</summary>
        Idle,

        /// <summary>正在寻找目标。</summary>
        Searching,

        /// <summary>跟随目标移动。</summary>
        Following,

        /// <summary>执行拦截动作。</summary>
        Intercepting,

        /// <summary>目标已丢失。</summary>
        Lost
    }
}
