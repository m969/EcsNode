
using System;
using System.Collections.Generic;

namespace ECS
{
    public interface IEcsComponent
    {

    }

    /// <summary>
    /// 组件是分离组合的机制，各个组件平级且分离
    /// </summary>
    public class EcsComponent : EcsObject, IEcsComponent
    {
        public EcsEntity Entity { get; set; }
    }
}