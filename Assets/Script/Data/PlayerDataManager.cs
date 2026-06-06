using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[Serializable]
public class Recipe
{
    public PotionData targetPotion;
    [ReadOnly] public string targetPotionId;
    public bool isUnlocked;
    public bool isQuestRecipe;
    public bool isBulkCraftable;

    public void OnBeforeSerialize()
    {
        if (targetPotion != null)
            targetPotionId = targetPotion.potionId;
    }
    public void OnAfterDeserialize()
    {
        if (!string.IsNullOrEmpty(targetPotionId))
        {
            targetPotion = DataManager.Instance.potionDatas
                .Find(data => data.potionId == targetPotionId);
        }
    }
}
[Serializable]
[Metadata("Recipes")]
public class RecipeList
{
    public List<Recipe> recipes;
}
[Serializable, Metadata("Day")]
public class DayData
{
    public int day;
}
public class PlayerDataManager : Singleton<PlayerDataManager>
{
    //add day 
    private const string RecipeListFileName = "PlayerRecipeData";
    private const string DayFileName = "PlayerDayData";
    [SerializeField] private RecipeList recipeList;
    private int day => DayManager.Instance.Day;
    public RecipeList RecipeList
    {
        get { return recipeList; }
    }

    //save inventory data to json path
    public void Save()
    {
        recipeList.recipes.ForEach(r => r.OnBeforeSerialize());
        DataManager.Instance.SaveToFile(RecipeListFileName, recipeList);
        DataManager.Instance.SaveToFile(DayFileName, new DayData { day = day });
        Debug.Log("Player Data Saved");
    }

    //get inventory data from json path and return new data set if null
    public void Load()
    {
        recipeList = DataManager.Instance.LoadFromFile(
            RecipeListFileName,
            () => new RecipeList { recipes = new List<Recipe>() }
        );

        recipeList.recipes.ForEach(r => r.OnAfterDeserialize());

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
        DayManager.Instance.SetDay(DataManager.Instance.LoadFromFile(DayFileName, () => new DayData { day = 0 }).day);


    }

}
