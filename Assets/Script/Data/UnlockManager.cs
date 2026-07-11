using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GroupObjectData
{
    public string unlockableObjectsId;
    public List<GameObject> unlockableObjects;
    public bool canLockAgain;
    public void UnlockObjects()
    {
        //if can lock again, check against req to see if will unlock and update unlock data accordingly
        if (canLockAgain)
        {
            var dat = ProgressionManager.Instance.progressions.Find(p => p.unlockId == unlockableObjectsId);
            if (dat == null)
            {
                return;
            }
            if (!dat.requirementList.AreAllMet())
            {
                PlayerDataManager.Instance.SetUnlock(unlockableObjectsId, false);
                return;
            }
        }
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
