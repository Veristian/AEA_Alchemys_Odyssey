using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class GraphRender : Singleton<GraphRender>
{
#region Fields
    [Header("Wave Settings")]
    [SerializeField] private float springIndex = 0.025f;
    [SerializeField] private float dampening = 0.025f;
    [SerializeField] private float spread = 0.025f;

    private Vector3 anchor;
    private Vector2 size;


    public void SetAnchor(Vector2 newAnchor)
    {
        anchor = newAnchor;
    }

    public void SetSize(Vector2 newSize)
    {
        size = newSize;
    }



#endregion

#region Init


#endregion
#region public methods
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
    public void ConvertSpecifiedPointsToCurve(AnimationCurve curve, Vector3[] points)
    {
        Keyframe[] keyframes = new Keyframe[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            keyframes[i] = new Keyframe(points[i].x, points[i].y);
        }
        curve.keys = keyframes;
    }
    public void ConvertSpecifiedPointsToCurve(AnimationCurve curve, Vector2[] points)
    {
        if (points == null || points.Length == 0) return;

        Keyframe[] keyframes = new Keyframe[points.Length];

        // Create keyframes
        for (int i = 0; i < points.Length; i++)
        {
            keyframes[i] = new Keyframe(points[i].x, points[i].y);
        }

        // Assign first so we can modify via MoveKey later
        curve.keys = keyframes;

        for (int i = 0; i < keyframes.Length; i++)
        {
            Keyframe key = keyframes[i];

            float tangent;

            if (i == 0)
            {
                // First point: slope to next point
                tangent = GetSlope(points[i], points[i + 1]);
                key.inTangent = tangent;
                key.outTangent = tangent;
            }
            else if (i == keyframes.Length - 1)
            {
                // Last point: slope from previous point
                tangent = GetSlope(points[i - 1], points[i]);
                key.inTangent = tangent;
                key.outTangent = tangent;
            }
            else
            {
                // Middle points: average slope (Catmull-Rom style)
                float slopePrev = GetSlope(points[i - 1], points[i]);
                float slopeNext = GetSlope(points[i], points[i + 1]);
                tangent = (slopePrev + slopeNext) * 0.5f;

                key.inTangent = tangent;
                key.outTangent = tangent;
            }

            curve.MoveKey(i, key);
        }
    }

    private float GetSlope(Vector2 a, Vector2 b)
    {
        float dx = b.x - a.x;

        if (Mathf.Approximately(dx, 0f))
            return 0f;

        return (b.y - a.y) / dx;
    }

    // public AnimationCurve ScaleCurve(AnimationCurve curve, float maxX, float maxY)
    // {
    //     AnimationCurve scaledCurve = new AnimationCurve();
    //     for (int i = 0; i < curve.keys.Length; i++)
    //     {
    //         Keyframe keyframe = curve.keys[i];
    //         keyframe.value = curve.keys[i].value * maxY;
    //         keyframe.time = curve.keys[i].time * maxX;
    //         keyframe.inTangent = curve.keys[i].inTangent * maxY / maxX;
    //         keyframe.outTangent = curve.keys[i].outTangent * maxY / maxX;
            
    //         scaledCurve.AddKey(keyframe);
    //     }
    //     return scaledCurve;
    // }

    
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

    public void DrawShape(SpriteShapeController[] spriteShape, Vector3[] points, Vector3 anchor, Vector2 size, float graphDefaultRest, bool topAnchor = false)
    {
        Draw(spriteShape, points, anchor, size, graphDefaultRest, topAnchor);
    }
    public void DrawShape(SpriteShapeController[] spriteShape, Vector3[] points, Vector3 anchor)
    {
        Draw(spriteShape, points, anchor, size, 0.5f);
    }
    public void DrawShape(SpriteShapeController[] spriteShape, Vector3[] points, Vector2 size)
    {
        Draw(spriteShape, points, anchor, size, 0.5f);
    }
    public void DrawShape(SpriteShapeController[] spriteShape, Vector3[] points)
    {
        Draw(spriteShape, points, anchor, size, 0.5f);
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
    private void Draw(SpriteShapeController[] spriteShape, Vector3[] points, Vector3 anchor, Vector2 size, float graphDefaultRest, bool topAnchor = false)
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
        foreach (SpriteShapeController controller in spriteShape)
        {
            // mathes points and wave size
            if (points.Length + 2 != controller.spline.GetPointCount())
            {
                if (points.Length + 2 < controller.spline.GetPointCount())
                {
                    for (int i = controller.spline.GetPointCount() - 1; i >= points.Length + 2; i--)
                    {
                        controller.spline.RemovePointAt(i);
                    }
                }
                else
                {
                    for (int i = controller.spline.GetPointCount(); i < points.Length + 2; i++)
                    {
                        controller.spline.InsertPointAt(i, anchor);
                    }
                }
            }

        }
        
        float referenceSize = points[points.Length - 1].x - points[0].x;
        
        float ratioX = size.x / referenceSize;
        // float ratioY = size.y / referenceSize;

        float[] newHeight = new float[points.Length];
        //calculate springs
        for (int i = 0; i < points.Length; i++)
        {
            newHeight[i] = spriteShape[0].spline.GetPosition(i + 1).y;

            float xValue = newHeight[i] - (anchor.y + points[i].y + size.y * graphDefaultRest);
            //debug log to show X Value
            // Debug.DrawLine(new Vector3(anchor.x + points[i].x * ratioX + 1, anchor.y + newHeight[i] + size.y * graphDefaultRest, anchor.z), new Vector3(anchor.x + points[i].x * ratioX + 1, anchor.y + newHeight[i] + size.y * graphDefaultRest - xValue, anchor.z), Color.yellow);
            float acceleration = (-springIndex * xValue) - (dampening * points[i].z);
            //debug line to show the spring force
            // Debug.DrawLine(new Vector3(anchor.x + points[i].x * ratioX + 2, anchor.y + newHeight[i] + size.y * graphDefaultRest, anchor.z), new Vector3(anchor.x + points[i].x * ratioX + 2, anchor.y + points[i].y + size.y * graphDefaultRest, anchor.z), Color.red);

            newHeight[i] += points[i].z * Time.deltaTime; // position
            points[i].z += acceleration * Time.deltaTime;   // velocity

            //debug line to show the current height and the target height
            // Debug.DrawLine(new Vector3(anchor.x + points[i].x * ratioX + 3, anchor.y + newHeight[i] + size.y * graphDefaultRest, anchor.z), new Vector3(anchor.x + points[i].x * ratioX + 3, anchor.y + points[i].y + size.y * graphDefaultRest, anchor.z), Color.green);
            
            // Debug.DrawLine(new Vector3(anchor.x + points[i].x * ratioX + 4, anchor.y + points[i].y + size.y * graphDefaultRest, anchor.z), new Vector3(anchor.x + points[i].x * ratioX + 4, anchor.y + size.y * graphDefaultRest, anchor.z), Color.blue);
        }
        //calculate spread
        float[] leftDeltas = new float[newHeight.Length];
        float[] rightDeltas = new float[newHeight.Length];
                    
        // do some passes where springs pull on their neighbours 
        //note to self: convert this to use points as baseline and not 0 0
        for (int j = 0; j < 1; j++)
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
        foreach (SpriteShapeController controller in spriteShape)
        {  
            if (topAnchor)
            {
                controller.spline.SetPosition(0, new Vector3(anchor.x, anchor.y + size.y, anchor.z));
            }
            else
            {
                controller.spline.SetPosition(0, anchor);
            }
            controller.spline.SetTangentMode(0, ShapeTangentMode.Linear);

            //set ratios
            int x = 1;
            //set pointts
            foreach (Vector3 point in points)
            {            
                                
                if (anchor.y + newHeight[x - 1] == anchor.y)
                {
                    controller.spline.SetPosition(x, new Vector3(anchor.x + points[x-1].x * ratioX, anchor.y + size.y * graphDefaultRest, anchor.z));
                }
                else
                {
                    controller.spline.SetPosition(x, new Vector3(anchor.x + points[x-1].x * ratioX, anchor.y + newHeight[x - 1], anchor.z));
                }
                if (x == 1 || x == points.Length)
                {
                    controller.spline.SetTangentMode(x, ShapeTangentMode.Linear);
                }
                else
                {
                    controller.spline.SetTangentMode(x, ShapeTangentMode.Continuous);
                }
                x++;
            }
            //set bottom right anchor
            if (topAnchor)
            {
                controller.spline.SetPosition(x, new Vector3(anchor.x + size.x, anchor.y + size.y, anchor.z));
            }
            else
            {
                controller.spline.SetPosition(x, new Vector3(anchor.x + size.x, anchor.y, anchor.z));
            }
            controller.spline.SetTangentMode(x, ShapeTangentMode.Linear);

            controller.spline.isOpenEnded = false;

            for (int i = 0; i < controller.spline.GetPointCount(); i++)
            {
                Smoothen(controller.spline, i, controller.spline.GetPointCount());
            }

        }
        
    }


#endregion

    
}
