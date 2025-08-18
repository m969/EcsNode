using System;

namespace ECS.Fody
{
    /// <summary>
    /// 用于标记一个方法，使其在指定的目标按钮点击后执行
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class AfterClickAttribute : Attribute
    {
        /// <summary>
        /// 目标按钮的名称
        /// </summary>
        public string TargetButtonName { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="targetButtonName">目标按钮名称</param>
        public AfterClickAttribute(string targetButtonName)
        {
            if (string.IsNullOrEmpty(targetButtonName))
                throw new ArgumentException("目标按钮名不能为空", nameof(targetButtonName));
            TargetButtonName = targetButtonName;
        }
    }
}