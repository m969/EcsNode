using ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IOnGridCellClick : IDispatch
{
    void OnGridCellClick(EcsEntity entity);
}

public interface IOnGridCellEnter : IDispatch
{
    void OnGridCellEnter(EcsEntity entity);
}

public interface IOnGridCellExit : IDispatch
{
    void OnGridCellExit(EcsEntity entity);
}

/// <summary>
/// 该组件用于控制网格单元格的行为
/// 监听用户鼠标移入事件、移出事件、点击事件并响应
/// </summary>
public class GridCellControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public EcsEntity GridCellEntity { get; set; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GridCellEntity.Dispatch<IOnGridCellClick>(system => system.OnGridCellClick(GridCellEntity));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GridCellEntity.Dispatch<IOnGridCellEnter>(system => system.OnGridCellEnter(GridCellEntity));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GridCellEntity.Dispatch<IOnGridCellExit>(system => system.OnGridCellExit(GridCellEntity));
    }
}
