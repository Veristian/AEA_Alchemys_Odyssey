using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Potion/Potion")]
public class PotionData : ScriptableObject
{
    [Header("Potion Info")]
    public string potionId;
    public string potionName;
    public Sprite potionSprite;
    public string description;

    [Header("Potion Properties")]
    public Color potionColor;
    public List<IngredientData> ingredients = new List<IngredientData>();
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
            ingredientCurves.Add(ingredients[i].ingredientCurve);
        }
        Vector2[] potionPoints = new Vector2[100];

        GraphRender.Instance.ConvertCurvesToSpecifiedPointsLength(ingredientCurves, potionPoints);
        GraphRender.Instance.ConvertSpecifiedPointsToCurve(potionCurve, potionPoints);
        
    }
}
