using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
[Serializable]
[CreateAssetMenu(fileName = "New Ingredient", menuName = "Potion/Ingredient")]
public class IngredientData : ResourceData
{
    [Header("Ingredient Info")]
    public string ingredientId;
    public string ingredientName;
    public GameObject ingredient2DObjectPrefab;
    public Sprite ingredientSprite;
    public string description;
    
    [Header("Ingredient Properties")]
    public AnimationCurve ingredientCurve = AnimationCurve.Linear(0, -10, 1, 10);
    public Color ingredientColor;
    [Range(1, 100)] public int ingredientAreaOfEffect = 100;

    private void Awake()
    {
        ingredientCurve.postWrapMode = WrapMode.Loop;
        ingredientCurve.preWrapMode = WrapMode.Loop;
    }

    private void OnValidate()
    {
        ingredientCurve.postWrapMode = WrapMode.Loop;
        ingredientCurve.preWrapMode = WrapMode.Loop;
    }
}
