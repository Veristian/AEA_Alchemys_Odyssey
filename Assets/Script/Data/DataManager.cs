using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using UnityEngine.Events;



public class DataManager : Singleton<DataManager>
{
    public List<IngredientData> ingredientDatas;
    public List<PotionData> potionDatas;
    private const string ingredientDatasPath = "Ingredient";
    private const string potionDatasPath = "Potion";
    public event Action OnGameLoaded;
    public event Action OnGameSaved;
    public bool IsGameLoaded { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        LoadGameData();
    }
    [ContextMenu("Load Game Data")]
    private void LoadGameData()
    {
        ingredientDatas = ResourceLoader
            .GetAll<IngredientData>(ingredientDatasPath)
            .ToList();

        potionDatas = ResourceLoader
            .GetAll<PotionData>(potionDatasPath)
            .ToList();

        InventoryManager.Instance.Load();
        PlayerDataManager.Instance.Load();
        IsGameLoaded = true;
        OnGameLoaded?.Invoke();
    }
    [ContextMenu("Save Game Data")]
    private void SaveGameData()
    {
        InventoryManager.Instance.Save();
        PlayerDataManager.Instance.Save();
        OnGameSaved?.Invoke();
    }

    public void SaveToFile(string path, object data)
    {
        string fullPath = System.IO.Path.Combine(Application.persistentDataPath, path + ".json");
        string json = SaveUtility.Serialize(data);
        System.IO.File.WriteAllText(fullPath, json);
        Debug.Log("Saved to " + fullPath);
    }
    public T LoadFromFile<T>(string path, Func<T> defaultFactory)
    {
        string fullPath = System.IO.Path.Combine(Application.persistentDataPath, path + ".json");

        if (System.IO.File.Exists(fullPath))
        {
            string json = System.IO.File.ReadAllText(fullPath);
            Debug.Log("Loaded from " + fullPath);
            return SaveUtility.Deserialize<T>(json);
        }
    
        return defaultFactory();
    }

}