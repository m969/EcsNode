using ECS;
using System.Collections.Generic;

namespace ECSGame.UnitDispatchModule
{
    /// <summary>
    /// 派遣规则引用与运行时参数缓存组件。纯数据组件。
    /// </summary>
    public class DispatchRuleComponent : EcsComponent
    {
        /// <summary>配置 Id（规则入口）</summary>
        public int ConfigId { get; set; }

        /// <summary>运行时参数（供规则判断与扩展使用）</summary>
        public Dictionary<string, object> RuntimeParams { get; set; } = new Dictionary<string, object>();
    }
}
