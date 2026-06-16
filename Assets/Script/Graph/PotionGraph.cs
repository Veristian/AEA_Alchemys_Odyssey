using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D; 
public class PotionGraph : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteShapeController[] spriteShapeController;

    public SpriteShapeController[] SpriteShapeController
    {
        get { return spriteShapeController; }
    }
    [Header("Graph Settings")]
    [SerializeField] private int pointsLength = 25;
    [SerializeField] private Vector2 graphSize = new Vector2(100, 100);
    [SerializeField, Range(0.25f, 0.75f)] private float graphDefaultRest = 0.5f;
    [SerializeField] public Vector3 graphOrigin;
    [SerializeField] private bool topAnchor = false;

    [Header("Curves")]
    [SerializeField] private bool clearOnStartup = true;
    [SerializeField] private List<AnimationCurve> potionCurves = new List<AnimationCurve>();
    [SerializeField] public float restOffset = 0f;
    [SerializeField, Range(0f, 0.4f)] public float bottomPaddingRatio = 0f;
    [SerializeField, Range(0f, 0.4f)] public float topPaddingRatio = 0f;

    public List<AnimationCurve> PotionCurves
    {
        get {return potionCurves;}
    }

    [Header("Debug")]
    [SerializeField] private Vector3[] currentPoints; //vector 2 for position and z for velocity
    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (clearOnStartup)
            potionCurves.Clear();
        if (graphOrigin == null)
        {
            Debug.LogWarning("Graph Origin is not assigned. Please assign a Transform to graphOrigin.");
            graphOrigin = Vector3.zero;
        }
        currentPoints = new Vector3[pointsLength];
        for (int i = 0; i < pointsLength; i++)
        {
            currentPoints[i] = Vector3.zero;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (spriteShapeController.Length == 0 || spriteShapeController[0] == null)
        {
            Debug.LogWarning("No SpriteShapeController assigned. Please assign at least one SpriteShapeController to draw the graph.");
            return;
        }
        if (currentPoints.Length != pointsLength)
        {
            currentPoints = new Vector3[pointsLength];
        }
        if (graphSize.x <= 0 || graphSize.y <= 0)
        {
            Debug.LogWarning("Graph size must be greater than zero. Please set a valid graph size.");
            return;
        }
        if (GraphRender.Instance == null)
        {
            Debug.LogError("GraphRender instance not found. Please ensure GraphRender is properly initialized in the scene.");
            return;
        }
        GraphRender.Instance.ConvertCurvesToSpecifiedPointsLength(potionCurves, currentPoints);
        GraphRender.Instance.DrawShape(spriteShapeController, currentPoints, graphOrigin, graphSize, graphDefaultRest, restOffset, bottomPaddingRatio, topPaddingRatio, topAnchor);  
    }


#region Public Methods
    public void SetGraphProperties(int newPointsLength, Vector2 newGraphSize, float newGraphDefaultRest, Vector3 newGraphOrigin)
    {
        pointsLength = newPointsLength;
        graphSize = newGraphSize;
        graphDefaultRest = newGraphDefaultRest;
        graphOrigin = newGraphOrigin;
    }
    public void RemoveLastCurve()
    {
        if (potionCurves.Count > 0)
            potionCurves.RemoveAt(potionCurves.Count - 1);
    }

    public void ClearCurves()
    {
        potionCurves.Clear();
    }

    public void UpdatePotionCurveAtIndex(int index, AnimationCurve curve)
    {
        if (index > potionCurves.Count || index < 0)
        {
            Debug.LogError("Index out of range");
            return;
        }
        potionCurves[index] = curve;
    }
    public void RemovePotionCurveAtIndex(int index)
    {
        if (index > potionCurves.Count - 1 || index < 0)
        {
            Debug.LogError("Index out of range");
            return;
        }
        potionCurves.Remove(potionCurves[index]);
    }

    public void UpdateLatestPotionCurve(AnimationCurve curve)
    {
        UpdatePotionCurveAtIndex(potionCurves.Count, curve);
    }
   public void FixPotionCurveNumber(int amount)
    {
        if (potionCurves.Count > amount)
        {
            for (int i = potionCurves.Count - 1; i >= amount; i--)
            {
                RemovePotionCurveAtIndex(i);
            }
        }
        else if (potionCurves.Count < amount)
        {
            for (int i = potionCurves.Count; i < amount; i++)
            {
                AddEmptyCurve();
            }
        }
    }

    public void AddEmptyCurve()
    {
        AnimationCurve emptyCurve = AnimationCurve.Linear(0,0,1,0);
        emptyCurve.preWrapMode = WrapMode.Loop;
        emptyCurve.postWrapMode = WrapMode.Loop;

        potionCurves.Add(emptyCurve);
    }
    public void SetPotionCurves(List<AnimationCurve> newCurves)
    {
        potionCurves = newCurves;
    }
    public void AddPotionCurve(AnimationCurve newCurve)
    {
        potionCurves.Add(newCurve);
    }
    public void SetPotionCurves(List<AnimationCurve> newCurves, float accuracyRequired)
    {
        potionCurves = new List<AnimationCurve>();

        foreach (var curve in newCurves)
        {
            AnimationCurve newCurve = new AnimationCurve();

            foreach (var key in curve.keys)
            {
                Keyframe newKey = key;
                newKey.value += accuracyRequired;
                newCurve.AddKey(newKey);
            }
            potionCurves.Add(newCurve);
        }
    }

    public void AddPotionCurve(AnimationCurve newCurve, float accuracyRequired)
    {
        AnimationCurve curve = new AnimationCurve();
        
        foreach (var key in newCurve.keys)
        {
            Keyframe newKey = key;
            newKey.value += accuracyRequired;
            curve.AddKey(newKey);
        }
        curve.preWrapMode = WrapMode.Loop;
        curve.postWrapMode = WrapMode.Loop;
        potionCurves.Add(curve);
    }

    public void AddPotionCurve(List<AnimationCurve> newCurves, float accuracyRequired = 0)
    {
        foreach (var curve in newCurves)
        {
            AnimationCurve newCurve = new AnimationCurve();

            foreach (var key in curve.keys)
            {
                Keyframe newKey = key;
                newKey.value += accuracyRequired;
                newCurve.AddKey(newKey);
            }
            curve.preWrapMode = WrapMode.Loop;
            curve.postWrapMode = WrapMode.Loop;

            potionCurves.Add(newCurve);
        }
    }

    public void SetAnchorTop(bool top)
    {
        topAnchor = top;
    }
    

    public Vector3[] GetPotionPoints()
    {
        return currentPoints;
    }

    public void Splash(float contactPoint, float force)
    {
        int index = Mathf.RoundToInt(Mathf.Clamp((contactPoint*(float)pointsLength),1,pointsLength));
        
        currentPoints[index-1].z += force;
    }
#endregion


}


