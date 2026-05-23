using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[Serializable]
public class Recipe
{
    public PotionData targetPotion;
    public bool isUnlocked;
    public bool isQuestRecipe;
    public bool isBulkCraftable;
}
[Serializable]
public class RecipeList
{
    public List<Recipe> recipes;
}
public class PlayerDataManager : Singleton<PlayerDataManager>
{
    private const string RecipeListFileName = "PlayerRecipeData";
    [SerializeField] private RecipeList recipeList;
    public RecipeList RecipeList
    {
        get { return recipeList; }
    }

    //save inventory data to json path
    public void Save()
    {
        DataManager.Instance.SaveToFile(RecipeListFileName, recipeList);
        Debug.Log("Player Data Saved");
    }

    //get inventory data from json path and return new data set if null
    public void Load()
    {
        recipeList = DataManager.Instance.LoadFromFile(
            RecipeListFileName,
            () => new RecipeList { recipes = new List<Recipe>() }
        );

        Debug.Log("Player Data Loaded");

        var existingIds = new HashSet<string>(
            recipeList.recipes
                .Select(rcp => rcp.targetPotion.potionId)
        );

        var newEntries = DataManager.Instance.potionDatas
            .Where(data => !existingIds.Contains(data.potionId))
            .Select(data => new Recipe
            {
                targetPotion = data,
                isUnlocked = false,
                isQuestRecipe = false,
                isBulkCraftable = false
            });

        recipeList.recipes.AddRange(newEntries);

    }

}
