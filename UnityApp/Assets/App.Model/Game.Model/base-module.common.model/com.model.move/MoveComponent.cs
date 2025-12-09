using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    public class MoveComponent : EcsComponent
    {
        public int Speed { get; set; }
        public int StopSpeed { get; set; }
        public int LeftStopStep { get; set; }
        public int ForecastLeftStopStep { get; set; }
        public TSVector TrueDirection { get; set; }
        public TSVector ForecastTrueDirection { get; set; }
        public bool Moving { get; set; }
        public TSVector Destination { get; set; }
    }
}
