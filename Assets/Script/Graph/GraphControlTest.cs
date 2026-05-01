// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.U2D; 
// [RequireComponent(typeof(GraphRender))]
// public class GraphControlTest : MonoBehaviour
// {
//     public Canvas canvas;
//     public SpriteShapeController spriteShapeController;
//     // Start is called before the first frame update
//     GraphRender graphRender;
//     public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
//     Vector2[] points;
//     void Start()
//     {
//         graphRender = GetComponent<GraphRender>();
//         graphRender.SetTargetCanvas(canvas);
//         points = new Vector2[25];

        
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         graphRender.ConvertCurveToSpecifiedPointsLength(curve, points);
//         graphRender.DrawShape(spriteShapeController, points);
//     }
// }


