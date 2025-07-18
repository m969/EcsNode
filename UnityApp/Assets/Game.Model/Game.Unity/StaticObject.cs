using ECS;
using ECSGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSUnity
{
    public static class StaticObject
    {
        public static EcsNode EcsNode { get; set; }
        public static TrueGame TrueGame { get; set; }
        public static bool SoundEditorTest { get; set; } = true;
    }
}