using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineData
{
    private readonly int id;
    private Vector2 pointA;
    private Vector2 pointB;
    
}

public class GraphRender : MonoBehaviour
{
#region Fields
    private Vector2 Anchor;
    private Canvas targetCanvas;

    private List<LineData> lineDataList;

    public void SetAnchor(Vector2 newAnchor)
    {
        Anchor = newAnchor;
        RefreshGraph();
    }

    public void SetTargetCanvas(Canvas canvas)
    {
        targetCanvas = canvas;
    }

#endregion

#region Init

    private void Start()
    {
        
    }

    private void Awake()
    {
        if (lineDataList == null) lineDataList = new List<LineData>();
    }

#endregion
#region public methods
    public void RefreshGraph()
    {

    }

#endregion


#region private methods

    private void DrawLine(Vector2 pointA, Vector2 pointB, float lineThickness, Color color)
    {
        if (targetCanvas == null) return;
        if (Anchor == null) return;
        int number = lineDataList.Count;

        GameObject line = new GameObject("Line" + number, typeof(Image));
        line.transform.SetParent(targetCanvas.transform);
        
        Image lineImg = line.GetComponent<Image>();
        lineImg.color = color;

        Vector2 direction = (pointB - pointA).normalized;
        float distance = Vector2.Distance(pointA, pointB);

        RectTransform lineRectTransform = line.GetComponent<RectTransform>();
        lineRectTransform.anchorMin = Vector2.zero;
        lineRectTransform.anchorMax = Vector2.zero;
        lineRectTransform.anchoredPosition = pointA + direction * distance * 0.5f;
        lineRectTransform.localEulerAngles = new Vector3(0,0, Mathf.Atan2(direction.y,direction.x));
        lineRectTransform.sizeDelta = new Vector2(distance, lineThickness);
    }






#endregion

    
}
