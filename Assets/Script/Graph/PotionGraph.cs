using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D; 
[RequireComponent(typeof(GraphRender))]
public class PotionGraph : MonoBehaviour
{
    [SerializeField] private SpriteShapeController spriteShapeController;

    private GraphRender graphRender;
    public List<AnimationCurve> potionCurves = new List<AnimationCurve>();
    Vector2[] points;
    private void Start()
    {
        graphRender = GetComponent<GraphRender>();
        points = new Vector2[25];
        
    }
    
    // Update is called once per frame
    private void Update()
    {
        graphRender.ConvertCurvesToSpecifiedPointsLength(potionCurves, points);
        graphRender.DrawShape(spriteShapeController, points, Vector3.zero, new Vector2(100, 100));
    }
}


