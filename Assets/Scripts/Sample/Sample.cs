using ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour
{
    public EcsNode EcsNode { get; set; }
    //private EcsUpdateDriveSystem EcsUpdateDriveSystem { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        EcsNode = new EcsNode();

        EcsNode.RegisterDrive<IAwake>();
        EcsNode.RegisterDrive<IInit>();
        EcsNode.RegisterDrive<IUpdate>();

        //EcsNode.RegisterEcsDriveSystem<EcsAwakeDriveSystem>();
        //EcsNode.RegisterEcsDriveSystem<EcsStartDriveSystem>();
        //EcsNode.RegisterEcsDriveSystem<EcsUpdateDriveSystem>();
        //EcsUpdateDriveSystem = EcsNode.GetEcsDriveSystem<EcsUpdateDriveSystem>();

        //EcsNode.RegisterEntitySystem<ActorSystem>();
        //EcsNode.RegisterEntitySystem<ActorStartSystem>();
        //EcsNode.RegisterEntitySystem<ActorUpdateSystem>();
        EcsNode.RegisterEntitySystem<MoveSystem>();
        //EcsNode.RegisterEntitySystem<HealthSystem>();

        var actor = EcsNode.AddChild<Actor>(beforeAwake: x => x.ActorType = 1);
        actor.AddComponent<MoveComponent>();
        actor.AddComponent<HealthComponent>();
        actor.Init();

        //var actorSystem = EcsNode.GetSystem<ActorSystem>();
        //actor.Initialize();
        //actorSystem.Test1(actor);
    }

    // Update is called once per frame
    void Update()
    {
        //EcsUpdateDriveSystem.Handle(EcsNode);
        EcsNode.DriveUpdate();
    }
}
