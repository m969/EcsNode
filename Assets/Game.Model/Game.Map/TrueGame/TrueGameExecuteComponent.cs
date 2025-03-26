using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ECSGame
{
    public class TrueGameExecuteComponent : EcsComponent
    {
        // 输入指令队列（线程安全）
        public readonly Dictionary<long, List<PlayerInput>> InputQueue = new();

        public readonly object _locker = new();
    } 
}
