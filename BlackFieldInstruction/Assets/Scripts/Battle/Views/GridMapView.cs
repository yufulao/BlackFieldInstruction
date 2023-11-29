using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMapView : MonoBehaviour
{
    private TextMesh[,] _debugTextArray;

    /// <summary>
    /// 创建并显示辅助线
    /// </summary>
    public void ShowDebug(int width,int height,float cellSize)
    {
        GridManager gridManager = GridManager.Instance;
        _debugTextArray = new TextMesh[width, height];
        GameObject textContainer = new GameObject("TestMeshObjContainer");
        Vector3 drawLineOffect = new Vector3(cellSize, 0, cellSize) * 0.5f;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 cellWorldPosition = gridManager.GetWorldPositionByPoint(x, z);
                _debugTextArray[x, z] = Utils.CreateWorldText((x+","+z),textContainer.transform, cellWorldPosition,new Vector3(0.1f,0.1f,0.1f),30, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center);
                Debug.DrawLine(cellWorldPosition-drawLineOffect, gridManager.GetWorldPositionByPoint(x, z + 1)-drawLineOffect, Color.white, 100f);
                Debug.DrawLine(cellWorldPosition-drawLineOffect, gridManager.GetWorldPositionByPoint(x + 1, z)-drawLineOffect, Color.white, 100f);
            }
        }
        Vector3 gridEndWorldPosition = gridManager.GetWorldPositionByPoint(width, height);
        Debug.DrawLine(gridManager.GetWorldPositionByPoint(0, height)-drawLineOffect, gridEndWorldPosition-drawLineOffect, Color.white, 100f);
        Debug.DrawLine(gridManager.GetWorldPositionByPoint(width, 0)-drawLineOffect, gridEndWorldPosition-drawLineOffect, Color.white, 100f);
    }
}
