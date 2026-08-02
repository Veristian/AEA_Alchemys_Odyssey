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
        if (DayManager.Instance)
            DayManager.Instance.OnDayChanged += HandleDayChanged;
    }

    private void OnDisable()
    {
        if (DayManager.Instance)
            DayManager.Instance.OnDayChanged -= HandleDayChanged;
    }

    private void HandleDayChanged(int day)
    {
        UnlockAvailableQuests();
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
        playerQuestsList.playerQuests.RemoveAll(i => i.questData == null);

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
    

    private void UpdateQuestInfo(QuestData questData, bool isCompleted, bool isOnGoing, bool isUnlocked, bool repeatable)
    {
        PlayerQuestData playerQuest = playerQuestsList.playerQuests
            .Find(q => q.questData.questId == questData.questId);

        UpdateQuestInfo(playerQuest, isCompleted, isOnGoing, isUnlocked, repeatable);
    }
    private void UpdateQuestInfo(PlayerQuestData questData, bool isCompleted, bool isOnGoing, bool isUnlocked, bool repeatable)
    {
        if (questData != null)
        {
            if (questData.isCompleted && isUnlocked && repeatable)
            {
                questData.isCompleted = false;
                questData.isOnGoing = false;
                questData.isUnlocked = true;
                DayManager.Instance.SetShowLocalNews(questData.questData, true);
            }
            else if (questData.isOnGoing && isCompleted)
            {
                questData.isCompleted = true;
                questData.isOnGoing = false;
                questData.isUnlocked = true;
            }
            else if (questData.isUnlocked && isOnGoing && !questData.isCompleted)
            {
                questData.isCompleted = false;
                questData.isOnGoing = true;
                questData.isUnlocked = true;
            }
            else if (isUnlocked && !questData.isOnGoing && !questData.isCompleted)
            {
                questData.isCompleted = false;
                questData.isOnGoing = false;
                questData.isUnlocked = true;
            }       
            if (questData.isCompleted)
            {
                DayManager.Instance.SetShowLocalNews(questData.questData, false);
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
        UpdateQuestInfo(questData, isCompleted: true, isOnGoing: false, isUnlocked: true, questData.questData.isRepeatable);
    }

    public void MarkQuestAsOnGoing(PlayerQuestData questData)
    {
        UpdateQuestInfo(questData, isCompleted: false, isOnGoing: true, isUnlocked: true, questData.questData.isRepeatable);
    }

    public void MarkQuestAsUnlocked(PlayerQuestData questData)
    {
        UpdateQuestInfo(questData, isCompleted: false, isOnGoing: false, isUnlocked: true, questData.questData.isRepeatable);
    }

    public void MarkQuestAsCompleted(QuestData questData)
    {
        UpdateQuestInfo(questData, isCompleted: true, isOnGoing: false, isUnlocked: true, questData.isRepeatable);
    }

    public void MarkQuestAsOnGoing(QuestData questData)
    {
        UpdateQuestInfo(questData, isCompleted: false, isOnGoing: true, isUnlocked: true, questData.isRepeatable);
    }

    public void MarkQuestAsUnlocked(QuestData questData)
    {
        UpdateQuestInfo(questData, isCompleted: false, isOnGoing: false, isUnlocked: true, questData.isRepeatable);
    }

    public QuestData IdToQuestData(string questId)
    {
        return DataManager.Instance.questDatas.FirstOrDefault(q => q.questId == questId);
    }
    public PlayerQuestData IdToPlayerQuestData(string questId)
    {
        return playerQuestsList.playerQuests.FirstOrDefault(q => q.questData.questId == questId);
    }

    public void SetTrackedQuest(PlayerQuestData questData)
    {
        TrackedQuest = questData;
    }

#endregion

#region Quest Interaction
    public void SubmitQuest(PlayerQuestData playerQuestData)
    {
        //check completion
        if (!playerQuestData.isUnlocked || !playerQuestData.isOnGoing || playerQuestData.isCompleted) return;
        if (!playerQuestData.questData.requirementsToComplete.SubmitAll()) return;
        //update quest
        MarkQuestAsCompleted(playerQuestData);
        //reward player
        PlayerDataManager.Instance.AddGold(playerQuestData.questData.questActions.goldReward);

        if (UiLoader.Instance.displayedQuestData == playerQuestData) UiLoader.Instance.UnTrackQuest();
        
        if (PlayerPopUpUiManager.Instance != null)
        {
            PlayerPopUpUiManager.Instance.QuestResultSetup(playerQuestData.questData.questName, playerQuestData.questData.questActions.goldReward);
        }
        if (playerQuestData.questData.submissionCharacter != SubmissionCharacter.None)
            FirstTimeTrigger.TryActivate("t8");

        //note to self: show thats all when finished quest and have nothing left to do
    }
    public void SubmitQuest(string questId)
    {
        PlayerQuestData playerQuestData = IdToPlayerQuestData(questId);
        if (playerQuestData == null) return;
        //check completion
        if (!playerQuestData.isUnlocked || !playerQuestData.isOnGoing || playerQuestData.isCompleted) return;
        if (!playerQuestData.questData.requirementsToComplete.SubmitAll()) return;
        //update quest
        MarkQuestAsCompleted(playerQuestData);
        //reward player
        PlayerDataManager.Instance.AddGold(playerQuestData.questData.questActions.goldReward);
        if (UiLoader.Instance.displayedQuestData == playerQuestData) UiLoader.Instance.UnTrackQuest();

        if (PlayerPopUpUiManager.Instance != null && playerQuestData.questData.hideInNews == false)
        {
            PlayerPopUpUiManager.Instance.QuestResultSetup(playerQuestData.questData.questName, playerQuestData.questData.questActions.goldReward);
        }

        //show thats all when finished quest and have nothing left to do
        if (GetOngoingQuests().Count == 0)
        {
            GiveQuest("finish_bed_thatsall");
        }
        //submit in bed
    }
#endregion

    public void SubmitAllQuest()
    {
        foreach (PlayerQuestData questData in playerQuestsList.playerQuests)
        {
            SubmitQuest(questData);
        }
    }

    public void GiveQuest(string questId)
    {
        PlayerQuestData playerQuestData = IdToPlayerQuestData(questId);

        if (playerQuestData == null)
        {
            Debug.LogWarning($"Quest '{questId}' not found.");
            return;
        }

        if (playerQuestData.isCompleted && !playerQuestData.questData.isRepeatable)
            return;

        MarkQuestAsOnGoing(playerQuestData);
        UiLoader.Instance.displayedQuestData = playerQuestData;
        UiLoader.Instance.TrackQuest();
    }

    public bool IsAnyMainQuestOngoing()
    {
        return playerQuestsList.playerQuests.Any(q =>
            q.isOnGoing &&
            q.questData != null &&
            q.questData.isMainQuest
        );
    }
}
