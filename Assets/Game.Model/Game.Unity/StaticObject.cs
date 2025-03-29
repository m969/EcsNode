using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECS.Unity
{
    public static class StaticObject
    {
        public static EcsNode EcsNode { get; set; }
        public static EcsNode PrePlayEcsNode { get; set; }
        public static bool SoundEditorTest { get; set; } = true;
    }
}