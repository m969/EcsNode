using ECS;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

namespace ECSGame
{
    /// <summary>
    /// 表示实体在 TrueSync 世界中的空间变换状态。
    /// </summary>
    public class TransformComponent : EcsComponent
    {
        /// <summary>
        /// 当前逻辑帧的世界坐标。
        /// </summary>
        public TSVector Position
        {
            get;
            set;
        }

        /// <summary>
        /// 基于旋转计算得到的前向单位向量。
        /// </summary>
        public TSVector Forward
        {
            get => this.Rotation * TSVector.forward;
            set => this.Rotation = TSQuaternion.LookRotation(value, TSVector.up);
        }

        /// <summary>
        /// 实体面向方向的四元数表示。
        /// </summary>
        public TSQuaternion Rotation
        {
            get;
            set;
        }

        /// <summary>
        /// 用于预测插值的下一帧世界坐标。
        /// </summary>
        public TSVector ForecastPosition
        {
            get;
            set;
        }
    }
}
