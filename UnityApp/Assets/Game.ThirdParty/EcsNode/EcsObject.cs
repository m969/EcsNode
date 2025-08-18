using System;
using System.Collections.Generic;
using System.Linq;

namespace ECS
{
    /// <summary>
    /// </summary>
    public abstract class EcsObject
    {
        public static void Destroy(EcsObject ecsObject)
        {
            if (ecsObject is EcsEntity entity)
            {
                entity.InstanceId = 0;

                var components = entity.Components.Values.ToArray();
                foreach (var item in components)
                {
                    //entity.DriveDestroy(item);
                    entity.EcsNode.DriveComponentSystems(entity, item, typeof(IDestroy));
                }
                foreach (var item in components)
                {
                    entity.Components.Remove(item.GetType());
                }

                var children = entity.Id2Children.Values.ToArray();
                foreach (var item in children)
                {
                    Destroy(item);
                }

                entity.Parent.RemoveChild(entity);
            }

            if (ecsObject is EcsComponent component)
            {
                //component.Entity.DriveDestroy(component);
                component.Entity.EcsNode.DriveComponentSystems(component.Entity, component, typeof(IDestroy));
                component.Entity.Components.Remove(component.GetType());
            }
        }
    }
}