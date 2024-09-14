using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderData;

namespace ECS
{
    public class SystemInfo
    {
        public IEcsEntitySystem System { get; set; }
        public MethodInfo Action { get; set; }
    }

    public class EcsNode : EcsEntity
    {
        public Dictionary<long, EcsEntity> AllEntities { get; set; } = new();
        public Dictionary<Type, List<EcsEntity>> Type2Entities { get; set; }= new();
        public Dictionary<Type, IEcsDriveSystem> AllEcsSystems { get; set; }= new();
        //public Dictionary<Type, IEcsEntitySystem> AllEntitySystems { get; set; }= new();
        public Dictionary<Type, Dictionary<Type, SystemInfo>> AllEntitySystems { get; set; }= new();
        public Dictionary<(Type, Type), Dictionary<Type, SystemInfo>> AllEntityComponentSystems { get; set; }= new();
        public Dictionary<Type, SystemInfo> AllUpdateSystems { get; set; }= new();
        public Dictionary<Type, Dictionary<Type, SystemInfo>> AllUpdateComponentSystems { get; set; }= new();
        public Dictionary<Type, Dictionary<Type, IEcsEntitySystem>> BeDrivedSystems { get; set; }= new();
        public List<Type> DriveTypes { get; set; } = new();

        public void RegisterDrive<T>()
        {
            DriveTypes.Add(typeof(T));
        }

        public T RegisterEcsDriveSystem<T>() where T : class, IEcsDriveSystem, new()
        {
            var system = new T();
            AllEcsSystems.Add(system.GetType(), system);
            return system;
        }

        public T GetEcsDriveSystem<T>() where T : class, IEcsDriveSystem, new()
        {
            return AllEcsSystems[typeof(T)] as T;
        }
        
        public T RegisterEntitySystem<T>() where T : class, IEcsEntitySystem, new()
        {
            var system = new T();
            var systemType = system.GetType();
            //AllEntitySystems.Add(systemType, system);

            if (system is IEcsEntitySystem ecsEntitySystem)
            {
                var entityType = ecsEntitySystem.EntityType;
                if (!AllEntitySystems.TryGetValue(entityType, out var systems))
                {
                    systems = new Dictionary<Type, SystemInfo>();
                    AllEntitySystems.Add(entityType, systems);
                }

                var interfaces = systemType.GetInterfaces();
                foreach (var interfaci in interfaces)
                {
                    foreach (var item in DriveTypes)
                    {
                        if (interfaci.IsAssignableFrom(item))
                        {
                            var arr = item.Name.ToCharArray();
                            var methodName = string.Empty;
                            for (int i = 1; i < arr.Length; i++)
                            {
                                methodName += arr[i];
                            }
                            var systemAction = systemType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
                            var systemInfo = new SystemInfo() { System = system, Action = systemAction };
                            systems.Add(item, systemInfo);

                            if (item == typeof(IUpdate))
                            {
                                AllUpdateSystems.Add(entityType, systemInfo);
                            }

                            break;
                        }
                    }
                }
            }
            if (system is IEcsEntitySystem2 ecsEntitySystem2)
            {
                var entityType = ecsEntitySystem2.EntityType;
                var componentType = ecsEntitySystem2.ComponentType;
                var tuple = (entityType, componentType);
                AllEntityComponentSystems.TryGetValue(tuple, out var pairs);
                if (pairs == null)
                {
                    pairs = new Dictionary<Type, SystemInfo>();
                    AllEntityComponentSystems.Add(tuple, pairs);
                }

                var interfaces = systemType.GetInterfaces();
                foreach (var interfaci in interfaces)
                {
                    foreach (var item in DriveTypes)
                    {
                        if (interfaci.IsAssignableFrom(item))
                        {
                            var arr  = item.Name.ToCharArray();
                            var methodName = string.Empty;
                            for (int i = 1; i < arr.Length; i++)
                            {
                                methodName += arr[i];
                            }
                            var systemAction = systemType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
                            var systemInfo = new SystemInfo() { System = system, Action = systemAction };
                            pairs.Add(item, systemInfo);

                            if (item == typeof(IUpdate))
                            {
                                if (!AllUpdateComponentSystems.TryGetValue(entityType, out var updateSystems))
                                {
                                    updateSystems = new Dictionary<Type, SystemInfo>();
                                    AllUpdateComponentSystems.Add(entityType, updateSystems);
                                }
                                updateSystems.Add(componentType, systemInfo);
                            }

                            break;
                        }
                    }
                }
            }
            return system;
        }

        public T GetSystem<T>() where T : class, IEcsEntitySystem, new()
        {
            return AllEntitySystems[typeof(T)] as T;
        }

        public T GetSystems<T, T2>(T entity, T2 component) where T : EcsEntity, new() where T2 : IEcsComponent, new()
        {
            return AllEntityComponentSystems[(typeof(T), typeof(T2))] as T;
        }

        public void DriveSystems<T1, T2>(T1 entity, T2 component, Type driveType) where T1 : EcsEntity where T2 : IEcsComponent
        {
            AllEntityComponentSystems.TryGetValue((entity.GetType(), component.GetType()), out var systems);
            if (systems == null)
            {
                return;
            }
            foreach (var item in systems)
            {
                if (item.Key.IsAssignableFrom(driveType))
                {
                    var systemInfo = item.Value;
                    var system = systemInfo.System;
                    var method = systemInfo.Action;
                    method.Invoke(system, new object[] { entity, component });
                }
            }
        }

        public void DriveUpdate()
        {
            foreach (var item in AllUpdateComponentSystems)
            {
                var entityType = item.Key;
                var entities = Type2Entities[entityType];
                foreach (var entity in entities)
                {
                    foreach (var component in entity.components)
                    {
                        var componentType = component.Key;
                        if (item.Value.TryGetValue(componentType, out var systemInfo))
                        {
                            var system = systemInfo.System;
                            var method = systemInfo.Action;
                            method.Invoke(system, new object[] { entity, component.Value });
                        }
                    }
                }
            }
        }
    }
}