using System;

namespace ECS.Fody
{
    /// <summary>
    /// 用于标记一个静态方法，使其在指定的目标方法调用完成后执行
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class AfterAttribute : Attribute
    {
        /// <summary>
        /// 目标方法的名称
        /// </summary>
        public string TargetMethodName { get; }

        /// <summary>
        /// 目标方法所在的类型（可选，如果为null则在同一类中查找）
        /// </summary>
        public Type TargetType { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="targetType">目标方法所在的类型</param>
        /// <param name="targetMethodName">目标方法名称</param>
        public AfterAttribute(Type targetType, string targetMethodName)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));
            if (string.IsNullOrEmpty(targetMethodName))
                throw new ArgumentException("目标方法名不能为空", nameof(targetMethodName));

            TargetType = targetType;
            TargetMethodName = targetMethodName;
        }
    }
}