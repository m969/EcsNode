using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

public class TrueTransformComponent : EcsComponent
{
    public TSVector Position
    {
        get;
        set;
    }

    public TSVector Forward
    {
        get => this.Rotation * TSVector.forward;
        set => this.Rotation = TSQuaternion.LookRotation(value, TSVector.up);
    }

    public TSQuaternion Rotation
    {
        get;
        set;
    }
}
