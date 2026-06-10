using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeHouse : Singleton<RecipeHouse>
{
    [Header("Reference")]
    public Transform contentTransform;

    public GameObject recipeHouseItemPrefab;
    public List<RecipeHouseItem> recipeHouseItems;

    private void OnEnable()
    {
        var manager = DataManager.Instance;
        manager.OnGameLoaded += SetupRecipes;

        if (manager.IsGameLoaded)
            SetupRecipes(); 
    }
    private void OnDisable()
    {
        if (DataManager.Instance != null)
            DataManager.Instance.OnGameLoaded -= SetupRecipes;
    }

    private void SetupRecipes()
    {
        if (contentTransform == null)
        {
            Debug.LogWarning("Content Transform reference is not assigned. Please assign a Transform reference to contentTransform in the inspector.");
            return;
        }
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
        recipeHouseItems.Clear();
        foreach (Recipe recipe in PlayerDataManager.Instance.RecipeList.recipes)
        {
            RecipeHouseItem item = Instantiate(recipeHouseItemPrefab, contentTransform).GetComponent<RecipeHouseItem>();
            recipeHouseItems.Add(item);
            item.AssignRecipe(recipe);
        }
    }

}
