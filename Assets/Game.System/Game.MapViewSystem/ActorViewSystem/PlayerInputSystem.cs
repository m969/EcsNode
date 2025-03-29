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
                InputType = InputType.StopMove
            });
        }

        public static void CheckLook(TrueGame game, PlayerInputComponent component)
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

        public static void CheckMove(TrueGame game, PlayerInputComponent component)
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
                component.NextFireTime = nowTime + interval;
                var direction = Vector3.forward * fireJoystick.Vertical + Vector3.right * fireJoystick.Horizontal;
                EventSystem.Run(InputEvent.NewEvent(), game, InputType.Fire, direction).Coroutine();
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

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                var transComp = game.MyActor.GetComponent<TransformComponent>();
                ConsoleLog.Debug($"{transComp.Position} {transComp.ForecastPosition}");
            }
        }

        public static void FrameUpdate(TrueGame game, PlayerInputComponent inputComp, long determineFrame, long advanceFrame)
        {
            if (inputComp.LookVector != Vector3.zero)
            {
                var input = new PlayerInput()
                {
                    Frame = advanceFrame,
                    PlayerId = game.MyActor.Id,
                    InputType = InputType.Look,
                    InputVector = inputComp.LookVector.ToTSVector(),
                };
                inputComp.PlayerInputs.Add(input);
            }

            if (inputComp.MoveVector != Vector3.zero)
            {
                var input = new PlayerInput()
                {
                    Frame = advanceFrame,
                    PlayerId = game.MyActor.Id,
                    InputType = InputType.Move,
                    InputVector = inputComp.MoveVector.ToTSVector(),
                };
                inputComp.PlayerInputs.Add(input);
            }

            //if (inputComp.FireState)
            //{
            //    var input = new PlayerInput()
            //    {
            //        PlayerId = game.MyActor.Id,
            //        InputType = PlayerInputType.Fire,
            //        InputVector = inputComp.FireVector.ToTSVector(),
            //    };
            //    TrueGameExecuteSystem.AddPlayerInput(game, input);
            //}

            var framePlayComp = game.MyActor.GetComponent<FramePlayComponent>();
            //ConsoleLog.Debug($"PlayerInputSystem {framePlayComp.AdvanceFrameInputs.Count} {determineFrame} {advanceFrame}");
            if (framePlayComp.AdvanceFrameInputs.TryGetValue(determineFrame, out var playerInputs))
            {
                if (framePlayComp.DetermineFrameInputs.ContainsKey(determineFrame) == false)
                {
                    framePlayComp.DetermineFrameInputs.Add(determineFrame, new List<PlayerInput>(playerInputs));
                }
                //framePlayComp.DetermineFrameInputs[determineFrame].Add(input);
                //foreach (var input in playerInputs)
                //{
                //    ActorPlaySystem.ProcessNetworkPlayerInput(game.MyActor, input, determineFrame);
                //}
            }
            foreach (var item in inputComp.PlayerInputs)
            {
                ActorPlaySystem.ProcessLocalPlayerInput(game.MyActor, item, advanceFrame);
                //TrueGameExecuteSystem.AddPlayerInput(game, item);
            }

            inputComp.PlayerInputs.Clear();
        }
    }
}
