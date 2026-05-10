using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Potion/Potion")]
public class PotionData : ResourceData
{
    [Header("Potion Info")]
    public string potionId;
    public string potionName;
    public Sprite potionSprite;
    public string description;

    [Header("Potion Properties")]
    public Color potionColor;
    public List<StoredData> ingredients = new List<StoredData>();
    [SerializeField, ReadOnly] private AnimationCurve potionCurve;

    [ContextMenu("Generate Potion Curve")]
    public void GeneratePotionCurve()
    {
        if (ingredients.Count == 0)
        {
            potionCurve = AnimationCurve.Linear(0, 0, 1, 0);
            return;
        }
        potionCurve = new AnimationCurve();
        List<AnimationCurve> ingredientCurves = new List<AnimationCurve>();
        for (int i = 0; i < ingredients.Count; i++)
        {
            ingredientCurves.Add(AdjustCurveToContactPoint(ingredients[i].ingredientData, ingredients[i].contactPoint));
        }
        Vector2[] potionPoints = new Vector2[100];

        GraphRender.Instance.ConvertCurvesToSpecifiedPointsLength(ingredientCurves, potionPoints);
        GraphRender.Instance.ConvertSpecifiedPointsToCurve(potionCurve, potionPoints);
        
    }
    private AnimationCurve AdjustCurveToContactPoint(IngredientData data, float contactPoint)
    {
        AnimationCurve newCurve = new AnimationCurve();
        newCurve.CopyFrom(data.ingredientCurve);
        Vector2[] points = new Vector2[10];
        GraphRender.Instance.ConvertCurveToSpecifiedPointsLength(data.ingredientCurve, points);
        for (int i = 0; i < points.Length; i++)
        {
            points[i].x *= (float)data.ingredientAreaOfEffect/100f;
            points[i].x += (contactPoint*100f)/(float)data.ingredientAreaOfEffect;
        }
        GraphRender.Instance.ConvertSpecifiedPointsToCurve(newCurve, points);
        return newCurve;

    }
}
