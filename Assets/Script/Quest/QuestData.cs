using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public abstract class Requirements
{
    public abstract bool IsMet();
}
public class DaysRequirement : Requirements
{
    public int minimumDaysPassed;
    public override bool IsMet()
    {
        return DayManager.Instance.day >= minimumDaysPassed;
    }
}
public class QuestCompletionRequirement : Requirements
{
    public string requiredQuestId;
    public override bool IsMet()
    {
        return QuestRuntimeManager.Instance.IsQuestCompleted(QuestRuntimeManager.Instance.IdToQuestData(requiredQuestId));
    }
}
public class QuestPotionsRequirement : Requirements
{
    public PotionData requiredPotion;
    public List<StoredData> requiredIngredient;
    public override bool IsMet()
    {
        return InventoryManager.Instance.CheckPotionExists(requiredPotion, requiredIngredient, ignoreIngredientList: requiredIngredient == null || requiredIngredient.Count == 0);
    }
}

public class QuestRewards
{
    public int goldReward;
}


[Serializable]
public enum SubmissionCharacter
{
    None,
    Stevie,
    Mayor
}


[Serializable]
[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Create New Quest")]

public class QuestData : ResourceData
{
    [Header("Quest")]
    public string quest_name;
    public string quest_description;

    public SubmissionCharacter submissionCharacter;
    public List<Requirements> requirementsToUnlock;
    public List<Requirements> requirementsToComplete;
    public QuestRewards questRewards;

    public bool CheckUnlockConditions()
    {
        return requirementsToUnlock.All(req => req.IsMet());
    }

    public bool CheckCompletionConditions()
    {
        return requirementsToComplete.All(req => req.IsMet());
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

    public void MarkAsCompleted()
    {
        QuestRuntimeManager.Instance.MarkQuestAsCompleted(this);
    }

    public void MarkAsOnGoing()
    {
        QuestRuntimeManager.Instance.MarkQuestAsOnGoing(this);
    }

    public void MarkAsUnlocked()
    {
        QuestRuntimeManager.Instance.MarkQuestAsUnlocked(this);
    }

    public string GetFormattedDescription()
    {
        string formattedDescription = quest_description;

        foreach (var requirement in requirementsToUnlock)
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
                    ? requiredQuestData.quest_name
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
