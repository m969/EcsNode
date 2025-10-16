using System.Collections.Generic;

namespace ECSGame.ChaseModule
{
    /// <summary>声明追踪流程的启动条件。</summary>
    public interface IStartConditionConfig
    {
        /// <summary>条件类型标识。</summary>
        string ConditionType { get; }

        /// <summary>条件参数表。</summary>
        Dictionary<string, string> Params { get; }
    }
}
