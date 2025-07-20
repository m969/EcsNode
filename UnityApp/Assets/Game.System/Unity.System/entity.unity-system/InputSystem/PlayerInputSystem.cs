using ECS;
using FairyGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using TrueSync;
using UnityEngine;

namespace ECSGame
{
    public class PlayerInputSystem : AEntitySystem<PlayerInput>,
        IInit<PlayerInput>,
        IUpdate<PlayerInput>
    {
        public void Init(PlayerInput component)
        {
            component.MoveJoystick = GameObject.Find("Canvas/Joystick_Move").GetComponent<VariableJoystick>();
            component.LookJoystick = GameObject.Find("Canvas/Joystick_Look").GetComponent<VariableJoystick>();
            component.FireJoystick = GameObject.Find("Canvas/Joystick_Fire").GetComponent<VariableJoystick>();
            component.MoveJoystick.OnEndMove += () =>
            {
                StopMove(component);
            };
            component.FireJoystick.OnMove += () =>
            {
                StartFire(component);
            };
            component.FireJoystick.OnEndMove += () =>
            {
                StopFire(component);
            };

            component.Game.OnFrameUpdate += FrameUpdate;
        }

        public void Update(PlayerInput component)
        {
            CheckMove(component);
            CheckLook(component);
            CheckFire(component);

            if (Input.GetKeyUp(KeyCode.Space))
            {
                foreach (var item in component.Game.Id2Children.Values)
                {
                    if (item != component.PlayerActor)
                    {
                        var transComp = item.GetComponent<TransformComponent>();
                        //ConsoleLog.Debug($"{transComp.Position} {transComp.ForecastPosition}");
                    }
                }
            }
        }

        public static PlayerInput Create(ushort nodeIndex, Assembly systemAssembly)
        {
            var playerInput = EcsNodeSystem.Create<PlayerInput>(nodeIndex, systemAssembly);
            return playerInput;
        }

        public static void StopMove(PlayerInput component)
        {
            component.PlayerInputs.Add(new InputData()
            {
                PlayerId = component.PlayerActor.Id,
                InputType = InputType.StopMove
            });
        }

        public static void CheckLook(PlayerInput component)
        {
            var lookJoystick = component.LookJoystick;
            var direction = Vector3.forward * lookJoystick.Vertical + Vector3.right * lookJoystick.Horizontal;
            if (direction.sqrMagnitude > 0.01f)
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

        public static void CheckMove(PlayerInput component)
        {
            var moveJoystick = component.MoveJoystick;
            var direction = Vector3.forward * moveJoystick.Vertical + Vector3.right * moveJoystick.Horizontal;
            if (direction.sqrMagnitude > 0.01f)
            {
                component.MoveVector = direction;
            }
            else
            {
                direction = Vector3.forward * Input.GetAxis("Vertical") + Vector3.right * Input.GetAxis("Horizontal");
                if (direction.sqrMagnitude > 0.01f)
                {
                    component.MoveVector = direction;
                }
                else
                {
                    if (component.MoveVector != Vector3.zero)
                    {
                        component.MoveVector = Vector3.zero;
                        StopMove(component);
                    }
                }
            }
        }

        public static void StartFire(PlayerInput component)
        {
            if (component.FireState == false)
            {
                component.FireState = true;
            }
        }

        public static void CheckFire(PlayerInput component)
        {
            if (component.FireState == false)
            {
                return;
            }

            var fireJoystick = component.FireJoystick;
            var fireSpeed = component.PlayerActor.GetComponent<FireComponent>().FireSpeed;
            var interval = FP.FromRaw(1) / FP.FromRaw(fireSpeed);
            var nowTime = FP.FromFloat(Time.realtimeSinceStartup);
            if (nowTime > component.NextFireTime)
            {
                component.NextFireTime = nowTime + interval;
                var direction = Vector3.forward * fireJoystick.Vertical + Vector3.right * fireJoystick.Horizontal;
                EventSystem.RunAsync(new InputExecuteEvent(), component, InputType.Fire, direction).Coroutine();
            }
        }

        public static void StopFire(PlayerInput component)
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

        public static void FrameUpdate(TrueGame game, long determineFrame)
        {
            var myActor = game.MyActor;
            var newInputFrame = determineFrame + TrueGame.ForecastFrame;
            var inputComp = EcsDomain.PlayerInput;

            if (inputComp.LookVector != Vector3.zero)
            {
                var input = new InputData()
                {
                    Frame = newInputFrame,
                    PlayerId = inputComp.PlayerActor.Id,
                    InputType = InputType.Look,
                    InputVector = inputComp.LookVector.ToTSVector(),
                };
                inputComp.PlayerInputs.Add(input);
            }

            if (inputComp.MoveVector != Vector3.zero)
            {
                var input = new InputData()
                {
                    Frame = newInputFrame,
                    PlayerId = inputComp.PlayerActor.Id,
                    InputType = InputType.Move,
                    InputVector = inputComp.MoveVector.ToTSVector(),
                };
                inputComp.PlayerInputs.Add(input);
            }

            {// 本地模拟服务端权威帧返回，改成真服务端流程这里需要注释掉
                var framePlayComp = inputComp.PlayerActor.GetComponent<FramePlayComponent>();
                if (framePlayComp.AdvanceFrameInputs.TryGetValue(determineFrame, out var playerInputs))
                {
                    if (framePlayComp.DetermineFrameInputs.ContainsKey(determineFrame) == false)
                    {
                        framePlayComp.DetermineFrameInputs.Add(determineFrame, new List<InputData>(playerInputs));
                    }
                }
            }

            foreach (var item in inputComp.PlayerInputs)
            {
                //ConsoleLog.Debug($"ProcessLocalPlayerInput {newInputFrame}");
                ActorAdvancePlaySystem.AddLocalPlayerInput(inputComp.PlayerActor, item, newInputFrame);
            }

            inputComp.PlayerInputs.Clear();
        }
    }
}
