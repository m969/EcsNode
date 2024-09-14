using System;
using System.Collections.Generic;

namespace ECS
{
    /// <summary>
    /// 实体是树形节点的机制，父子节点具有包含的属性，同类节点具有独立分离的属性，能同时满足系统和组件的需求
    /// </summary>
    public class EcsEntity
    {
        public long Id { get; set; }
        private Dictionary<long, EcsEntity> id2Children = new();
        public Dictionary<Type, IEcsComponent> components = new();
        public EcsEntity Parent { get; set; }

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
            if (!ecsNode.Type2Entities.TryGetValue(entity.GetType(), out var list))
            {
                list = new List<EcsEntity>();
                ecsNode.Type2Entities.Add(entity.GetType(), list);
            }
            list.Add(entity);
            //ecsNode.GetEcsDriveSystem<EcsAwakeDriveSystem>().Handle(ecsNode, entity);
        }
        
        public T AddChild<T>(Action<T> beforeAwake = null) where T : EcsEntity, new()
        {
            var entity = new T();
            entity.Id = DateTime.UtcNow.Ticks;
            entity.Parent = this;
            id2Children.Add(entity.Id, entity);
            beforeAwake?.Invoke(entity);
            AddEntity(entity);
            return entity;
        }

        public T AddComponent<T>(Action<T> beforeAwake = null) where T : IEcsComponent, new()
        {
            var component = new T();
            component.Id = DateTime.UtcNow.Ticks;
            components.Add(typeof(T), component);
            beforeAwake?.Invoke(component);
            Awake(component);
            return component;
        }

        private void Awake<T>(T component) where T : IEcsComponent, new()
        {
            var ecsNode = GetEcsNode();
            ecsNode.DriveSystems(this, component, typeof(IAwake));
        }

        public void Init()
        {
            var ecsNode = GetEcsNode();
            foreach (var item in components.Values)
            {
                ecsNode.DriveSystems(this, item, typeof(IInit));
            }
        }
    }
}