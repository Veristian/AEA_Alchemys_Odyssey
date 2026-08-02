using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Ink.Runtime;
[Serializable]
public class Requirements
{
    public virtual bool IsMet()
    {
        return true;
    }
    public virtual bool Submit()
    {
        return true;
    }

}
[Serializable]
public class DaysRequirement : Requirements
{
    public int minimumDaysPassed;
    public override bool IsMet()
    {
        return DayManager.Instance.Day >= minimumDaysPassed;
    }
    public override bool Submit()
    {
        return DayManager.Instance.Day >= minimumDaysPassed;
    }
}
[Serializable]
public class MaxDaysRequirement : Requirements
{
    public int maximumDaysPassed;
    public override bool IsMet()
    {
        return DayManager.Instance.Day <= maximumDaysPassed;
    }
    public override bool Submit()
    {
        return DayManager.Instance.Day <= maximumDaysPassed;
    }
}
[Serializable]
public class ExactDaysRequirement : Requirements
{
    public int currentDay;
    public override bool IsMet()
    {
        return DayManager.Instance.Day == currentDay;
    }
    public override bool Submit()
    {
        return DayManager.Instance.Day == currentDay;
    }
}
[Serializable]
public class QuestCompletionRequirement : Requirements
{
    public string requiredQuestId;
    public bool isCompleted = true;
    public override bool IsMet()
    {
        return QuestRuntimeManager.Instance.IsQuestCompleted(QuestRuntimeManager.Instance.IdToQuestData(requiredQuestId)) == isCompleted;
    }
    public override bool Submit()
    {
        return QuestRuntimeManager.Instance.IsQuestCompleted(QuestRuntimeManager.Instance.IdToQuestData(requiredQuestId)) == isCompleted;
    }
}
[Serializable]
public class QuestOngoingRequirement : Requirements
{
    public string requiredQuestId;
    public bool isOngoing = false;
    public override bool IsMet()
    {
        return QuestRuntimeManager.Instance.IsQuestOnGoing(QuestRuntimeManager.Instance.IdToQuestData(requiredQuestId)) == isOngoing;
    }
    public override bool Submit()
    {
        return QuestRuntimeManager.Instance.IsQuestOnGoing(QuestRuntimeManager.Instance.IdToQuestData(requiredQuestId)) == isOngoing;
    }
}
[Serializable]
public class QuestUnlockedRequirement : Requirements
{
    public string unlockedQuestId;
    public bool isUnlocked = true;
    public override bool IsMet()
    {
        return QuestRuntimeManager.Instance.IsQuestUnlocked(QuestRuntimeManager.Instance.IdToQuestData(unlockedQuestId)) == isUnlocked;
    }
    public override bool Submit()
    {
        return QuestRuntimeManager.Instance.IsQuestUnlocked(QuestRuntimeManager.Instance.IdToQuestData(unlockedQuestId)) == isUnlocked;
    }
}
[Serializable]
public class QuestPotionsRequirement : Requirements
{
    public PotionData requiredPotion;
    public List<StoredData> requiredIngredient;
    public override bool IsMet()
    {
        return InventoryManager.Instance.CheckPotionExists(requiredPotion, requiredIngredient, ignoreIngredientList: requiredIngredient == null || requiredIngredient.Count == 0).exist;
    }
    public override bool Submit()
    {
        return InventoryManager.Instance.CheckPotionExistsAndRemove(requiredPotion, requiredIngredient, ignoreIngredientList: requiredIngredient == null || requiredIngredient.Count == 0);
    }
}

[Serializable]
public class QuestAction
{
    public int goldReward;
}


[Serializable]
public enum SubmissionCharacter
{
    None,
    Ramonts,
    Violette,
    Kenneth,
    Sven,
    Poshe,
    Circe,
    Tidus,
    Chemy,
    Forest,
    Bed

}
[Serializable]
public class RequirementList
{
    [SerializeReference]
    public List<Requirements> requirements;

    public bool AreAllMet()
    {
        if (requirements == null) return true;

        return requirements.All(req => req.IsMet());
    }
    public bool SubmitAll()
    {
        return requirements.All(req => req.Submit());
    }
    [ContextMenu("Add Minimum Days Requirement")]
    public void AddDaysRequirement()
    {
        requirements.Add(new DaysRequirement { minimumDaysPassed = 1});
    }
    [ContextMenu("Add Quest Completion Requirement")]
    public void AddQuestCompletionRequirement()
    {
        requirements.Add(new QuestCompletionRequirement { requiredQuestId = null});
    }
    [ContextMenu("Add Quest Potions Requirement")]
    public void AddQuestPotionsRequirement()
    {
        requirements.Add(new QuestPotionsRequirement { requiredPotion = null, requiredIngredient = null});
    }
    [ContextMenu("Add Maximum Days Requirement")]
    public void AddMaxDaysRequirement()
    {
        requirements.Add(new MaxDaysRequirement { maximumDaysPassed = 1});
    }
    [ContextMenu("Add Exact Days Requirement")]
    public void AddExactDaysRequirement()
    {
        requirements.Add(new ExactDaysRequirement { currentDay = 1});
    }

}

[Serializable]
[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Create New Quest")]

public class QuestData : ResourceData
{
    public string questName;
    [TextArea]
    public string questDescription;
    [TextArea(20, 20)]
    public string questFullDescription;

    public SubmissionCharacter submissionCharacter;
    public RequirementList requirementsToUnlock;
    public RequirementList requirementsToComplete;
    public QuestAction questActions;
    public bool isMainQuest;
    public bool isRepeatable;
    public bool hideInNews;
    [ReadOnly] public TextAsset storyText;
    public Story story { get; private set; }
    private void OnEnable()
    {
        GetStory();
    }
    private void OnValidate()
    {
        GetStory();
    }
    [ContextMenu("Get Story")]
    public void GetStory()
    {
        if (string.IsNullOrEmpty(questId))
        {
            Debug.LogWarning("Quest ID is null or empty for quest: " + questName + ". Please assign a valid quest ID.");
            return;
        }

        storyText = DialogueManager.GetInkJSON(questId);
        if (storyText != null)
        {
            story = new Story(storyText.text);
        }
        else
        {
            Debug.LogWarning("Failed to load story for quest: " + questId);
        }
    }

    public bool CheckUnlockConditions()
    {
        return requirementsToUnlock.AreAllMet();
    }

    public bool CheckCompletionConditions()
    {
        return requirementsToComplete.AreAllMet();
    }

    public bool IsCompleted()
    {
        return QuestRuntimeManager.Instance.IsQuestCompleted(this);
    }

    public bool IsUnlocked()
    {
        return QuestRuntimeManager.Instance.IsQuestUnlocked(this);
    }

    public bool IsOnGoing()
    {
        return QuestRuntimeManager.Instance.IsQuestOnGoing(this);
    }

    // public void MarkAsCompleted()
    // {
    //     QuestRuntimeManager.Instance.MarkQuestAsCompleted(this);
    // }

    // public void MarkAsOnGoing()
    // {
    //     QuestRuntimeManager.Instance.MarkQuestAsOnGoing(this);
    // }

    // public void MarkAsUnlocked()
    // {
    //     QuestRuntimeManager.Instance.MarkQuestAsUnlocked(this);
    // }

    public string GetFormattedDescription()
    {
        string formattedDescription = questDescription;

        foreach (var requirement in requirementsToUnlock.requirements)
        {
            if (requirement is DaysRequirement daysReq)
            {
                formattedDescription = formattedDescription.Replace(
                    $"{{{nameof(DaysRequirement)}}}",
                    $"Pass {daysReq.minimumDaysPassed} days"
                );
            }
            else if (requirement is QuestCompletionRequirement questReq)
            {
                var requiredQuestData = QuestRuntimeManager.Instance.IdToQuestData(questReq.requiredQuestId);

                string requiredQuestName = requiredQuestData != null
                    ? requiredQuestData.questName
                    : "Unknown Quest";

                formattedDescription = formattedDescription.Replace(
                    $"{{{nameof(QuestCompletionRequirement)}}}",
                    $"Complete '{requiredQuestName}'"
                );
            }
            else if (requirement is QuestPotionsRequirement potionReq)
            {
                formattedDescription = formattedDescription.Replace(
                    $"{{{nameof(QuestPotionsRequirement)}}}",
                    $"Have {potionReq.requiredPotion.potionName}"
                );
            }
        }

        return formattedDescription;
    }

    // public bool CheckUnlockConditions()
    // {
    //     foreach (var requirement in requirementsToUnlock)
    //     {
    //         if (!CheckRequirement(requirement))
    //             return false;
    //     }
    //     return true;
    // }


    // public bool CheckCompletionConditions()
    // {
    //     foreach (var requirement in requirementsToComplete)
    //     {
    //         if (!CheckRequirement(requirement))
    //             return false;
    //     }
    //     return true;
    // }

    // private bool CheckRequirement(Requirements requirement)
    // {
    //     switch (requirement)
    //     {
    //         case DaysRequirement daysReq:
    //             return GameManager.Instance.DaysPassed >= daysReq.minimumDaysPassed;
    //         case QuestCompletionRequirement questReq:
    //             return QuestManager.Instance.IsQuestCompleted(questReq.requiredQuestId);
    //         case QuestPotionsRequirement potionReq:
    //             return InventoryManager.Instance.GetIngredientCount(potionReq.requiredItemId) >= potionReq.requiredItemCount;
    //         default:
    //             Debug.LogWarning("Unknown requirement type: " + requirement.GetType());
    //             return false;
    //     }
    // }
}
