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
    // public bool isQuestRecipe;
    // public bool isBulkCraftable;

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
[Serializable, Metadata("Gold")]
public class GoldData
{
    public int gold;
}
[Serializable, Metadata("Unlocks")]
public class UnlocksData
{
    public List<UnlockData> unlockData;
}
[Serializable]
public class UnlockData
{
    public string itemGroupId;
    public bool isUnlocked;
}
public class PlayerDataManager : Singleton<PlayerDataManager>
{
    //add day 
    private const string RecipeListFileName = "PlayerRecipeData";
    private const string DayFileName = "PlayerDayData";
    private const string GoldFileName = "PlayerGoldData";
    private const string UnlocksFileName = "PlayerUnlocksData";

    [SerializeField] private RecipeList recipeList;
    public int day;
    public int gold;
    [SerializeField] private UnlocksData gameObjectUnlocks;
    public UnlocksData GameObjectUnlocks
    {
        get { return gameObjectUnlocks; }
    }
    public RecipeList RecipeList
    {
        get { return recipeList; }
    }

    protected override void Awake()
    {
        transform.parent = null;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    //save inventory data to json path
    public void Save()
    {
        recipeList.recipes.ForEach(r => r.OnBeforeSerialize());
        DataManager.Instance.SaveToFile(RecipeListFileName, recipeList);
        DataManager.Instance.SaveToFile(DayFileName, new DayData { day = day });
        DataManager.Instance.SaveToFile(GoldFileName, new GoldData { gold = gold });
        DataManager.Instance.SaveToFile(UnlocksFileName, gameObjectUnlocks);
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
        recipeList.recipes.RemoveAll(i => i.targetPotion == null);

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
                // isQuestRecipe = false,
                // isBulkCraftable = false
            });

        recipeList.recipes.AddRange(newEntries);
        gold = DataManager.Instance.LoadFromFile(GoldFileName, () => new GoldData { gold = 0 }).gold;

        gameObjectUnlocks = DataManager.Instance.LoadFromFile(
            UnlocksFileName,
            () => new UnlocksData { unlockData = new List<UnlockData>() }
        );

        InitializeUnlockedObjects();
        DayManager.Instance.SetDay(DataManager.Instance.LoadFromFile(DayFileName, () => new DayData { day = 1 }).day);

        
    }

    //called on first load and by unlock manager after game loaded
    public void InitializeUnlockedObjects()
    {
        if (UnlockManager.Instance != null)
        {
            Debug.Log("Unlocking Objects");
            //get unlock data from scene that is missing from save

            //check unlock manager and get missing
            foreach (GroupObjectData groupObjectData in UnlockManager.Instance.gameObjectUnlockables)
            {
                if (!gameObjectUnlocks.unlockData.Any(u => u.itemGroupId == groupObjectData.unlockableObjectsId))
                {
                    gameObjectUnlocks.unlockData.Add(new UnlockData
                    {
                        itemGroupId = groupObjectData.unlockableObjectsId,
                        isUnlocked = false
                    });
                }
            }
            //iterate through list for all unlocked objects to be enabled
            foreach (UnlockData data in gameObjectUnlocks.unlockData)
            {
                if (data.isUnlocked)
                    UnlockManager.Instance.UnlockObject(data.itemGroupId);
            }
        }
        
    }

    public void SetUnlock(string id, bool unlocked = true)
    {
        UnlockData unlockData = gameObjectUnlocks.unlockData.Find(g => g.itemGroupId == id);
        if (unlockData == null) 
        {
            Debug.LogWarning($"UnlockData with id {id} not found in gameObjectUnlocks.");
            if (unlocked)
            {
                unlockData = new UnlockData
                {
                    itemGroupId = id,
                    isUnlocked = true
                };
                gameObjectUnlocks.unlockData.Add(unlockData);                
                if (UnlockManager.Instance != null && unlocked)
                {
                    UnlockManager.Instance.UnlockObject(unlockData.itemGroupId);
                }                    
                return;
            }
            else
            {
                return;
            }
        }
        if (unlockData.isUnlocked) return;
        unlockData.isUnlocked = unlocked;
        if (UnlockManager.Instance != null && unlocked)
        {
            UnlockManager.Instance.UnlockObject(unlockData.itemGroupId);
        }
    }

    public void AddGold(int gold)
    {
        this.gold += gold;
    }
    public bool SubtractGold(int gold)
    {
        if (this.gold > gold)
        {
            this.gold -= gold;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsUnlocked(string id)
    {
        UnlockData unlockData = gameObjectUnlocks.unlockData.Find(g => g.itemGroupId == id);
        if (unlockData == null) return false;
        return unlockData.isUnlocked;
    }


}
