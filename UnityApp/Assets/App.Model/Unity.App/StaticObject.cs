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
        public static bool SoundEditorTest { get; set; } = true;
        public static TrueGame TrueGame { get; set; }
        public static Actor MyActor { get; set; }
        public static Actor OtherActor { get; set; }
    }
}