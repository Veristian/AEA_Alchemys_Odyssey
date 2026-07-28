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

    public event Action OnShopOpen;
    public event Action OnNewsOpen;

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
    [Header("Day")]
    [SerializeField] private TextMeshProUGUI dayText;

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
    public PlayerQuestData displayedQuestData;

    [Header("Shop")]
    [Header("Shop/Load")]
    [SerializeField] private Transform shopItemContainer;
    [SerializeField] private GameObject shopItemPrefab;
    [SerializeField] private TextMeshProUGUI shopCoinDisplay;
    [Header("Shop/Display")]
    [SerializeField] private TextMeshProUGUI shopItemName;
    [SerializeField] private TextMeshProUGUI shopItemDescription;
    [SerializeField] private TextMeshProUGUI shopItemPrice;
    [SerializeField] private Image shopItemImage;

    private ShopItemData displayedShopItemData;
    [Header("News")]
    [SerializeField] private GameObject NewsGameObject;
    [Header("News/Display")]
    [SerializeField] private TextMeshProUGUI newsHeadlineText;
    [SerializeField] private TextMeshProUGUI newsHeadlineDescription;
    [SerializeField] private Image newsHeadlineImage;
    [SerializeField] private TextMeshProUGUI localNewsText;
    [SerializeField] private TextMeshProUGUI localNewsDescription;
    [SerializeField] private Image localNewsImage;

    [Header("News/Load")]
    [SerializeField] private Transform newsRequestContainer;
    [SerializeField] private GameObject newsRequestPrefab;


#region Subscription

    private void Start()
    {
        trackedQuestPanel?.SetActive(false);
        DisplayDay(DayManager.Instance.Day);
        LoadShopData();
        LoadQuestData();
    }
    private void OnEnable()
    {
        OnInventoryOpen += LoadInventoryData;
        OnIndexOpen += LoadIndexData;
        OnQuestOpen += LoadQuestData;
        OnShopOpen += LoadShopData;
        OnNewsOpen += LoadNewsData;
        if (DayManager.Instance) DayManager.Instance.OnDayChanged += DisplayDay;
    }
    private void OnDisable()
    {
        OnInventoryOpen -= LoadInventoryData;
        OnIndexOpen -= LoadIndexData;
        OnQuestOpen -= LoadQuestData;
        OnShopOpen -= LoadShopData;
        OnNewsOpen -= LoadNewsData;
        if (DayManager.Instance) DayManager.Instance.OnDayChanged -= DisplayDay;
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
    public void DisplayDay(int day)
    {
        if (dayText) dayText.text = "Day "+ day.ToString();
    }

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

        if (QuestRuntimeManager.Instance.TrackedQuest != null && QuestRuntimeManager.Instance.TrackedQuest.questData != null)
        {
            DisplayQuestData(QuestRuntimeManager.Instance.TrackedQuest);
            TrackQuest();
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
        if (displayedQuestData.questData != null)
        {
            QuestRuntimeManager.Instance.SetTrackedQuest(displayedQuestData);
            trackedQuestPanel.SetActive(true);
            trackedQuestName.text = displayedQuestData.questData.questName;
            trackedQuestDescription.text = displayedQuestData.questData.questDescription;
            if (ParticleGuide.Instance != null)
            {
                ParticleGuide.Instance.FindDestinationWithName(displayedQuestData.questData.submissionCharacter.ToString());
            }
        }
    }
    public void UnTrackQuest()
    {
        QuestRuntimeManager.Instance.SetTrackedQuest(null);
        trackedQuestPanel.SetActive(false);
        trackedQuestName.text = "";
        trackedQuestDescription.text = "";
        if (ParticleGuide.Instance != null)
        {
            ParticleGuide.Instance.HideGuide();
        }
    }

    private void DisplayEmptyQuest()
    {
        Instantiate(questEmptyPrefab, mainQuestContainer);
        Instantiate(questEmptyPrefab, subQuestContainer);
    }

    
#endregion

#region Shop
    
    public void CallOpenShopEvent()
    {
        OnShopOpen?.Invoke();
    }

    // PlayerDataManager will unlock the items once bought and the progression manager will enable the item in scene based on player data
    public void LoadShopData()
    {
        foreach (Transform child in shopItemContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (ShopItemData shopItemData in ShopManager.Instance.shopItems
             .Where(item => item != null)
             .OrderBy(item => item.IsUnlocked)   
             .ThenBy(item => item.price))        // cheapest first
        {
            GameObject shopItem = Instantiate(shopItemPrefab, shopItemContainer);
            ShopDisplay shopDisplay = shopItem.GetComponent<ShopDisplay>();
            shopDisplay.InitShopDisplay(shopItemData);
        }
        if (shopCoinDisplay != null)
        {
            shopCoinDisplay.text = PlayerDataManager.Instance.gold.ToString();
        }
        if (shopItemName)
            shopItemName.text = "";
        if (shopItemDescription)
            shopItemDescription.text = "";
        if (shopItemPrice)
            shopItemPrice.text = "";
        if (shopItemImage)
            shopItemImage.enabled = false;
        displayedShopItemData = null;

    }

    public void DisplayShopItemData(ShopItemData shopItemData)
    {
        displayedShopItemData = shopItemData;
        if (shopItemName)
            shopItemName.text = displayedShopItemData.itemName;
        if (shopItemDescription)
            shopItemDescription.text = displayedShopItemData.description;
        if (shopItemPrice)
            shopItemPrice.text = displayedShopItemData.price.ToString();
        if (shopItemImage)
        {
            shopItemImage.enabled = true;
            shopItemImage.sprite = displayedShopItemData.itemSprite;
        }
    }

    public void BuyShopItem()
    {
        if (displayedShopItemData != null)
        {
            if (PlayerDataManager.Instance.SubtractGold(displayedShopItemData.price))
            {
                PlayerDataManager.Instance.SetUnlock(displayedShopItemData.itemGroupId, true);
                LoadShopData(); // Refresh the shop display after purchase
                AudioController.Instance.PlaySFX("Coin");
            }
            else
            {
                Debug.Log("Not enough gold to buy this item.");
            }
        }
    }

#endregion

#region News
    public void CallOpenNewsEvent()
    {
        OnNewsOpen?.Invoke();
    }

    private void LoadNewsData()
    {
        newsHeadlineText.text = DayManager.Instance.currentHeadline.headlineText;
        newsHeadlineDescription.text = DayManager.Instance.currentHeadline.headlineDescription;
        newsHeadlineImage.sprite = DayManager.Instance.currentHeadline.headlineImage;

        localNewsText.text = DayManager.Instance.currentLocalNews.newsText;
        localNewsDescription.text = DayManager.Instance.currentLocalNews.newsDescription;
        localNewsImage.sprite = DayManager.Instance.currentLocalNews.newsImage;


        foreach (Transform child in newsRequestContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (LocalRequest localRequest in DayManager.Instance.currentLocalRequest)
        {
            GameObject newsRequestItem = Instantiate(newsRequestPrefab, newsRequestContainer);
            DailyTaskDisplay dailyTaskDisplay = newsRequestItem.GetComponent<DailyTaskDisplay>();
            dailyTaskDisplay.InitDisplay(localRequest);
        }
    }

    // public void ActivateNews(bool active)
    // {
    //     NewsGameObject.SetActive(active);
    //     if (active)
    //     {
    //         CallOpenNewsEvent();
    //     }
    // }
#endregion
}
