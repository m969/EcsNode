using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using TrueSync;
using UnityEngine;
using ECSGame;

namespace ECSUnity
{
    [SystemPlatformFilter((int)RuntimePlatform.Android)]
    [SystemPlatformFilter((int)RuntimePlatform.IPhonePlayer)]
    [SystemPlatformFilter((int)RuntimePlatform.WindowsEditor)]
    [SystemPlatformFilter((int)RuntimePlatform.OSXEditor)]
    [SystemGameFilter((int)GameType.TrueGameDemo)]
    [SystemEcsFilter(EcsType.PlayerInput)]
    public class TrueGameInputSystem : AComponentSystem<PlayerInput, TrueGameInputComponent>,
        IAwake<PlayerInput, TrueGameInputComponent>,
        IInit<PlayerInput, TrueGameInputComponent>
    {
        public void Awake(PlayerInput playerInput, TrueGameInputComponent component)
        {
            component.PlayerActor = UnityStatic.MyActor;
            component.TrueWorld = EcsDomain.TrueWorld;
        }

        public void Init(PlayerInput playerInput, TrueGameInputComponent component)
        {
            component.MoveJoystick = GameObject.Find("Canvas/Joystick_Move").GetComponent<VariableJoystick>();
            component.LookJoystick = GameObject.Find("Canvas/Joystick_Look").GetComponent<VariableJoystick>();
            component.FireJoystick = GameObject.Find("Canvas/Joystick_Fire").GetComponent<VariableJoystick>();
            component.MoveJoystick.OnEndMove += () =>
            {
                StopMove(playerInput);
            };
            component.FireJoystick.OnMove += () =>
            {
                StartFire(playerInput);
            };
            component.FireJoystick.OnEndMove += () =>
            {
                StopFire(playerInput);
            };
        }

        public static void Update(PlayerInput playerInput)
        {
            if (playerInput.TryGetComponent<TrueGameInputComponent>(out var component) == false)
            {
                return;
            }

            CheckMove(playerInput);
            CheckLook(playerInput);
            CheckFire(playerInput);

            if (Input.GetKeyUp(KeyCode.Space))
            {
                foreach (var item in component.TrueWorld.Id2Children.Values)
                {
                    if (item != component.PlayerActor)
                    {
                        var transComp = item.GetComponent<TransformComponent>();
                        //ConsoleLog.Debug($"{transComp.Position} {transComp.ForecastPosition}");
                    }
                }
            }
        }

        public static void StopMove(PlayerInput playerInput)
        {
            if (playerInput.TryGetComponent<TrueGameInputComponent>(out var component) == false)
            {
                return;
            }
            component.InputDatas.Add(new InputData()
            {
                PlayerId = component.PlayerActor.Id,
                InputType = InputType.StopMove
            });
        }

        public static void CheckLook(PlayerInput playerInput)
        {
            if (playerInput.TryGetComponent<TrueGameInputComponent>(out var component) == false)
            {
                return;
            }
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

        public static void CheckMove(PlayerInput playerInput)
        {
            if (playerInput.TryGetComponent<TrueGameInputComponent>(out var component) == false)
            {
                return;
            }
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
                        StopMove(playerInput);
                    }
                }
            }
        }

        public static void StartFire(PlayerInput playerInput)
        {
            if (playerInput.TryGetComponent<TrueGameInputComponent>(out var component) == false)
            {
                return;
            }
            if (component.FireState == false)
            {
                component.FireState = true;
            }
        }

        public static void CheckFire(PlayerInput playerInput)
        {
            if (playerInput.TryGetComponent<TrueGameInputComponent>(out var component) == false)
            {
                return;
            }
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
                DomainSystem.PublishAsync(playerInput, new InputEvent()
                {
                    InputType = InputType.Fire,
                    Direction = direction
                }).Coroutine();
            }
        }

        public static void StopFire(PlayerInput playerInput)
        {
            if (playerInput.TryGetComponent<TrueGameInputComponent>(out var component) == false)
            {
                return;
            }
            if (component.FireState == true)
            {
                component.FireState = false;
            }
        }

        public static void OnFrameUpdate(TrueWorld trueWorld, long determineFrame)
        {
            var newInputFrame = determineFrame + TrueWorld.ForecastFrame;
            var playerInput = EcsDomain.PlayerInput;
            if (playerInput == null || playerInput.TryGetComponent<TrueGameInputComponent>(out var component) == false)
            {
                return;
            }

            if (component.LookVector != Vector3.zero)
            {
                var input = new InputData()
                {
                    Frame = newInputFrame,
                    PlayerId = component.PlayerActor.Id,
                    InputType = InputType.Look,
                    InputVector = component.LookVector.ToTSVector(),
                };
                component.InputDatas.Add(input);
            }

            if (component.MoveVector != Vector3.zero)
            {
                var input = new InputData()
                {
                    Frame = newInputFrame,
                    PlayerId = component.PlayerActor.Id,
                    InputType = InputType.Move,
                    InputVector = component.MoveVector.ToTSVector(),
                };
                component.InputDatas.Add(input);
            }

            {// 本地模拟服务端权威帧返回，改成真服务端流程这里需要注释掉
                var framePlayComp = component.PlayerActor.GetComponent<FramePlayComponent>();
                if (framePlayComp.AdvanceFrameInputs.TryGetValue(determineFrame, out var playerInputs))
                {
                    if (framePlayComp.DetermineFrameInputs.ContainsKey(determineFrame) == false)
                    {
                        framePlayComp.DetermineFrameInputs.Add(determineFrame, new List<InputData>(playerInputs));
                    }
                }
            }

            foreach (var item in component.InputDatas)
            {
                //ConsoleLog.Debug($"ProcessLocalPlayerInput {newInputFrame}");
                ActorAdvancePlaySystem.AddLocalPlayerInput(component.PlayerActor, item, newInputFrame);
            }

            component.InputDatas.Clear();
        }
    }
}
