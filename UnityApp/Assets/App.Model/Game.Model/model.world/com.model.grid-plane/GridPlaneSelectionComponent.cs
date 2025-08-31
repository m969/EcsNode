using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ECSGame
{
    /// <summary>
    /// 网格选择组件
    /// </summary>
    public class GridPlaneSelectionComponent : EcsComponent
    {
        public long SelectCellId = 0;
    }
}