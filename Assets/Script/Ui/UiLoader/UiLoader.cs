using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;
using UnityEngine.UI;
public class UiLoader : Singleton<UiLoader>
{
    public event Action OnInventoryOpen;

    public event Action OnIndexOpen;

    public event Action OnQuestOpen;

    [Header("Inventory")]

    [SerializeField] private Transform inventoryIngredientContainer;
    [SerializeField] private Transform inventoryPotionContainer;
    [SerializeField] private GameObject inventoryPrefab;

    [Header("Index")]
    [Header("Index/Load")]
    [SerializeField] private Transform indexIngredientContainer;
    [SerializeField] private Transform indexPotionContainer;
    [SerializeField] private GameObject indexPrefab;
    [Header("Index/Display")]
    [SerializeField] private TextMeshProUGUI indexName;
    [SerializeField] private TextMeshProUGUI indexDescription;
    [SerializeField] private Image indexImage;
    [Header("Quest")]
    [Header("Quest/Load")]
    [SerializeField] private Transform mainQuestContainer;
    [SerializeField] private Transform subQuestContainer;
    [SerializeField] private GameObject questPrefab;
    [SerializeField] private GameObject questEmptyPrefab;
    [Header("Quest/Display")]
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questDescription;
    [SerializeField] private TextMeshProUGUI goldRewardAmount;
    [SerializeField] private GameObject goldIcon;

    [Header("Quest/Tracked")]
    [SerializeField] private TextMeshProUGUI trackedQuestName;
    [SerializeField] private TextMeshProUGUI trackedQuestDescription;
    [SerializeField] private GameObject trackedQuestPanel;
    private PlayerQuestData displayedQuestData;



#region Subscription

    private void Start()
    {
        trackedQuestPanel?.SetActive(false);
    }
    private void OnEnable()
    {
        OnInventoryOpen += LoadInventoryData;
        OnIndexOpen += LoadIndexData;
        OnQuestOpen += LoadQuestData;
    }
    private void OnDisable()
    {
        OnInventoryOpen -= LoadInventoryData;
        OnIndexOpen -= LoadIndexData;
        OnQuestOpen -= LoadQuestData;
    }
#endregion
#region  Inventory
    public void CallOpenInventoryEvent()
    {
        OnInventoryOpen?.Invoke();
    }

    private void LoadInventoryData()
    {
        foreach (Transform child in inventoryIngredientContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (IngredientInventoryData ingredientInventoryData in InventoryManager.Instance.IngredientInventoryList.ingredientsList)
        {
            if (ingredientInventoryData == null)
            {
                Debug.LogWarning("IngredientInventoryData is null. Skipping this entry.");
                continue;
            }
            if (ingredientInventoryData.ingredientData == null)
            {
                Debug.LogWarning("IngredientData is null for IngredientInventoryData: " + ingredientInventoryData + ". Skipping this entry.");
                continue;
            }
            if (ingredientInventoryData.amount == 0)
            {
                continue;
            }
            GameObject ingredientItem = Instantiate(inventoryPrefab, inventoryIngredientContainer);
            InventoryDisplay inventoryDisplay = ingredientItem.GetComponent<InventoryDisplay>();
            inventoryDisplay.InitIngredientDisplay(ingredientInventoryData);
        } 


        foreach (Transform child in inventoryPotionContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (PotionInventoryData potionInventoryData in InventoryManager.Instance.PotionInventoryList.potionList)
        {
            if (potionInventoryData == null)
            {
                Debug.LogWarning("PotionInventoryData is null. Skipping this entry.");
                continue;
            }
            GameObject potionItem = Instantiate(inventoryPrefab, inventoryPotionContainer);
            InventoryDisplay inventoryDisplay = potionItem.GetComponent<InventoryDisplay>();
            inventoryDisplay.InitPotionDisplay(potionInventoryData);
        } 
        
    }
#endregion


#region Index
    public void CallOpenIndexEvent()
    {
        OnIndexOpen?.Invoke();
    }
    private void LoadIndexData()
    {
        foreach (Transform child in indexIngredientContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (IngredientData ingredientData in DataManager.Instance.ingredientDatas)
        {
            GameObject ingredientItem = Instantiate(indexPrefab, indexIngredientContainer);
            JournalDisplay journalDisplay = ingredientItem.GetComponent<JournalDisplay>();
            journalDisplay.InitIngredientDisplay(ingredientData);
        }

        foreach (Transform child in indexPotionContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (PotionData potionData in DataManager.Instance.potionDatas)
        {
            if (potionData.potionId == "Null_Potion")
            {
                continue;
            }
            GameObject potionItem = Instantiate(indexPrefab, indexPotionContainer);
            JournalDisplay journalDisplay = potionItem.GetComponent<JournalDisplay>();
            journalDisplay.InitPotionDisplay(potionData);
        }
            
    }

    public void DisplayIngredientData(IngredientData ingredientData)
    {
        indexName.text = ingredientData.ingredientName;
        indexDescription.text = ingredientData.description;
        indexImage.sprite = ingredientData.ingredientSprite;
        indexImage.preserveAspect = true;
    }

    public void DisplayPotionData(PotionData potionData)
    {
        indexName.text = potionData.potionName;
        indexDescription.text = potionData.description;
        indexImage.sprite = potionData.potionSprite;
        indexImage.preserveAspect = true;
    }
#endregion

#region Day

#endregion

#region Quest
    public void CallOpenQuestEvent()
    {
        OnQuestOpen?.Invoke();
    }

    private void LoadQuestData()
    {
        foreach (Transform child in mainQuestContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in subQuestContainer)
        {
            Destroy(child.gameObject);
        }

        questName.text = "";
        questDescription.text = "";
        goldIcon.SetActive(false);
        goldRewardAmount.text = "";
        bool hasQuest = false;

        foreach (PlayerQuestData questData in QuestRuntimeManager.Instance.PlayerQuestsList.playerQuests)
        {
            if (questData == null)
            {
                Debug.LogWarning("PlayerQuestData is null. Skipping this entry.");
                continue;
            }
            if (questData.questData.IsOnGoing() != true)
            {
                continue;
            }

            hasQuest = true;
            GameObject questItem = Instantiate(questPrefab, mainQuestContainer);
            QuestDisplay questDisplay = questItem.GetComponent<QuestDisplay>();

            if (questData.questData.isRepeatable)
            {
                questItem.transform.SetParent(subQuestContainer);
            }
            questDisplay.Initialize(questData);
        }
        if (!hasQuest)
        {
            Debug.Log("No ongoing quests.");
            DisplayEmptyQuest();
        }

    }

    public void DisplayQuestData(PlayerQuestData questData)
    {
        displayedQuestData = questData;
        questName.text = displayedQuestData.questData.questName;
        questDescription.text = displayedQuestData.questData.questDescription;
        goldIcon.SetActive(true);
        goldRewardAmount.text = displayedQuestData.questData.questActions.goldReward.ToString();
    }

    public void TrackQuest()
    {
        if (displayedQuestData != null)
        {
            QuestRuntimeManager.Instance.SetTrackedQuest(displayedQuestData);
            trackedQuestPanel.SetActive(true);
            trackedQuestName.text = displayedQuestData.questData.questName;
            trackedQuestDescription.text = displayedQuestData.questData.questDescription;
        }
    }

    private void DisplayEmptyQuest()
    {
        Instantiate(questEmptyPrefab, mainQuestContainer);
        Instantiate(questEmptyPrefab, subQuestContainer);
    }

    
#endregion
}
