using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;

namespace ECSGame
{
    /// <summary>
    /// EntityObj表示实体的场景GameObject，并添加实体通用的属性序列化显示组件用来运行时debug。
    /// </summary>
    public class EntityObjComponent : EcsComponent
    {
        public GameObject EntityObj { get; set; }
    }
}
