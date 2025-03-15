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
        }

        public void Init(TrueGame game, PlayerInputComponent component)
        {
        }

        public static void StopMove(TrueGame game, PlayerInputComponent component)
        {
            component.PlayerInputs.Add(new PlayerInput()
            {
                PlayerId = game.MyActor.Id,
                InputType = PlayerInputType.Stop
            });
        }

        public static void Update(TrueGame game, PlayerInputComponent component)
        {
            var moveJoystick = component.MoveJoystick;
            var lookJoystick = component.LookJoystick;
            var fireJoystick = component.FireJoystick;

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

            direction = Vector3.forward * moveJoystick.Vertical + Vector3.right * moveJoystick.Horizontal;
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

            //if (Input.GetKeyUp(KeyCode.W)
            //    || Input.GetKeyUp(KeyCode.A)
            //    || Input.GetKeyUp(KeyCode.S)
            //    || Input.GetKeyUp(KeyCode.D)
            //    )
            //{
            //    StopMove(game, component);
            //}
        }
    } 
}
