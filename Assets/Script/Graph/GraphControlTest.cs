using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GraphRender))]
public class GraphControlTest : MonoBehaviour
{
    public Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        GraphRender graphRender = GetComponent<GraphRender>();
        graphRender.SetTargetCanvas(canvas);

        for (float i = 0; i < 100; i++)
        {
            float x1 = Mathf.Pow( i - 50, 2);
            float y1 = i*10;
            float x2 = Mathf.Pow( i - 49, 2);
            float y2 = (i+1)*10;
            graphRender.DrawLine(new Vector2(y1, x1), new Vector2(y2, x2),3,Color.black);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
