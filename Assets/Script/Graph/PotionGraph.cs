using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D; 
[RequireComponent(typeof(GraphRender))]
public class PotionGraph : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteShapeController spriteShapeController;
    private GraphRender graphRender;

    [Header("Graph Settings")]
    [SerializeField] private int pointsLength = 25;
    [SerializeField] private Vector2 graphSize = new Vector2(100, 100);
    [SerializeField] private Transform graphOrigin;
    [Header("Curves")]
    [SerializeField] private List<AnimationCurve> potionCurves = new List<AnimationCurve>();

    [Header("Debug")]
    [SerializeField] private Vector2[] targetPoints;
    [SerializeField] private Vector3[] currentPoints; //vector 2 for position and z for velocity
    private void Awake()
    {
        graphRender = GetComponent<GraphRender>();
        Initialize();
    }

    private void Initialize()
    {
        if (graphOrigin == null)
        {
            Debug.LogWarning("Graph Origin is not assigned. Please assign a Transform to graphOrigin.");
            graphOrigin = this.transform;
        }
        targetPoints = new Vector2[pointsLength];
        currentPoints = new Vector3[pointsLength];
        for (int i = 0; i < pointsLength; i++)
        {
            targetPoints[i] = Vector2.zero;
            currentPoints[i] = Vector3.zero;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        graphRender.ConvertCurvesToSpecifiedPointsLength(potionCurves, targetPoints);
        graphRender.LerpMixVectorList(currentPoints, targetPoints, 100000f);
        graphRender.DrawShape(spriteShapeController, currentPoints, graphOrigin.position, graphSize);
    }


#region Public Methods
    public void AddCurve(AnimationCurve curve)
    {
        potionCurves.Add(curve);
    }

    public void RemoveLastCurve()
    {
        if (potionCurves.Count > 0)
            potionCurves.RemoveAt(potionCurves.Count - 1);
    }
#endregion


}


