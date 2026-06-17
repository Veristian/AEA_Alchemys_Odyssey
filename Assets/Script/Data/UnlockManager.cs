using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GroupObjectData
{
    public string unlockableObjectsId;
    public List<GameObject> unlockableObjects;
    public void UnlockObjects()
    {
        foreach (GameObject GO in unlockableObjects)
        {
            GO.SetActive(true);
        }
    }
}
public class UnlockManager : Singleton<UnlockManager>
{
    public List<GroupObjectData> gameObjectUnlockables;

    public void UnlockObject(string id)
    {
        gameObjectUnlockables.Find(g => g.unlockableObjectsId == id)?.UnlockObjects();
    }

    private void OnEnable()
    {
        if (DataManager.Instance.IsGameLoaded)
        {
            PlayerDataManager.Instance.InitializeUnlockedObjects();
        }
    }
}
