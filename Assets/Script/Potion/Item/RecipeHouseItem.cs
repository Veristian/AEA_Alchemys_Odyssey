using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RecipeHouseItem : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Button button;  
    [SerializeField] private TextMeshProUGUI potionNameText;
    [SerializeField] private TextMeshProUGUI potionDetailText;

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
        if (recipe == null)
        {
            potionNameText.text = "Empty Slot";
            potionDetailText.text = "";
            return;
        }
        if (recipe.targetPotion == null)
        {
            potionNameText.text = "Empty Slot";
            potionDetailText.text = "";
            return;
        }
        if (potionNameText == null || potionDetailText == null)
        {
            Debug.LogWarning("Potion Name Text or Potion Detail Text is not assigned in the inspector.");
            return;
        } 
        potionNameText.text = recipe.targetPotion.potionName;
        potionDetailText.text = recipe.targetPotion.description;
    }

    private void SetTarget()
    {
        PotionManager.Instance.AssignPotionTarget(recipe.targetPotion);
    }


}
