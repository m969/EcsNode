using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerInputSystem : AEcsComponentSystem<TrueGame, PlayerInputComponent>,
    IAwake<TrueGame, PlayerInputComponent>,
    IInit<TrueGame, PlayerInputComponent>
{
    public void Awake(TrueGame game, PlayerInputComponent component)
    {
    }

    public void Init(TrueGame game, PlayerInputComponent component)
    {
    }
     
    public static void Update(TrueGame game, PlayerInputComponent component)
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            component.PlayerInputs.Add(new PlayerInput()
            {
                PlayerId = 0,
                Command = "Move"
            });
        }
    }
}
