using System;
using System.Collections.Generic;

namespace ECS
{
    /// <summary>
    /// </summary>
    public class EcsObject
    {
        public long Id { get; set; }
        public Dictionary<Type, EcsComponent> Components { get; set; } = new ();

        public virtual T AddComponent<T>(Action<T> beforeAwake = null) where T : EcsComponent, new()
        {
            var component = new T();
            component.Id = DateTime.UtcNow.Ticks;
            Components.Add(typeof(T), component);
            beforeAwake?.Invoke(component);
            return component;
        }

        public T GetComponent<T>() where T : EcsComponent, new()
        {
            Components.TryGetValue(typeof(T), out var component);
            return component as T;
        }
    }
}