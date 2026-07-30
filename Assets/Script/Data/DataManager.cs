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
    public List<QuestData> questDatas;
    public List<TutorialData> tutorialDatas;
    private const string ingredientDatasPath = "Ingredient";
    private const string potionDatasPath = "Potion";
    private const string questDatasPath = "Quest";
    private const string tutorialDatasPath = "Tutorial";
    public event Action OnGameLoaded;
    public event Action OnGameSaved;
    public bool IsGameLoaded { get; private set; }
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
        if (!IsGameLoaded && Instance == this)
            LoadGameData();

    }

    [ContextMenu("Load Game Data")]
    public void LoadGameData()
    {
        ingredientDatas = ResourceLoader
            .GetAll<IngredientData>(ingredientDatasPath)
            .ToList();

        potionDatas = ResourceLoader
            .GetAll<PotionData>(potionDatasPath)
            .ToList();

        questDatas = ResourceLoader
            .GetAll<QuestData>(questDatasPath)
            .ToList();
        tutorialDatas = ResourceLoader
            .GetAll<TutorialData>(tutorialDatasPath)
            .ToList();
        
        InventoryManager.Instance.Load();
        PlayerDataManager.Instance.Load();
        QuestRuntimeManager.Instance.Load();
        HerbManager.Instance.Load();
        IsGameLoaded = true;
        OnGameLoaded?.Invoke();
    }
    [ContextMenu("Save Game Data")]
    public void SaveGameData()
    {
        InventoryManager.Instance.Save();
        PlayerDataManager.Instance.Save();
        QuestRuntimeManager.Instance.Save();
        HerbManager.Instance.Save();
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