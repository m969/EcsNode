// using ECS;
// using ECSUnity;
// using ET;
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using TrueSync;

// namespace ECSGame
// {
//     public static class DomainSystem
//     {
//         public static Game AddGame(int gameType, Assembly systemAssembly)
//         {
//             var game = GameSystem.Create(systemAssembly);
//             game.Type = gameType;
//             EcsDomain.AddNode(game);
//             EcsDomain.Game = game;
//             return game;
//         }

//         public static World AddWorld(Assembly systemAssembly)
//         {
//             var gameWorld = WorldSystem.Create(systemAssembly);
//             gameWorld.Init();
//             EcsDomain.AddNode(gameWorld);
//             EcsDomain.World = gameWorld;
//             return gameWorld;
//         }

//         public static TrueWorld AddTrueWorld(Assembly systemAssembly)
//         {
//             var trueWorld = TrueWorldSystem.Create(systemAssembly);
//             trueWorld.Init();
//             EcsDomain.AddNode(trueWorld);
//             EcsDomain.TrueWorld = trueWorld;
//             return trueWorld;
//         }

//         public static Player AddPlayer(Assembly systemAssembly)
//         {
//             var player = PlayerSystem.Create(systemAssembly);
//             player.Init();
//             EcsDomain.AddNode(player);
//             EcsDomain.Player = player;
//             return player;
//         }
//     }
// }
