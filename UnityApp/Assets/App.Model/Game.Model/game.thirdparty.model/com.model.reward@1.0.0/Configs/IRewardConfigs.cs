using System;
using System.Collections.Generic;

namespace ECSGame.RewardModule
{
    /// <summary>
    /// 奖励类型
    /// </summary>
    public enum RewardType
    {
        Currency = 0,
        Item = 1,
        Exp = 2,
        Energy = 3,
        Custom = 9,
        LootTable = 100,
    }

    /// <summary>
    /// 发放状态
    /// </summary>
    public enum GrantStatus
    {
        Success,
        Partial,
        Failed,
        Queued
    }

    /// <summary>
    /// 错误域
    /// </summary>
    public enum ErrorDomain
    {
        ConfigMissing,
        ConditionNotMet,
        CapacityInsufficient,
        DedupeConflict,
        RateLimited,
        BackendError,
        Unknown
    }

    /// <summary>
    /// 奖励条目配置（最小粒度）
    /// </summary>
    public interface IRewardEntryConfig
    {
        int Id { get; }
        string Key { get; }
        RewardType Type { get; }
        int RefId { get; }
        long Amount { get; }
        Dictionary<string, string>? Meta { get; }
    }

    /// <summary>
    /// 奖励包配置（多个条目组合）
    /// </summary>
    public interface IRewardPackageConfig
    {
        int Id { get; }
        string Key { get; }
        int[] Entries { get; }
        string[]? Tags { get; }
        int Version { get; }
    }

    /// <summary>
    /// 掉落表条目
    /// </summary>
    public struct LootItem
    {
        public int EntryId;
        public int Weight;
    }

    /// <summary>
    /// 掉落表配置（可选）
    /// </summary>
    public interface IRewardLootTableConfig
    {
        int Id { get; }
        string Key { get; }
        List<LootItem> Items { get; }
    }

    /// <summary>
    /// 配置提供器接口
    /// </summary>
    public interface IRewardConfigProvider
    {
        IReadOnlyDictionary<int, IRewardEntryConfig> LoadEntries();
        IReadOnlyDictionary<int, IRewardPackageConfig> LoadPackages();
        IReadOnlyDictionary<int, IRewardLootTableConfig> LoadLootTables();
    }
}
