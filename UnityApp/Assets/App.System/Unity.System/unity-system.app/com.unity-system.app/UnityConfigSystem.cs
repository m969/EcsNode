using ECS;
using ECSGame;
using System;
using System.Reflection;
using cfg.data;
using SimpleJSON;
using System.IO;
using UnityEngine;

namespace ECSUnity
{
    public class UnityConfigSystem : AComponentSystem<UnityApp, UnityConfigComponent>,
    IAwake<UnityApp, UnityConfigComponent>
    {
        public void Awake(UnityApp app, UnityConfigComponent component)
        {
            string gameConfDir = Path.Combine(Application.dataPath, "GameResources\\LubanConfigs\\GenerateDatas\\json"); // 替换为gen.bat中outputDataDir指向的目录
            var tables = new cfg.Tables(file => JSON.Parse(File.ReadAllText($"{gameConfDir}/{file}.json")));
            ItemConfig.Tables = tables;
            GridPlaneConfig.Tables = tables;
            ConsoleLog.Debug($"UnityConfigSystem Tables loaded.");
        }
    }
}