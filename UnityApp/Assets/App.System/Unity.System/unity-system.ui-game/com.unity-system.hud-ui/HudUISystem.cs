using ECS;
using ECSGame;
using ECS.Fody;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ECSGame.ChaseModule;

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
            hudUIComp.HealthSlider = hudUIComp.HudCanvas.GetComponentInChildren<UnityEngine.UI.Slider>();
            hudUIComp.DamagePopupPrefab = hudUIComp.HudCanvas.transform.Find("DamageText").gameObject;
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
            if (healthComp.Health <= 0)
            {
                hudUIComp.HealthSlider.gameObject.SetActive(false);
                return;
            }
            hudUIComp.HealthSlider.value = healthComp.Health / (float)healthComp.MaxHealth;
        }

        public static void NewDamagePopup(Actor entity, int damage)
        {
            var hudUIComp = entity.GetComponent<HudUIComponent>();
            var damagePopupObj = GameObject.Instantiate(hudUIComp.DamagePopupPrefab, hudUIComp.HudCanvas.transform);
            damagePopupObj.SetActive(true);
            var damageText = damagePopupObj.GetComponent<UnityEngine.UI.Text>();
            damageText.text = damage.ToString();
            GameObject.Destroy(damagePopupObj, 1f);
        }

        [After(typeof(AISystem), nameof(AISystem.StartNode))]
        public static void OnStartNode(AINode aiNode)
        {
            if (aiNode.AIAction is AttackAIAction)
            {
                var targetActor = ChaseSystem.GetCurrentTarget(aiNode.Entity) as Actor;
                if (targetActor != null)
                {
                    NewDamagePopup(targetActor, -30);
                }
            }
        }
    }
}
