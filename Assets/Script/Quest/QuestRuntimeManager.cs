using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Unity.VisualScripting;
[Serializable]
public class PlayerQuestData
{
    public QuestData questData;
    [ReadOnly] public string questId;
    public bool isCompleted;
    public bool isOnGoing;
    public bool isUnlocked;

    public void OnBeforeSerialize()
    {
        if (questData != null)
            questId = questData.questId;
    }
    public void OnAfterDeserialize()
    {
        if (!string.IsNullOrEmpty(questId))
        {
            questData = DataManager.Instance.questDatas
                .Find(data => data.questId == questId);
        }
    }
}

[Serializable]
[Metadata("PlayerQuests")]
public class PlayerQuestList
{
    public List<PlayerQuestData> playerQuests;
}
public class QuestRuntimeManager : Singleton<QuestRuntimeManager>
{
    private const string PlayerQuestDataFileName = "PlayerQuestData";
    [SerializeField] private PlayerQuestList playerQuestsList;
    public PlayerQuestList PlayerQuestsList
    {
        get { return playerQuestsList; }
        private set
        {
            playerQuestsList = value;
            OnQuestListUpdated?.Invoke();
        }
    }
    [SerializeField] private PlayerQuestData trackedQuest;
    public PlayerQuestData TrackedQuest
    {
        get { return trackedQuest; }
        private set
        {
            trackedQuest = value;
            OnTrackedQuestChanged?.Invoke(trackedQuest);
        }
    }
    public event Action OnQuestListUpdated;
    public event Action<PlayerQuestData> OnTrackedQuestChanged;
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
    private void Start()
    {
        UnlockAvailableQuests();
    }
    private void OnEnable()
    {
        DayManager.Instance.OnDayChanged += (day) => UnlockAvailableQuests();
    }

    private void OnDisable()
    {
        DayManager.Instance.OnDayChanged -= (day) => UnlockAvailableQuests();
    }


    public void Save()
    {
        playerQuestsList.playerQuests.ForEach(r => r.OnBeforeSerialize());
        DataManager.Instance.SaveToFile(PlayerQuestDataFileName, playerQuestsList);
    }
    public void Load()
    {
        playerQuestsList = DataManager.Instance.LoadFromFile(
            PlayerQuestDataFileName,
            () => new PlayerQuestList { playerQuests = new List<PlayerQuestData>() }
        );

        playerQuestsList.playerQuests.ForEach(r => r.OnAfterDeserialize());

        var existingIds = new HashSet<string>(
            playerQuestsList.playerQuests
                .Select(q => q.questId)
        );

        var newEntries = DataManager.Instance.questDatas
            .Where(data => !existingIds.Contains(data.questId))
            .Select(data => new PlayerQuestData
            {
                questData = data,
                isCompleted = false,
                isOnGoing = false,
                isUnlocked = false
            });

        playerQuestsList.playerQuests.AddRange(newEntries);
        OnQuestListUpdated?.Invoke();
    }
    

    private void UpdateQuestInfo(QuestData questData, bool isCompleted, bool isOnGoing, bool isUnlocked)
    {
        PlayerQuestData playerQuest = playerQuestsList.playerQuests
            .Find(q => q.questData.questId == questData.questId);

        UpdateQuestInfo(playerQuest, isCompleted, isOnGoing, isUnlocked);
    }
    private void UpdateQuestInfo(PlayerQuestData questData, bool isCompleted, bool isOnGoing, bool isUnlocked)
    {
        if (questData != null)
        {
            if (questData.isCompleted)
            {
            }
            else if (questData.isOnGoing && isCompleted)
            {
                questData.isCompleted = true;
                questData.isOnGoing = false;
                questData.isUnlocked = true;
            }
            else if (questData.isUnlocked && isOnGoing)
            {
                questData.isCompleted = false;
                questData.isOnGoing = true;
                questData.isUnlocked = true;
            }
            else if (isUnlocked)
            {
                questData.isCompleted = false;
                questData.isOnGoing = false;
                questData.isUnlocked = true;
            }        
        }
        OnQuestListUpdated?.Invoke();
    }

    private void UnlockAvailableQuests()
    {
        foreach (PlayerQuestData playerQuestData in playerQuestsList.playerQuests)
        {
            if (playerQuestData.questData.requirementsToUnlock.AreAllMet())
            {
                MarkQuestAsUnlocked(playerQuestData);
            }
        }
    }


#region public
    
    public List<PlayerQuestData> GetUnlockedQuests()
    {
        return playerQuestsList.playerQuests.FindAll(q => q.isUnlocked == true);
    }
    public List<PlayerQuestData> GetOngoingQuests()
    {
        return playerQuestsList.playerQuests.FindAll(q => q.isOnGoing == true);
    }
    public List<PlayerQuestData> GetCompletedQuests()
    {
        return playerQuestsList.playerQuests.FindAll(q => q.isCompleted == true);
    }
    
    public bool IsQuestCompleted(QuestData questData)
    {
        PlayerQuestData playerQuest = playerQuestsList.playerQuests
            .Find(q => q.questData.questId == questData.questId);

        return playerQuest != null && playerQuest.isCompleted;
    }

    public bool IsQuestOnGoing(QuestData questData)
    {
        PlayerQuestData playerQuest = playerQuestsList.playerQuests
            .Find(q => q.questData.questId == questData.questId);

        return playerQuest != null && playerQuest.isOnGoing;
    }

    public bool IsQuestUnlocked(QuestData questData)
    {
        PlayerQuestData playerQuest = playerQuestsList.playerQuests
            .Find(q => q.questData.questId == questData.questId);

        return playerQuest != null && playerQuest.isUnlocked;
    }
    public void MarkQuestAsCompleted(PlayerQuestData questData)
    {
        UpdateQuestInfo(questData, isCompleted: true, isOnGoing: false, isUnlocked: true);
    }

    public void MarkQuestAsOnGoing(PlayerQuestData questData)
    {
        UpdateQuestInfo(questData, isCompleted: false, isOnGoing: true, isUnlocked: true);
    }

    public void MarkQuestAsUnlocked(PlayerQuestData questData)
    {
        UpdateQuestInfo(questData, isCompleted: false, isOnGoing: false, isUnlocked: true);
    }

    public void MarkQuestAsCompleted(QuestData questData)
    {
        UpdateQuestInfo(questData, isCompleted: true, isOnGoing: false, isUnlocked: true);
    }

    public void MarkQuestAsOnGoing(QuestData questData)
    {
        UpdateQuestInfo(questData, isCompleted: false, isOnGoing: true, isUnlocked: true);
    }

    public void MarkQuestAsUnlocked(QuestData questData)
    {
        UpdateQuestInfo(questData, isCompleted: false, isOnGoing: false, isUnlocked: true);
    }

    public QuestData IdToQuestData(string questId)
    {
        return DataManager.Instance.questDatas.FirstOrDefault(q => q.questId == questId);
    }

#endregion

    
}
