using System;
using System.Collections.Generic;

namespace ECS
{
    /// <summary>
    /// 实体是树形节点的机制，父子节点具有包含的属性，同类节点具有独立分离的属性，能同时满足系统和组件的需求
    /// </summary>
    public class EcsEntity : EcsObject
    {
        private Dictionary<long, EcsEntity> id2Children = new();
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
        }
        
        public T AddChild<T>(Action<T> beforeAwake = null) where T : EcsEntity, new()
        {
            var entity = new T();
            entity.Id = DateTime.UtcNow.Ticks;
            entity.Parent = this;
            id2Children.Add(entity.Id, entity);
            beforeAwake?.Invoke(entity);
            AddEntity(entity);
            DriveAwake(entity);
            return entity;
        }

        public override T AddComponent<T>(Action<T> beforeAwake = null)
        {
            var component = base.AddComponent(beforeAwake);
            DriveAwake(component);
            return component;
        }

        private void DriveAwake(EcsEntity entity)
        {
            var ecsNode = GetEcsNode();
            ecsNode.DriveSystems(entity, typeof(IAwake));
        }

        private void DriveAwake<T>(T component) where T : EcsComponent, new()
        {
            var ecsNode = GetEcsNode();
            ecsNode.DriveSystems(this, component, typeof(IAwake));
        }

        public void Init()
        {
            var ecsNode = GetEcsNode();
            foreach (var item in Components.Values)
            {
                ecsNode.DriveSystems(this, item, typeof(IInit));
            }
        }
    }
}