using ECS;
using ECSGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSUnity
{
    public static class UnityAppStatic
    {
        public static bool SoundEditorTest { get; set; } = true;
        public static Actor MyActor { get; set; }
        public static Actor OtherActor { get; set; }
    }
}