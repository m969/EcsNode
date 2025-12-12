// using ECS;
// using ECSGame.ChaseModule;
// using ECSGame.Module.Building;
// using ECSGame.Module.GridBased;
// using ECSGame.TaskModule;
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;

// namespace ECSGame.UnitDispatchModule
// {
//     public partial class UnitDispatcherSystem :
//         IUpdate<UnitDispatcher>,
//         IOnDispatchStarted,
//         IOnDispatchTimeout,
//         IOnDispatchCompleted
//     {
//         public void Update(UnitDispatcher entity)
//         {
//             if (entity.DispatchCount <= 0)
//             {
//                 return;
//             }
//             DispatchStateSystem.Tick(entity, UnityEngine.Time.deltaTime);
//         }
//     }
// }
