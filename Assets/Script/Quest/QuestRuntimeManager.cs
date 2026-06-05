using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
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
            questId = questData.quest_id;
    }
    public void OnAfterDeserialize()
    {
        if (!string.IsNullOrEmpty(questId))
        {
            questData = DataManager.Instance.questDatas
                .Find(data => data.quest_id == questId);
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
    public PlayerQuestList playerQuestsList;
    public PlayerQuestData trackedQuest;
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
            .Where(data => !existingIds.Contains(data.quest_id))
            .Select(data => new PlayerQuestData
            {
                questData = data,
                isCompleted = false,
                isOnGoing = false,
                isUnlocked = false
            });

        playerQuestsList.playerQuests.AddRange(newEntries);
    }
    

    private void UpdateQuestInfo(QuestData questData, bool isCompleted, bool isOnGoing, bool isUnlocked)
    {
        PlayerQuestData playerQuest = playerQuestsList.playerQuests
            .Find(q => q.questData.quest_id == questData.quest_id);

        if (playerQuest != null)
        {
            playerQuest.isCompleted = isCompleted;
            playerQuest.isOnGoing = isOnGoing;
            playerQuest.isUnlocked = isUnlocked;
        }
    }


#region public
    public bool IsQuestCompleted(QuestData questData)
    {
        PlayerQuestData playerQuest = playerQuestsList.playerQuests
            .Find(q => q.questData.quest_id == questData.quest_id);

        return playerQuest != null && playerQuest.isCompleted;
    }

    public bool IsQuestOnGoing(QuestData questData)
    {
        PlayerQuestData playerQuest = playerQuestsList.playerQuests
            .Find(q => q.questData.quest_id == questData.quest_id);

        return playerQuest != null && playerQuest.isOnGoing;
    }

    public bool IsQuestUnlocked(QuestData questData)
    {
        PlayerQuestData playerQuest = playerQuestsList.playerQuests
            .Find(q => q.questData.quest_id == questData.quest_id);

        return playerQuest != null && playerQuest.isUnlocked;
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
        return DataManager.Instance.questDatas.FirstOrDefault(q => q.quest_id == questId);
    }

#endregion

    
}
