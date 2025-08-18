# （网格系统）模块程序设计文档 - version：1.0.4
## 1. 程序功能概述
网格系统模块用于实现游戏中的格子化建造玩法，负责网格区域的初始化、格子状态管理，并提供格子坐标与世界坐标的转换接口。

## 2. 数据结构设计
### 配置接口设计
/// <summary>
/// 网格区域配置接口，唯一标识Id，辅助名称Key，包含网格尺寸、格子大小等参数。
/// </summary>
public interface IGridPlaneConfig
{
    /// <summary>网格区域配置唯一标识</summary>
    int Id { get; }
    /// <summary>网格区域配置名称</summary>
    string Key { get; }
    /// <summary>网格区域宽度（格子数量）</summary>
    int Width { get; }
    /// <summary>网格区域高度（格子数量）</summary>
    int Height { get; }
    /// <summary>单个格子的大小（世界单位）</summary>
    float CellSize { get; }
}

### 实体设计
/// <summary>
/// 网格区域实体，继承自EcsEntity，包含网格配置、尺寸、位置等属性。
/// </summary>
public class GridPlane : EcsEntity
{
    /// <summary>网格配置ID</summary>
    public int ConfigId { get; set; }
    /// <summary>网格宽度</summary>
    public int Width { get; set; }
    /// <summary>网格高度</summary>
    public int Height { get; set; }
    /// <summary>格子大小</summary>
    public float CellSize { get; set; }
    /// <summary>网格区域世界坐标位置</summary>
    public (float x, float y) Position { get; set; }
}

/// <summary>
/// 单个格子实体，继承自EcsEntity，包含格子坐标、状态、占用者Id等属性。
/// </summary>
public class GridCell : EcsEntity
{
    /// <summary>所属网格区域ID</summary>
    public long GridPlaneId { get; set; }
    /// <summary>格子X坐标（网格坐标系）</summary>
    public int X { get; set; }
    /// <summary>格子Y坐标（网格坐标系）</summary>
    public int Y { get; set; }
    /// <summary>格子状态（空置/占用）</summary>
    public GridCellState State { get; set; }
    /// <summary>占用者实体ID</summary>
    public long OccupiedById { get; set; }
}

/// <summary>
/// 格子状态枚举
/// </summary>
public enum GridCellState
{
    /// <summary>空置</summary>
    Empty = 0,
    /// <summary>占用</summary>
    Occupied = 1
}

### 组件设计
/// <summary>
/// 网格区域管理组件，继承自EcsComponent，管理网格区域实体字典。挂载在World实体上。
/// </summary>
public class GridPlaneListComponent : EcsComponent
{
    /// <summary>网格区域实体字典，key为网格区域ID</summary>
    public Dictionary<long, GridPlane> GridPlanes { get; set; }
}

/// <summary>
/// 格子管理组件，继承自EcsComponent，管理格子实体字典。
/// </summary>
public class GridCellListComponent : EcsComponent
{
    /// <summary>格子实体字典，key为格子坐标元组</summary>
    public Dictionary<(int x, int y), GridCell> Cells { get; set; }
}

/// <summary>
/// 坐标转换组件，继承自EcsComponent，提供格子坐标与世界坐标的转换功能。
/// </summary>
public class GridConverterComponent : EcsComponent
{
    /// <summary>关联的网格区域ID</summary>
    public long GridPlaneId { get; set; }
    /// <summary>网格区域世界坐标位置</summary>
    public (float x, float y) Position { get; set; }
    /// <summary>格子大小</summary>
    public float CellSize { get; set; }
}

## 3. 系统业务设计
### 实体系统设计
public class GridPlaneSystem : AEntitySystem<GridPlane>, IAwake<GridPlane>, IInit<GridPlane>, IAfterInit<GridPlane>, IEnable<GridPlane>, IDisable<GridPlane>, IUpdate<GridPlane>, IDestroy<GridPlane>
{
    /// <summary>创建并初始化网格区域实体</summary>
    /// <param name="parent">父实体</param>
    /// <param name="config">网格区域配置接口</param>
    /// <param name="position">网格区域位置</param>
    public static GridPlane CreateGridPlane(EcsEntity parent, IGridPlaneConfig config, (float x, float y) position)
    {
        // 方法实现略
        throw new NotImplementedException();
    }
    // 生命周期接口方法略
}

public class GridCellSystem : AEntitySystem<GridCell>, IAwake<GridCell>, IInit<GridCell>, IAfterInit<GridCell>, IEnable<GridCell>, IDisable<GridCell>, IUpdate<GridCell>, IDestroy<GridCell>
{
    /// <summary>创建格子实体</summary>
    /// <param name="parent">父实体（网格区域）</param>
    /// <param name="x">格子X坐标</param>
    /// <param name="y">格子Y坐标</param>
    public static GridCell CreateGridCell(GridPlane parent, int x, int y)
    {
        // 方法实现略
        throw new NotImplementedException();
    }
    
    /// <summary>更新格子状态</summary>
    /// <param name="entity">格子实体</param>
    /// <param name="state">新状态</param>
    /// <param name="occupierId">占用者ID</param>
    public static void UpdateCellState(GridCell entity, GridCellState state, long occupierId = 0)
    {
        // 方法实现略
        throw new NotImplementedException();
    }
    // 生命周期接口方法略
}

### 组件系统设计
public class GridPlaneListSystem : AComponentSystem<EcsEntity, GridPlaneListComponent>, 
    IAwake<EcsEntity, GridPlaneListComponent>, 
    IInit<EcsEntity, GridPlaneListComponent>, 
    IAfterInit<EcsEntity, GridPlaneListComponent>, 
    IEnable<EcsEntity, GridPlaneListComponent>, 
    IDisable<EcsEntity, GridPlaneListComponent>, 
    IDestroy<EcsEntity, GridPlaneListComponent>
{
    /// <summary>添加网格区域（在实体上登记一个GridPlane）</summary>
    /// <param name="entity">EcsEntity 实体</param>
    /// <param name="gridPlane">要添加的网格区域实体</param>
    public static void AddGridPlane(EcsEntity entity, GridPlane gridPlane)
    {
        // 方法实现略
        throw new NotImplementedException();
    }
    
    /// <summary>移除网格区域（从实体移除指定 Id 的 GridPlane）</summary>
    /// <param name="entity">EcsEntity 实体</param>
    /// <param name="gridPlaneId">网格区域实体 Id</param>
    public static void RemoveGridPlane(EcsEntity entity, long gridPlaneId)
    {
        // 方法实现略
        throw new NotImplementedException();
    }
    
    /// <summary>根据 Id 获取网格区域</summary>
    /// <param name="entity">EcsEntity 实体</param>
    /// <param name="id">网格区域实体 Id</param>
    public static GridPlane GetGridPlane(EcsEntity entity, long id)
    {
        // 方法实现略
        throw new NotImplementedException();
    }
}

public class GridCellListSystem : AComponentSystem<GridPlane, GridCellListComponent>, IAwake<GridPlane, GridCellListComponent>, IInit<GridPlane, GridCellListComponent>, IAfterInit<GridPlane, GridCellListComponent>, IEnable<GridPlane, GridCellListComponent>, IDisable<GridPlane, GridCellListComponent>, IDestroy<GridPlane, GridCellListComponent>
{
    /// <summary>获取指定坐标的格子</summary>
    public static GridCell GetCell(GridPlane entity, int x, int y)
    {
        // 方法实现略
        throw new NotImplementedException();
    }
    
    /// <summary>获取所有空置格子</summary>
    public static IEnumerable<GridCell> GetEmptyCells(GridPlane entity)
    {
        // 方法实现略
        throw new NotImplementedException();
    }
}

public class GridConverterSystem : AComponentSystem<GridPlane, GridConverterComponent>, IAwake<GridPlane, GridConverterComponent>, IInit<GridPlane, GridConverterComponent>, IAfterInit<GridPlane, GridConverterComponent>, IEnable<GridPlane, GridConverterComponent>, IDisable<GridPlane, GridConverterComponent>, IDestroy<GridPlane, GridConverterComponent>
{
    /// <summary>网格坐标转世界坐标</summary>
    public static (float x, float y) GridToWorld(GridPlane entity, int gridX, int gridY)
    {
        // 方法实现略
        throw new NotImplementedException();
    }
    
    /// <summary>世界坐标转网格坐标</summary>
    public static (int x, int y) WorldToGrid(GridPlane entity, float worldX, float worldY)
    {
        // 方法实现略
        throw new NotImplementedException();
    }
    
    /// <summary>检查坐标是否在网格区域内</summary>
    public static bool IsInBounds(GridPlane entity, int x, int y)
    {
        // 方法实现略
        throw new NotImplementedException();
    }
}

### 系统事件接口设计
/// <summary>
/// 网格状态变更事件接口
/// </summary>
public interface IOnGridCellStateChanged : IDispatch
{
    void OnGridCellStateChanged(GridCell entity, int x, int y, GridCellState newState);
}

/// <summary>
/// 网格区域变更事件接口
/// </summary>
public interface IOnGridPlaneChanged : IDispatch
{
    void OnGridPlaneChanged(GridPlane entity);
}
