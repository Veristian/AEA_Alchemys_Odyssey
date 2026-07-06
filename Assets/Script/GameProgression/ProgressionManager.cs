using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Progression
{
    public RequirementList requirementList;
    public string unlockId;
}
//used to enable herbs or furniture on certain days, or after certain quests are completed or bought.
public class ProgressionManager : Singleton<ProgressionManager>
{
    public List<Progression> progressions;

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

    void OnEnable()
    {
        if (QuestRuntimeManager.Instance != null)
            QuestRuntimeManager.Instance.OnQuestListUpdated += UpdateProgress;
        if (DayManager.Instance != null)
            DayManager.Instance.OnDayChanged += HandleDayChanged;
    }
    void OnDisable()
    {
        if (QuestRuntimeManager.Instance != null)
            QuestRuntimeManager.Instance.OnQuestListUpdated -= UpdateProgress;
        if (DayManager.Instance != null)
            DayManager.Instance.OnDayChanged -= HandleDayChanged;

    }
    private void HandleDayChanged(int day)
    {
        UpdateProgress();
    }
    public void UpdateProgress()
    {
        foreach (Progression progress in progressions)
        {
            if (progress.requirementList.AreAllMet())
            {
                PlayerDataManager.Instance.SetUnlock(progress.unlockId);
            }
        }
    }
}
