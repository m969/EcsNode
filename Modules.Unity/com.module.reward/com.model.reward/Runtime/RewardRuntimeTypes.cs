using System;
using System.Collections.Generic;

namespace ECSGame.RewardModule
{
    /// <summary>
    /// 运行期用于组合/展开的条目描述
    /// </summary>
    public struct RewardEntryDefinition
    {
        public RewardType Type;
        public int RefId;
        public long Amount;
        public Dictionary<string, string>? Meta;
    }

    /// <summary>
    /// 预览展开结果
    /// </summary>
    public struct ExpandResult
    {
        public List<RewardEntryDefinition> Entries;
        public List<string>? Logs;
    }

    /// <summary>
    /// 发放上下文
    /// </summary>
    public struct GrantContext
    {
        public long PlayerId;
        public string Source;
        public string Reason;
        public string TraceId;
        public string? DedupeKey;
        public string Region;
        public string Platform;
        public DateTime Time;
    }

    /// <summary>
    /// 发放结果
    /// </summary>
    public struct GrantResult
    {
        public GrantStatus Status;
        public List<RewardEntryDefinition> EntriesApplied;
        public List<(RewardEntryDefinition Entry, string Reason)> EntriesSkipped;
        public (ErrorDomain Domain, int Code, string Message)? Error;
        public string TxId;
        public List<string>? Logs;
    }
}
