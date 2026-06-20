using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientCurveDisplay : Singleton<IngredientCurveDisplay>
{

    [ReadOnly] public IngredientInventoryData ingredientInventoryData;
    [Header("References")]
    [SerializeField] private GameObject displayBoard;
    [SerializeField] private PotionGraph graph;
    [Header("Graph Settings")]
    [SerializeField] private int pointsLength = 10;
    [SerializeField] private Vector2 graphSize = new Vector2(10, 10);
    [SerializeField, Range(0.25f, 0.75f)] private float graphDefaultRest = 0.5f;
    [SerializeField] public Vector3 graphOrigin;
    [SerializeField] private Camera targetCamera;
    protected override void Awake()
    {
        base.Awake();
        if (targetCamera == null)
            targetCamera = Camera.main;
        displayBoard.SetActive(false);
    }

    private void Update()
    {
        if (!displayBoard.activeSelf) return;

        FollowMouseWithFlip();
    }
    public void SetVisibility(bool visible)
    {
        if (displayBoard)
            displayBoard.SetActive(visible);

        if (ingredientInventoryData != null && graph)
        {
            graph.SetGraphProperties(pointsLength, graphSize, graphDefaultRest, graphOrigin);
            graph.SetPotionCurves(new List<AnimationCurve>
            {
                ingredientInventoryData.ingredientData.ingredientCurve
            });
        }
    }

    private void FollowMouseWithFlip()
    {
        Vector3 worldPos = targetCamera.ScreenToWorldPoint(
        new Vector3(InputManager.Instance.MousePosition.x, InputManager.Instance.MousePosition.y, 5f) // adjust Z distance
    );

        displayBoard.transform.position = worldPos;
    }


}
