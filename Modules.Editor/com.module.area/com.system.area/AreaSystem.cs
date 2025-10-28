using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;
using ECSGame.TaskModule;

namespace ECSGame
{
    public class AreaType
    {
        /// <summary>
        /// 地图区域
        /// </summary>
        public const int Map = 1;
        /// <summary>
        /// 活动副本
        /// </summary>
        public const int Activity = 2;
    }

    public class AreaSystem : AEntitySystem<Area>,
        IAwake<Area>,
        IInit<Area>
    {
        public static Area Create(EcsEntity gameWorld, long areaId)
        {
            var area = gameWorld.AddChild<Area>(areaId, beforeAwake: x => x.Type = AreaType.Map);
            return area;
        }

        public void Awake(Area entity)
        {

        }

        public void Init(Area entity)
        {

        }
    }
}
