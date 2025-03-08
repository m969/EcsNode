using System;
using System.Collections.Generic;
using System.Reflection;

namespace ECS
{
    public class SystemInfo
    {
        public object System { get; set; }
        public MethodInfo Action { get; set; }
    }

    public class Debug
    {
        public static void Log(object log)
        {
#if UNITY_EDITOR
            //UnityEngine.Debug.Log(log);
#endif
        }

        public static void LogError(object log)
        {
#if UNITY_EDITOR
            //UnityEngine.Debug.Log(log);
#endif
        }
    }

    public class EcsNode : EcsEntity
    {
        public Dictionary<long, EcsEntity> AllEntities { get; set; } = new();
        public Dictionary<Type, List<EcsEntity>> Type2Entities { get; set; }= new();

        public Dictionary<Type, IEcsSystem> AllSystems { get; set; }= new();
        public Dictionary<Type, Dictionary<Type, SystemInfo>> AllEntitySystems { get; set; }= new();
        public Dictionary<(Type, Type), Dictionary<Type, SystemInfo>> AllEntityComponentSystems { get; set; }= new();
        public Dictionary<Type, SystemInfo> AllUpdateSystems { get; set; }= new();
        //public Dictionary<Type, Dictionary<Type, SystemInfo>> AllUpdateComponentSystems { get; set; }= new();
        public List<Type> DriveTypes { get; set; } = new();
        public Type[] AllTypes { get; set; }

        public void RegisterDrive<T>()
        {
            DriveTypes.Add(typeof(T));
        }

        public void AddSystems(Type[] types)
        {
            AllTypes = types;

            var allSystems = new Dictionary<Type, IEcsSystem>();
            var allEntitySystems = new Dictionary<Type, Dictionary<Type, SystemInfo>>();
            var allEntityComponentSystems = new Dictionary<(Type, Type), Dictionary<Type, SystemInfo>>();
            var allUpdateSystems = new Dictionary<Type, SystemInfo>();
            var updateEntityTypes = new List<Type>();

            foreach (var systemType in types)
            {
                if (systemType.BaseType == null)
                {
                    continue;
                }
                if (systemType.BaseType.BaseType == null)
                {
                    continue;
                }
                if (!systemType.BaseType.BaseType.IsAssignableFrom(typeof(IEcsSystem)))
                {
                    continue;
                }

                var system = systemType.Assembly.CreateInstance(systemType.Name) as IEcsSystem;
                allSystems.Add(systemType, system);

                if (system is IEcsEntitySystem ecsEntitySystem)
                {
                    var entityType = ecsEntitySystem.EntityType;
                    if (!allEntitySystems.TryGetValue(entityType, out var systems))
                    {
                        systems = new Dictionary<Type, SystemInfo>();
                        allEntitySystems.Add(entityType, systems);
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
                                    allUpdateSystems.Add(entityType, systemInfo);
                                    updateEntityTypes.Add(entityType);
                                }

                                break;
                            }
                        }
                    }
                }

                if (system is IEcsComponentSystem ecsEntitySystem2)
                {
                    var entityType = ecsEntitySystem2.EntityType;
                    var componentType = ecsEntitySystem2.ComponentType;
                    var tuple = (entityType, componentType);
                    allEntityComponentSystems.TryGetValue(tuple, out var pairs);
                    if (pairs == null)
                    {
                        pairs = new Dictionary<Type, SystemInfo>();
                        allEntityComponentSystems.Add(tuple, pairs);
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
                                pairs.Add(item, systemInfo);
                                break;
                            }
                        }
                    }
                }
            }

            AllSystems = allSystems;
            AllEntitySystems = allEntitySystems;
            AllEntityComponentSystems = allEntityComponentSystems;
            AllUpdateSystems = allUpdateSystems;
            UpdateEntityTypes = updateEntityTypes;
        }

        public T GetSystem<T>() where T : class, IEcsSystem, new()
        {
            var systemType = typeof(T);
            AllSystems.TryGetValue(systemType, out var system);
            return system as T;
        }

        public void DriveSystems(EcsEntity entity, Type entityType, Type driveType)
        {
            AllEntitySystems.TryGetValue(entityType, out var systems);
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
                    method.Invoke(system, new object[] { entity });
                }
            }
        }

        public void DriveSystems<T1>(T1 entity, Type driveType) where T1 : EcsEntity
        {
            DriveSystems(entity, typeof(EcsEntity), driveType);
            DriveSystems(entity, entity.GetType(), driveType);
        }

        public void DriveSystems<T1, T2>(T1 entity, T2 component, Type driveType) where T1 : EcsEntity where T2 : EcsComponent
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

        public List<Type> UpdateEntityTypes { get; set; } = new();
        public Dictionary<Type, List<EcsEntity>> UpdateEntities { get; set; } = new();
        public Queue<EcsEntity> AddEntities { get; set; } = new();
        public Queue<EcsEntity> RemoveEntities { get; set; } = new();

        public void DriveUpdate()
        {
            while (AddEntities.Count > 0)
            {
                var entity = AddEntities.Dequeue();
                var entityType = entity.GetType();
                if (!UpdateEntities.TryGetValue(entityType, out var list))
                {
                    list = new List<EcsEntity>();
                    UpdateEntities.Add(entityType, list);
                }
                list.Add(entity);
            }

            while (RemoveEntities.Count > 0)
            {
                var entity = RemoveEntities.Dequeue();
                var entityType = entity.GetType();
                if (UpdateEntities.TryGetValue(entityType, out var list))
                {
                    list.Remove(entity);
                }
            }

            var systems = AllUpdateSystems;
            foreach (var item in systems)
            {
                var entityType = item.Key;
                if (UpdateEntities.TryGetValue(entityType, out var entities))
                {
                    var systemInfo = item.Value;
                    var system = systemInfo.System;
                    var method = systemInfo.Action;
                    foreach (var entity in entities)
                    {
                        method.Invoke(system, new object[] { entity });
                    }
                }
            }
        }

        public void DriveFixedUpdate()
        {
            foreach (var item in AllUpdateSystems)
            {
                var entityType = item.Key;
                var entities = UpdateEntities[entityType];
                var systemInfo = item.Value;
                var system = systemInfo.System;
                var method = systemInfo.Action;
                foreach (var entity in entities)
                {
                    method.Invoke(system, new object[] { entity });
                }
            }
        }
    }
}