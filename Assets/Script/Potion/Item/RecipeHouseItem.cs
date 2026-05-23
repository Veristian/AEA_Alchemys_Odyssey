using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RecipeHouseItem : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Button button;  
    [Header("Recipe Info")]
    [SerializeField] private Recipe recipe;
    public Recipe Recipe
    {
        get {return recipe;}
    }

    private void OnEnable()
    {
        button.onClick.AddListener(SetTarget);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(SetTarget);
    }

    public void AssignRecipe(Recipe recipe)
    {
        this.recipe = recipe;      
    }

    private void SetTarget()
    {
        PotionManager.Instance.AssignPotionTarget(recipe.targetPotion);
    }


}
