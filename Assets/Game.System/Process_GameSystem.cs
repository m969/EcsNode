using ECS;
using System.Collections;
using System.Collections.Generic;
using System;
using ECSUnity;
using System.Reflection;
using FairyGUI;
using Login;

//public class TestLoader : ILoader
//{
//    public bool FileExists(string specifier)
//    {
//        return specifier == "test.mjs";
//    }

//    public string ReadFile(string specifier, out string debugpath)
//    {
//        debugpath = "test.mjs";
//        return "console.log('test Runtime')";
//    }
//}

namespace ECSGame
{
    public class Process_GameSystem
    {
        public static void Init(EcsNode ecsNode, Assembly assembly)
        {
            ConsoleLog.Debug($"Process_GameSystem Init");

            var nodeType = ecsNode.GetComponent<ConfigComponent>().NodeType;

            var allTypes = assembly.GetTypes();
            var typeList = new List<Type>();
            typeList.AddRange(allTypes);

            ecsNode.AddSystems(typeList.ToArray());

            EcsNodeSystem.Create(ecsNode);
            ecsNode.AddComponent<SoundComponent>();
            ecsNode.AddComponent<UIComponent>();
            ecsNode.Init();

            ecsNode.GetComponent<ReloadComponent>().SystemAssembly = assembly;

            //ecsNode.EcsUpdate = new EcsNodeSystem();

            //if (nodeType == EcsNodeType.TrueAuthority)
            //{
            //    var game = TrueGameSystem.Create(ecsNode);
            //    game.Init();

            //    var actor = ActorSystem.Create(game, ecsNode.NewId());
            //    //actor.AddComponent<EntityViewComponent>();
            //    actor.GetComponent<CollisionComponent>().Layer = 1;
            //    actor.Init();

            //    var actor1 = ActorSystem.Create(game, ecsNode.NewId());
            //    actor1.AddComponent<AIComponent>();
            //    //actor1.AddComponent<EntityViewComponent>();
            //    actor1.GetComponent<CollisionComponent>().Layer = 2;
            //    actor1.Init();
            //}

            if (nodeType == EcsNodeType.LocalPrePlay)
            {
                var game = TrueGameSystem.Create(ecsNode);
                game.AddComponent<PlayerInputComponent>();
                game.Init();

                StaticObject.TrueGame = game;

                var actor = ActorSystem.Create(game, ecsNode.NewInstanceId());
                actor.AddComponent<FramePlayComponent>();
                actor.AddComponent<EntityViewComponent>();
                actor.GetComponent<CollisionComponent>().Layer = 1;
                actor.Init();

                var actor1 = ActorSystem.Create(game, ecsNode.NewInstanceId());
                actor1.AddComponent<FramePlayComponent>();
                actor1.AddComponent<EntityViewComponent>();
                actor1.GetComponent<CollisionComponent>().Layer = 2;
                actor1.AddComponent<AIComponent>();
                actor1.Init();

                game.MyActor = actor;
                game.OtherActor = actor1;
            }

            var groot = GRoot.inst;
            ReloadUI(ecsNode);

            //var loader = new TSLoader();
            //// UseRuntimeLoader在Runtime下会形成链式处理。在Editor下不生效。
            //// 执行顺序是Loader加入顺序的倒序

            //// 通过菜单命令可以快速把TS构建到Resources目录供DefaultLoader使用   
            //loader.UseRuntimeLoader(new DefaultLoader());
            //// Editor下打开PUERTS_TSLOADER_DISABLE_EDITOR_FEATURE可以测试runtime下的效果。
            //loader.UseRuntimeLoader(new TestLoader());

            //var env = new JsEnv(loader);
            ////env.ExecuteModule("test.mts");
            //env.ExecuteModule("main.mts");
        }

        public static void ReloadUI(EcsNode ecsNode)
        {
            foreach (var item in GRoot.inst.GetChildren())
            {
                item.Dispose();
            }
            var uiComp = ecsNode.GetComponent<UIComponent>();
            uiComp.Type2Windows.Clear();
            UIObjectFactory.Clear();
            UIPackage.RemoveAllPackages();

            LoginBinder.BindAll();
            UIPackage.AddPackage("FGUI/Login");

            UISystem.Show<UI_HomePageWindow>(beforeAwake: x =>
            {
                x.TrueGame = StaticObject.TrueGame;
            });
        }

        public static void Reload(EcsNode ecsNode, Assembly assembly)
        {
            ConsoleLog.Debug($"Process_GameSystem Reload");

            ecsNode.GetComponent<ReloadComponent>().SystemAssembly = assembly;

            var allTypes = assembly.GetTypes();
            var typeList = new List<Type>();
            typeList.AddRange(allTypes);

            ecsNode.AddSystems(typeList.ToArray());

            EventSystem.Reload(ecsNode);

            //ReloadUI(ecsNode);

            //foreach (var item in ecsNode.Id2Children.Values)
            //{
            //    if (item is Actor actor)
            //    {
            //        MoveSystem.SetSpeed(actor, 2);
            //    }
            //}
        }
    }
}
