using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
public class LineData
{
    private readonly int id;
    private Vector2 pointA;
    private Vector2 pointB;
    
    public LineData(int id, Vector2 pointA, Vector2 pointB)
    {
        this.id = id;
        this.pointA = pointA;
        this.pointB = pointB;
    }
}

public class GraphRender : MonoBehaviour
{
#region Fields
    private Vector3 anchor;
    private Vector2 size;

    // private Canvas targetCanvas;

    private List<LineData> lineDataList;

    public void SetAnchor(Vector2 newAnchor)
    {
        anchor = newAnchor;
    }

    public void SetSize(Vector2 newSize)
    {
        size = newSize;
    }

    // public void SetTargetCanvas(Canvas canvas)
    // {
    //     targetCanvas = canvas;
    // }

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

    /// <summary>
    /// Convert curve points to graph coordinates with specified size using length of points as x range
    /// </summary>
    /// <param name="curve"></param>
    /// <param name="points"></param>
    public void ConvertCurveToSpecifiedPointsLength(AnimationCurve curve, Vector2[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            float x = i / (float)(points.Length - 1); // Scale x to range [0, 1]
            float y = curve.Evaluate(x);
            points[i] = new Vector2(x, y);
        }
    }
    public void ConvertCurvesToSpecifiedPointsLength(List<AnimationCurve> curves, Vector2[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            float x = i / (float)(points.Length - 1); // Scale x to range [0, 1]
            float y = 0;
            foreach (var curve in curves)
            {
                y += curve.Evaluate(x);
            }
            points[i] = new Vector2(x, y);
        }
    }
    public void LerpVector2List(Vector2[] currentPoints, Vector2[] targetPoints, float speed = 10f)
    {
        if (currentPoints.Length != targetPoints.Length)
        {
            Debug.LogError("Current points and target points must have the same length");
            return;
        }

        for (int i = 0; i < currentPoints.Length; i++)
        {
            currentPoints[i] = Vector2.Lerp(currentPoints[i], targetPoints[i], Time.deltaTime * speed);
        }
    }

    public void DrawShape(SpriteShapeController spriteShape, Vector2[] points, Vector3 anchor, Vector2 size)
    {
        Draw(spriteShape, points, anchor, size);
    }
    public void DrawShape(SpriteShapeController spriteShape, Vector2[] points, Vector3 anchor)
    {
        Draw(spriteShape, points, anchor, size);
    }
    public void DrawShape(SpriteShapeController spriteShape, Vector2[] points, Vector2 size)
    {
        Draw(spriteShape, points, anchor, size);
    }
    public void DrawShape(SpriteShapeController spriteShape, Vector2[] points)
    {
        Draw(spriteShape, points, anchor, size);
    }
#endregion


#region private methods

    // public void DrawLine(Vector2 pointA, Vector2 pointB, float lineThickness, Color color)
    // {
    //     if (targetCanvas == null) return;
    //     if (Anchor == null) return;
    //     int number = lineDataList.Count;

    //     GameObject line = new GameObject("Line" + number, typeof(Image));
    //     line.transform.SetParent(targetCanvas.transform);
        
    //     Image lineImg = line.GetComponent<Image>();
    //     lineImg.color = color;

    //     Vector2 direction = (pointB - pointA).normalized;
    //     float distance = Vector2.Distance(pointA, pointB);

    //     RectTransform lineRectTransform = line.GetComponent<RectTransform>();
    //     lineRectTransform.anchorMin = Vector2.zero;
    //     lineRectTransform.anchorMax = Vector2.zero;
    //     lineRectTransform.anchoredPosition = pointA + direction * distance * 0.5f;
    //     lineRectTransform.localEulerAngles = new Vector3(0,0, Mathf.Atan2(direction.y,direction.x)*180/Mathf.PI);
    //     lineRectTransform.sizeDelta = new Vector2(distance, lineThickness);
    //     lineRectTransform.localScale = Vector3.one;

    //     lineDataList.Add(new LineData(number, pointA, pointB));
    // }

    private void Draw(SpriteShapeController spriteShape, Vector2[] points, Vector3 anchor, Vector2 size)
    {
        //null handler
        if (spriteShape == null)
        {
            Debug.LogError("SpriteShapeController is null");
            return;
        }
        if (points == null || points.Length == 0)
        {
            Debug.LogError("Points array is null or empty");
            return;
        }



        int splinePointCount = spriteShape.spline.GetPointCount();
        int pointsCount = points.Length;
        if (splinePointCount > pointsCount + 2)
        {
            for (int x = splinePointCount - 1; x >= pointsCount + 2; x--)
            {
                spriteShape.spline.RemovePointAt(x);
            }
        }
        if (splinePointCount > 0)
        {
            spriteShape.spline.SetPosition(0, anchor);
            spriteShape.spline.SetTangentMode(0, ShapeTangentMode.Continuous);
        }
        else
        {
            spriteShape.spline.InsertPointAt(0, anchor);
            spriteShape.spline.SetTangentMode(0, ShapeTangentMode.Continuous);
        }

        int i = 1;
        float graphWidth = points[points.Length - 1].x - points[0].x;
        float ratioX = size.x / graphWidth;
        float ratioY = size.y / graphWidth;
        foreach (Vector2 point in points)
        {
            if (i < splinePointCount)
            {
                spriteShape.spline.SetPosition(i, new Vector3(anchor.x + point.x * ratioX, anchor.y + point.y * ratioY + size.y, anchor.z));
                spriteShape.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            }
            else
            {
                spriteShape.spline.InsertPointAt(i, new Vector3(anchor.x + point.x * ratioX, anchor.y + point.y * ratioY + size.y, anchor.z));
                spriteShape.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            }
            // spriteShape.spline.InsertPointAt(i, new Vector3(anchor.x + point.x * ratioX, anchor.y + point.y * ratioY + size.y, anchor.z));
            // spriteShape.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            i++;
        }
        if (i < splinePointCount)
        {
            spriteShape.spline.SetPosition(i, new Vector3(anchor.x + size.x, anchor.y, anchor.z));
            spriteShape.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
        }
        else
        {
            spriteShape.spline.InsertPointAt(i, new Vector3(anchor.x + size.x, anchor.y, anchor.z));
            spriteShape.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
        }

        spriteShape.spline.isOpenEnded = false;
        
    }

#endregion

    
}
