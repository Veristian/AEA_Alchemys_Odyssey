using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
[Serializable]
public class Headline
{
    [TextArea(3, 10)]
    public string headlineText;
    [TextArea(3, 10)]
    public string headlineDescription;
    public Sprite headlineImage;

    public RequirementList requirementsToBeShown;
}
[Serializable]
public class LocalRequest
{
    // public string requestId;
    [TextArea(3, 10)]
    public string requestText;
    [TextArea(3, 10)]
    public string requestDescription;
    public Sprite requestImage;
    public QuestData questData;
    public string followUpNewsId;

    public RequirementList requirementsToBeShown;
}
[Serializable]
public class LocalNews
{
    public string newsId;
    [TextArea(3, 10)]
    public string newsText;
    [TextArea(3, 10)]
    public string newsDescription;
    public Sprite newsImage;
    // public RequirementList requirementsToBeShown;
    public bool willBeShown;
    public bool hasBeenShown;
    public bool canAlwaysShow = false;
    
}
public class DayManager : Singleton<DayManager>
{
    // [SerializeField, ReadOnly] private int day;
    public int Day
    {
        get => PlayerDataManager.Instance.day;
        private set
        {
            PlayerDataManager.Instance.day = value;
            OnDayChanged?.Invoke(PlayerDataManager.Instance.day);
        }
    }
    public event System.Action<int> OnDayChanged;

    public List<Headline> AllHeadlines;
    public List<LocalRequest> AllLocalRequests;
    public List<LocalNews> AllLocalNews;

    public List<Headline> ActiveHeadlines;
    public List<LocalRequest> ActiveLocalRequests;
    public List<LocalNews> ActiveLocalNews;

    public Headline currentHeadline;
    public List<LocalRequest> currentLocalRequest;
    public LocalNews currentLocalNews;

    public Headline previousHeadline;
    public List<LocalRequest> previousLocalRequest;
    public LocalNews previousLocalNews;
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
    public void SetDay(int newDay)
    {
        if (newDay < 0)
        {
            Debug.LogWarning("Day cannot be negative.");
            return;
        }
        Day = newDay;
        StartNewDay();
    }
    public void IncrementDay()
    {
        Day++;
        StartNewDay();
    }
    //method for debugging
    public void DecrementDay()
    {
        if (Day > 0)
            Day--;
    }

    private void PoolAndSetActiveElements()
    {
        ActiveHeadlines = AllHeadlines.FindAll(h => h.requirementsToBeShown.AreAllMet());
        ActiveLocalRequests = AllLocalRequests.FindAll(r => r.requirementsToBeShown.AreAllMet() );
        ActiveLocalNews = AllLocalNews.FindAll(n => n.willBeShown);
    }


    private void StartNewDay()
    {

        PoolAndSetActiveElements();

        previousHeadline = currentHeadline;
        previousLocalRequest = currentLocalRequest;
        previousLocalNews = currentLocalNews;

        currentHeadline = ActiveHeadlines.Count > 0 ? ActiveHeadlines[UnityEngine.Random.Range(0, ActiveHeadlines.Count)] : null;
        var mainQuests = ActiveLocalRequests
            .Where(x => x.questData.isMainQuest && !x.questData.hideInNews)
            .OrderBy(x => UnityEngine.Random.value)
            .ToList();

        var sideQuests = ActiveLocalRequests
            .Where(x => !x.questData.isMainQuest && !x.questData.hideInNews)
            .OrderBy(x => UnityEngine.Random.value)
            .ToList();

        currentLocalRequest = mainQuests
            .Take(3)
            .Concat(sideQuests.Take(Mathf.Max(0, 3 - mainQuests.Count)))
            .ToList();
        currentLocalNews = ActiveLocalNews.Count > 0 ? ActiveLocalNews[UnityEngine.Random.Range(0, ActiveLocalNews.Count)] : null;

        var oneTimeNews = ActiveLocalNews
        .Where(x => !x.canAlwaysShow)
        .ToList();

        if (oneTimeNews.Count > 0)
        {
            currentLocalNews = oneTimeNews[UnityEngine.Random.Range(0, oneTimeNews.Count)];

            // Remove it so it can't be selected again
            ActiveLocalNews.Remove(currentLocalNews);
        }
        else
        {
            currentLocalNews = ActiveLocalNews.Count > 0
                ? ActiveLocalNews[UnityEngine.Random.Range(0, ActiveLocalNews.Count)]
                : null;
        }

        //note to self: additionally give player no main quest quest if there is no main quest available and give new herbs on day 2 and 3
        //submit in bed and forest or news
        if (mainQuests.Count == 0 && sideQuests.Count > 0)
        {
            QuestRuntimeManager.Instance.GiveQuest("finish_bed_nomainrequest");
        }

        if (Day == 2)
        {
            QuestRuntimeManager.Instance.GiveQuest("start_forest_newherbs");
        }
        else if (Day == 3)
        {
            QuestRuntimeManager.Instance.GiveQuest("start_forest_newherbs");
        }
    }

    public void SetShowLocalNews(QuestData questData, bool ignoreHasBeenShown)
    {
        if (questData == null) return;
        //match to local request    
        var localRequest = AllLocalRequests.Find(n => n.questData == questData);
        //get local news from the followup id
        if (localRequest == null || localRequest.followUpNewsId == null || localRequest.followUpNewsId == "") return;
        var localNews = AllLocalNews.Find(n => n.newsId == localRequest.followUpNewsId);
        //set local news shown to true
        if (localNews == null) return;
        if (localNews.hasBeenShown && !ignoreHasBeenShown) return;
        localNews.willBeShown = true;
        localNews.hasBeenShown = false;
    }

    public void ViewNewsAndAcceptNews()
    {
        if (currentLocalNews == null || currentHeadline == null) return;
        if (!currentLocalNews.canAlwaysShow)
        {
            currentLocalNews.willBeShown = false;
            currentLocalNews.hasBeenShown = true;
        }

        //open UI
        PlayerPopUpUiManager.Instance.ForceOpenDailyTidals();
        UiLoader.Instance.CallOpenNewsEvent();
        //accept quests
        foreach (var request in currentLocalRequest)
        {
            QuestRuntimeManager.Instance.MarkQuestAsOnGoing(request.questData);
            if (request.questData.questId == "quest_sven_luckpotion")
            {
                FirstTimeTrigger.TryActivate("t9");
            }
        }
    }

    public void SaveAndProceedNextDay()
    {
        IncrementDay();
        DataManager.Instance.SaveGameData();
    }

    
}
