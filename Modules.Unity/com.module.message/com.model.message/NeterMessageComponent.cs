using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame.Module.Message
{
    /// <summary>
    /// Neter消息组件，存储消息Id与消息类型的双向映射。
    /// 仅承载数据，不包含业务逻辑。
    /// </summary>
    public class NeterMessageComponent : EcsComponent
    {
        /// <summary>
        /// 消息Id到消息类型的映射。
        /// </summary>
        public Dictionary<int, Type> Id2Type { get; set; }

        /// <summary>
        /// 消息类型到消息Id的映射。
        /// </summary>
        public Dictionary<Type, int> Type2Id { get; set; }
    }
}
