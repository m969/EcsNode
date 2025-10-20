using System.Collections.Generic;
using ECS;
using UnityEngine;

namespace ECSGame.ChaseModule
{
    /// <summary>处理追踪区域限制的系统。</summary>
    public partial class AreaLimitSystem : AComponentSystem<EcsEntity, AreaLimitComponent>
    {
        /// <summary>检测位置是否位于限制区域内。</summary>
        /// <param name="entity">追踪实体。</param>
        /// <param name="position">待检测位置。</param>
        /// <returns>是否处于限制区域内。</returns>
        public static bool CheckInside(EcsEntity entity, Vector3 position)
        {
            var component = entity.GetComponent<AreaLimitComponent>();
            var area = component.Area;
            if (area == null || area.Type == AreaType.None)
            {
                component.IsOutOfArea = false;
                return true;
            }

            var inside = area.Type switch
            {
                AreaType.Circle => CheckCircle(area, position),
                AreaType.Polygon => CheckPolygon(area, position),
                _ => true
            };

            component.IsOutOfArea = !inside;
            return inside;
        }

        /// <summary>按照配置处理越界行为。</summary>
        /// <param name="entity">追踪实体。</param>
        public static void HandleOutOfArea(EcsEntity entity)
        {
            var component = entity.GetComponent<AreaLimitComponent>();
            var area = component.Area;
            if (area == null)
            {
                return;
            }

            if (!component.IsOutOfArea)
            {
                return;
            }

            entity.Dispatch<IOnChaseAreaViolated>(handler => handler.OnChaseAreaViolated(entity, area.Strategy));

            switch (area.Strategy)
            {
                case OutOfAreaStrategy.None:
                    component.IsOutOfArea = false;
                    break;
                case OutOfAreaStrategy.Rollback:
                    // 留给外部根据RollbackStep执行实际回退逻辑
                    break;
                case OutOfAreaStrategy.StopChase:
                    ChaseSystem.StopChase(entity, "area_violation");
                    break;
            }
        }

        private static bool CheckCircle(IChaseAreaConfig area, Vector3 position)
        {
            var delta = position - area.Center;
            return delta.sqrMagnitude <= area.Radius * area.Radius;
        }

        private static bool CheckPolygon(IChaseAreaConfig area, Vector3 position)
        {
            var points = area.Points;
            if (points == null || points.Count < 3)
            {
                return true;
            }

            var count = points.Count;
            var inside = false;
            var testPoint = new Vector2(position.x, position.z);

            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                var pi = new Vector2(points[i].x, points[i].z);
                var pj = new Vector2(points[j].x, points[j].z);

                var intersect = ((pi.y > testPoint.y) != (pj.y > testPoint.y)) &&
                                (testPoint.x < (pj.x - pi.x) * (testPoint.y - pi.y) / (pj.y - pi.y + float.Epsilon) + pi.x);
                if (intersect)
                {
                    inside = !inside;
                }
            }

            return inside;
        }
    }
}
