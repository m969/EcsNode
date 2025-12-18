using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using TrueSync;

namespace ECSGame
{
    /// <summary>
    /// 
    /// </summary>
    public class PlayerRegisterEvent : IEvent
    {
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class PlayerLoginEvent : IEvent
    {
    }

    public class PlayerStartBuildEvent : IEvent
    {
        public long BuildingId { get; set;}
    }

    public class PlayerStartDispatchEvent : IEvent
    {
        public long BuildingId { get; set;}
    }
}
