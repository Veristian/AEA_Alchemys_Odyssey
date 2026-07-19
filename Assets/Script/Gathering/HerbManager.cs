using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[Serializable]
public class HerbStateData
{
    public string herbID;
    public bool isTaken;
    public int dayTaken;
}

[Serializable]
[Metadata("HerbSaveData")]
public class HerbSaveData
{
    public List<HerbStateData> herbs = new();
}

public class HerbManager : Singleton<HerbManager>
{

    private Dictionary<string, HerbStateData> herbStates = new();

    private const string herbStateFileName = "PlayerHerbStateData";

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

    // private void Start()
    // {
    //     Load();
    // }

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloading;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloading;
    }

    public HerbStateData GetState(string herbID)
    {
        if (!herbStates.TryGetValue(herbID, out HerbStateData state))
        {
            state = new HerbStateData
            {
                herbID = herbID,
                isTaken = false,
                dayTaken = 0
            };

            herbStates.Add(herbID, state);
        }

        return state;
    }

    public void SetState(string herbID, bool taken, int dayTaken)
    {
        HerbStateData state = GetState(herbID);
        state.isTaken = taken;
        state.dayTaken = dayTaken;
    }

    public HerbSaveData GetSaveData()
    {
        HerbSaveData save = new();

        foreach (var state in herbStates.Values)
            save.herbs.Add(state);

        return save;
    }

    public void LoadSaveData(HerbSaveData save)
    {
        herbStates.Clear();

        if (save == null)
            return;

        foreach (var herb in save.herbs)
        {
            herbStates[herb.herbID] = herb;
        }
    }

    private void OnSceneUnloading(Scene scene)
    {
        // Save();
    }

    public void Save()
    {
        HerbSaveData data = new();

        foreach (var herb in herbStates.Values)
            data.herbs.Add(herb);

        DataManager.Instance.SaveToFile(herbStateFileName, data);
    }

    public void Load()
    {
        herbStates.Clear();

        HerbSaveData data = DataManager.Instance.LoadFromFile(
            herbStateFileName,
            () => new HerbSaveData());

        foreach (var herb in data.herbs)
            herbStates[herb.herbID] = herb;
    }


}