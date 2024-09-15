using ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour
{
    public EcsNode EcsNode { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        EcsNode = new EcsNode();

        EcsNode.RegisterDrive<IAwake>();
        EcsNode.RegisterDrive<IInit>();
        EcsNode.RegisterDrive<IUpdate>();

        EcsNode.RegisterSystem<EntitySystem>();
        EcsNode.RegisterSystem<MoveSystem>();

        var actor = EcsNode.AddChild<Actor>(beforeAwake: x => x.Type = 1);
        actor.AddComponent<MoveComponent>();
        actor.AddComponent<HealthComponent>();
        actor.Init();

        actor.MoveToAsync(Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        EcsNode.DriveUpdate();
    }
}
