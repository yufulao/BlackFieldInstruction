using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridObject:MonoBehaviour
{
    public GridObjInfo gridObjInfo;

    public virtual void OnInit()
    {
        
    }

    public virtual void OnReset()
    {
        gameObject.transform.position = GridManager.Instance.GetWorldPositionByPoint(gridObjInfo.originalPoint.x,gridObjInfo.originalPoint.y);
        gridObjInfo.currentPoint=gridObjInfo.originalPoint;
    }

}