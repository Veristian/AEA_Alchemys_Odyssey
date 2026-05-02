using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Potion/Ingredient")]
public class IngredientData : ScriptableObject
{
    [Header("Ingredient Info")]
    public string ingredientId;
    public string ingredientName;
    public Sprite ingredientSprite;
    public string description;
    
    [Header("Ingredient Properties")]
    public AnimationCurve ingredientCurve = AnimationCurve.Linear(0, -10, 1, 10);
    public Color ingredientColor;
    [Range(1, 100)] public int ingredientAreaOfEffect = 100;
    
}
