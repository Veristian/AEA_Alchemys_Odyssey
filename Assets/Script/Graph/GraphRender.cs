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
[System.Serializable]
public class Spring
{
    public float Height;
    public float Speed;

    public void Update(float dampening, float tension)
    {
        float acceleration = -tension * Height;
        Speed += acceleration * Time.deltaTime;
        Speed *= dampening;
        Height += Speed * Time.deltaTime;
    }
}
public class GraphRender : MonoBehaviour
{
#region Fields
    [Header("Wave Settings")]
    [SerializeField] private float springIndex = 0.025f;
    [SerializeField] private float dampening = 0.025f;
    [SerializeField] private float spread = 0.025f;
    [SerializeField] private float tension = 0.025f;

    private Vector3 anchor;
    private Vector2 size;
    // private Spring[] springs;

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
    public void ConvertCurveToSpecifiedPointsLength(AnimationCurve curve, Vector3[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            float x = i / (float)(points.Length - 1); // Scale x to range [0, 1]
            float y = curve.Evaluate(x);
            points[i] = new Vector3(x, y, points[i].z);
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
    public void ConvertCurvesToSpecifiedPointsLength(List<AnimationCurve> curves, Vector3[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            float x = i / (float)(points.Length - 1); // Scale x to range [0, 1]
            float y = 0;
            foreach (var curve in curves)
            {
                y += curve.Evaluate(x);
            }
            points[i] = new Vector3(x, y, points[i].z);
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
    public void LerpVector3List(Vector3[] currentPoints, Vector3[] targetPoints, float speed = 10f)
    {
        if (currentPoints.Length != targetPoints.Length)
        {
            Debug.LogError("Current points and target points must have the same length");
            return;
        }

        for (int i = 0; i < currentPoints.Length; i++)
        {
            currentPoints[i] = Vector3.Lerp(currentPoints[i], targetPoints[i], Time.deltaTime * speed);
        }
    }
    public void LerpMixVectorList(Vector3[] currentPoints, Vector2[] targetPoints, float speed = 1f)
    {
        if (currentPoints.Length != targetPoints.Length)
        {
            Debug.LogError("Current points and target points must have the same length");
            return;
        }

        for (int i = 0; i < currentPoints.Length; i++)
        {
            currentPoints[i].x = Mathf.Lerp(currentPoints[i].x, targetPoints[i].x, Time.deltaTime * speed);
            currentPoints[i].y = Mathf.Lerp(currentPoints[i].y, targetPoints[i].y, Time.deltaTime * speed);
        }
    }

    public void DrawShape(SpriteShapeController spriteShape, Vector3[] points, Vector3 anchor, Vector2 size)
    {
        Draw(spriteShape, points, anchor, size);
    }
    public void DrawShape(SpriteShapeController spriteShape, Vector3[] points, Vector3 anchor)
    {
        Draw(spriteShape, points, anchor, size);
    }
    public void DrawShape(SpriteShapeController spriteShape, Vector3[] points, Vector2 size)
    {
        Draw(spriteShape, points, anchor, size);
    }
    public void DrawShape(SpriteShapeController spriteShape, Vector3[] points)
    {
        Draw(spriteShape, points, anchor, size);
    }
#endregion


#region private methods
    private void Smoothen(Spline waterSpline, int index, int WavesCount)
    {
        Vector3 position = waterSpline.GetPosition(index);
        Vector3 positionPrev = position;
        Vector3 positionNext = position;
        if (index > 1) {
            positionPrev = waterSpline.GetPosition(index-1);
        }
        if (index + 1< WavesCount) {
            positionNext = waterSpline.GetPosition(index+1);
        }
        else if (index + 1 == WavesCount) {
            positionNext = waterSpline.GetPosition(0);
        }

        Vector3 forward = gameObject.transform.forward;

        float scale = Mathf.Min((positionNext - position).magnitude, (positionPrev - position).magnitude) * 0.33f;

        Vector3 leftTangent = (positionPrev - position).normalized * scale;
        Vector3 rightTangent = (positionNext - position).normalized * scale;

        SplineUtility.CalculateTangents(position, positionPrev, positionNext, forward, scale, out rightTangent, out leftTangent);
        
        waterSpline.SetLeftTangent(index, leftTangent);
        waterSpline.SetRightTangent(index, rightTangent);
    }
    private void Draw(SpriteShapeController spriteShape, Vector3[] points, Vector3 anchor, Vector2 size)
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

        // mathes points and wave size
        if (points.Length + 2 != spriteShape.spline.GetPointCount())
        {
            if (points.Length + 2 < spriteShape.spline.GetPointCount())
            {
                for (int i = spriteShape.spline.GetPointCount() - 1; i >= points.Length + 2; i--)
                {
                    spriteShape.spline.RemovePointAt(i);
                }
            }
            else
            {
                for (int i = spriteShape.spline.GetPointCount(); i < points.Length + 2; i++)
                {
                    spriteShape.spline.InsertPointAt(i, anchor);
                }
            }
        }

        float referenceSize = points[points.Length - 1].x - points[0].x;
        
        float ratioX = size.x / referenceSize;
        float ratioY = size.y / referenceSize;

        float[] newHeight = new float[points.Length];
        //calculate springs
        for (int i = 0; i < points.Length; i++)
        {
            newHeight[i] = spriteShape.spline.GetPosition(i + 1).y;
            float xValue = newHeight[i] - (anchor.y + points[i].y * ratioY + size.y);
            float acceleration = (-springIndex * xValue) - (dampening * points[i].z);

            newHeight[i] += points[i].z * Time.deltaTime; // position
            points[i].z += acceleration * Time.deltaTime;   // velocity
        }
        //calculate spread
        float[] leftDeltas = new float[newHeight.Length];
        float[] rightDeltas = new float[newHeight.Length];
                    
        // do some passes where springs pull on their neighbours 
        for (int j = 0; j < 8; j++)
        {
            for (int i = 0; i < newHeight.Length; i++)
            {
                if (i > 0)
                {
                    leftDeltas[i] = spread * (newHeight[i] - newHeight[i - 1]);
                    points[i - 1].z += leftDeltas[i];
                }
                if (i < newHeight.Length - 1)
                {
                    rightDeltas[i] = spread * (newHeight[i] - newHeight[i + 1]);
                    points[i + 1].z += rightDeltas[i];
                }
            }
            for (int i = 0; i < newHeight.Length; i++)
            {
                if (i > 0)
                    points[i - 1].y += leftDeltas[i];
                if (i < newHeight.Length - 1)
                    points[i + 1].y += rightDeltas[i];
            }
        }

        //set positions of each points

        //set bottom left anchor
        spriteShape.spline.SetPosition(0, anchor);
        spriteShape.spline.SetTangentMode(0, ShapeTangentMode.Linear);

        //set ratios
        int x = 1;
        //set pointts
        foreach (Vector3 point in points)
        {            
            spriteShape.spline.SetPosition(x, new Vector3(anchor.x + points[x-1].x * ratioX, anchor.y + newHeight[x - 1] + size.y, anchor.z));
            if (x == 1 || x == points.Length)
            {
                spriteShape.spline.SetTangentMode(x, ShapeTangentMode.Linear);
            }
            else
            {
                spriteShape.spline.SetTangentMode(x, ShapeTangentMode.Continuous);
            }
            x++;
        }
        //set bottom right anchor
        spriteShape.spline.SetPosition(x, new Vector3(anchor.x + size.x, anchor.y, anchor.z));
        spriteShape.spline.SetTangentMode(x, ShapeTangentMode.Linear);

        spriteShape.spline.isOpenEnded = false;

        for (int i = 0; i < spriteShape.spline.GetPointCount(); i++)
        {
            Smoothen(spriteShape.spline, i, spriteShape.spline.GetPointCount());
        }
        
    }


#endregion

    
}
