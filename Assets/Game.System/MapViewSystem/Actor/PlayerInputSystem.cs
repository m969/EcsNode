using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrueSync;
using UnityEngine;

namespace ECSGame
{
    public class PlayerInputSystem : AComponentSystem<TrueGame, PlayerInputComponent>,
IAwake<TrueGame, PlayerInputComponent>,
IInit<TrueGame, PlayerInputComponent>
    {
        public void Awake(TrueGame game, PlayerInputComponent component)
        {
            component.MoveJoystick = GameObject.Find("Canvas/Joystick_Move").GetComponent<VariableJoystick>();
            component.LookJoystick = GameObject.Find("Canvas/Joystick_Look").GetComponent<VariableJoystick>();
            component.FireJoystick = GameObject.Find("Canvas/Joystick_Fire").GetComponent<VariableJoystick>();
            component.MoveJoystick.OnEndMove += () =>
            {
                StopMove(game, component);
            };
            component.FireJoystick.OnMove += () =>
            {
                StartFire(game, component);
            };
            component.FireJoystick.OnEndMove += () =>
            {
                StopFire(game, component);
            };
        }

        public void Init(TrueGame game, PlayerInputComponent component)
        {
        }

        public static void StopMove(TrueGame game, PlayerInputComponent component)
        {
            component.PlayerInputs.Add(new PlayerInput()
            {
                PlayerId = game.MyActor.Id,
                InputType = PlayerInputType.StopMove
            });
        }

        public static void CheckLook(TrueGame game, PlayerInputComponent component)
        {
            var lookJoystick = component.LookJoystick;
            var direction = Vector3.forward * lookJoystick.Vertical + Vector3.right * lookJoystick.Horizontal;
            if (direction.sqrMagnitude > 0.1f)
            {
                component.LookVector = direction;
            }
            else
            {
                if (component.LookVector != Vector3.zero)
                {
                    component.LookVector = Vector3.zero;
                }
            }
        }

        public static void CheckMove(TrueGame game, PlayerInputComponent component)
        {
            var moveJoystick = component.MoveJoystick;
            var direction = Vector3.forward * moveJoystick.Vertical + Vector3.right * moveJoystick.Horizontal;
            if (direction.sqrMagnitude > 0.1f)
            {
                component.MoveVector = direction;
            }
            else
            {
                direction = Vector3.forward * Input.GetAxis("Vertical") + Vector3.right * Input.GetAxis("Horizontal");
                if (direction.sqrMagnitude > 0.1f)
                {
                    component.MoveVector = direction;
                }
                else
                {
                    if (component.MoveVector != Vector3.zero)
                    {
                        component.MoveVector = Vector3.zero;
                        StopMove(game, component);
                    }
                }
            }
        }

        public static void StartFire(TrueGame game, PlayerInputComponent component)
        {
            if (component.FireState == false)
            {
                component.FireState = true;
            }
        }

        public static void CheckFire(TrueGame game, PlayerInputComponent component)
        {
            if (component.FireState == false)
            {
                return;
            }

            var fireJoystick = component.FireJoystick;
            var fireSpeed = game.MyActor.GetComponent<FireComponent>().FireSpeed;
            var interval = FP.FromRaw(1) / FP.FromRaw(fireSpeed);
            var nowTime = FP.FromFloat(Time.realtimeSinceStartup);
            if (nowTime > component.NextFireTime)
            {
                //ConsoleLog.Debug($"CheckFire {nowTime.AsFloat()} {component.NextFireTime.AsFloat()}");
                component.NextFireTime = nowTime + interval;

                var direction = Vector3.forward * fireJoystick.Vertical + Vector3.right * fireJoystick.Horizontal;
                component.FireVector = direction;
                var input = new PlayerInput()
                {
                    PlayerId = game.MyActor.Id,
                    InputType = PlayerInputType.Fire,
                    InputVector = component.FireVector.ToTSVector(),
                };
                TrueGameExecuteSystem.AddPlayerInput(game, input);
                
                //EventSystem.Run(EventType.InputEvent, Tuple.Create("Fire"));
            }
        }

        public static void StopFire(TrueGame game, PlayerInputComponent component)
        {
            if (component.FireState == true)
            {
                component.FireState = false;
            }

            //component.PlayerInputs.Add(new PlayerInput()
            //{
            //    PlayerId = game.MyActor.Id,
            //    InputType = PlayerInputType.StopFire
            //});
        }

        public static void Update(TrueGame game, PlayerInputComponent component)
        {
            CheckMove(game, component);
            CheckLook(game, component);
            CheckFire(game, component);
        }
    } 
}
