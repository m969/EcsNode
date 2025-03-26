using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECS.Unity
{
    public static class StaticUtils
    {
        public static EcsNode EcsNode { get; set; }
        public static bool SoundEditorTest { get; set; } = true;
    }
}