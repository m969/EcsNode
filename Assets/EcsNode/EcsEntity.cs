using System;
using System.Collections.Generic;

namespace ECS
{
    /// <summary>
    /// 实体是树形节点的机制，父子节点具有包含的属性，同类节点具有独立分离的属性，能同时满足系统和组件的需求
    /// </summary>
    public class EcsEntity : EcsObject
    {
        public static int IdIndex;
        public long Id { get; set; }
        public Dictionary<long, EcsEntity> Id2Children = new();
        public EcsEntity Parent { get; set; }

        public EcsNode EcsNode
        {
            get
            {
                if (this is EcsNode node)
                    return node;
                return Parent.EcsNode;
            }
        }

        public EcsNode GetEcsNode()
        {
            if (this is EcsNode node)
                return node;
            return Parent.GetEcsNode();
        }

        private void AddEntity(EcsEntity entity)
        {
            var ecsNode = GetEcsNode();
            ecsNode.AllEntities.Add(entity.Id, entity);
            var entityType = entity.GetType();
            if (!ecsNode.Type2Entities.TryGetValue(entityType, out var list))
            {
                list = new List<EcsEntity>();
                ecsNode.Type2Entities.Add(entityType, list);
            }
            list.Add(entity);
            if (ecsNode.UpdateEntityTypes.Contains(entityType))
            {
                ecsNode.AddEntities.Enqueue(entity);
            }
        }

        private void RemoveEntity(EcsEntity entity)
        {
            var ecsNode = GetEcsNode();
            ecsNode.AllEntities.Remove(entity.Id);
            var entityType = entity.GetType();
            if (ecsNode.Type2Entities.TryGetValue(entityType, out var list))
            {
                list.Remove(entity);
                if (ecsNode.UpdateEntityTypes.Contains(entityType))
                {
                    ecsNode.RemoveEntities.Enqueue(entity);
                }
            }
        }

        public T AddChild<T>(Action<T> beforeAwake = null) where T : EcsEntity, new()
        {
            var entity = new T();
            entity.Id = ++IdIndex;
            entity.Parent = this;
            Id2Children.Add(entity.Id, entity);
            beforeAwake?.Invoke(entity);
            AddEntity(entity);
            DriveAwake(entity);
            return entity;
        }

        public T GetChild<T>(long id) where T : EcsEntity, new()
        {
            Id2Children.TryGetValue(id, out var entity);
            return entity as T;
        }

        public void RemoveChild(EcsEntity entity)
        {
            Id2Children.Remove(entity.Id);
            RemoveEntity(entity);
        }

        public Dictionary<Type, EcsComponent> Components { get; set; } = new();
        public T AddComponent<T>(Action<T> beforeAwake = null) where T : EcsComponent, new()
        {
            var component = new T();
            component.Entity = this;
            Components.Add(typeof(T), component);
            beforeAwake?.Invoke(component);
            DriveAwake(component);
            return component;
        }

        public void RemoveComponent<T>() where T : EcsComponent, new()
        {
            Components.Remove(typeof(T));
        }

        public void RemoveComponent(Type type)
        {
            Components.Remove(type);
        }

        public T GetComponent<T>() where T : EcsComponent, new()
        {
            Components.TryGetValue(typeof(T), out var component);
            return component as T;
        }

        private void DriveAwake(EcsEntity entity)
        {
            var ecsNode = GetEcsNode();
            ecsNode.DriveSystems(entity, typeof(IAwake));
        }

        private void DriveAwake<T>(T component) where T : EcsComponent, new()
        {
            var ecsNode = GetEcsNode();
            ecsNode.DriveComponentSystems(this, component, typeof(IAwake));
        }

        public void Init()
        {
            var ecsNode = GetEcsNode();
            ecsNode.DriveSystems(this, typeof(IInit));
            foreach (var item in Components.Values)
            {
                ecsNode.DriveComponentSystems(this, item, typeof(IInit));
            }
        }
    }
}