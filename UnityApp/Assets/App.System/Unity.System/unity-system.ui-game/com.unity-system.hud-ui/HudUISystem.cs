using ECS;
using ECSGame;
using ECS.Fody;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ECSUnity
{
    public class HudUISystem : AComponentSystem<Actor, HudUIComponent>,
        IInit<Actor, HudUIComponent>,
        IOnChange<Actor, HudUIComponent>
    {
        public void Init(Actor actor, HudUIComponent component)
        {
        }

        public void OnChange(Actor actor, HudUIComponent component)
        {

        }

        [After(typeof(ModelViewSystem), nameof(ModelViewSystem.SetModel))]
        public static void OnSetModel(EcsEntity entity, GameObject modelObj)
        {
            var hudUIComp = entity.GetComponent<HudUIComponent>();
            if (hudUIComp == null)
            {
                return;
            }
            if (modelObj.transform.childCount < 2)
            {
                return;
            }
            var canvasObj = modelObj.transform.GetChild(1);
            hudUIComp.HudCanvas = canvasObj.GetComponent<Canvas>();
        }

        [After(typeof(HealthSystem), nameof(HealthSystem.ChangeHealth))]
        public static void OnChangeHealth(Actor entity, int value)
        {
            var hudUIComp = entity.GetComponent<HudUIComponent>();
            if (hudUIComp == null)
            {
                return;
            }
            var healthComp = entity.GetComponent<HealthComponent>();
            if (hudUIComp.HealthSlider == null)
            {
                return;
            }
            hudUIComp.HealthSlider.value = healthComp.Health / (float)healthComp.MaxHealth;
        }
    }
}
