using System.Collections.Generic;

namespace ECSGame.ChaseModule
{
    /// <summary>声明追踪流程的停止条件。</summary>
    public interface IStopConditionConfig
    {
        /// <summary>条件类型标识。</summary>
        string ConditionType { get; }

        /// <summary>条件参数表。</summary>
        Dictionary<string, string> Params { get; }
    }
}
