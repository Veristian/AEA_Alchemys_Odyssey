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
        currentLocalRequest = ActiveLocalRequests
            .OrderBy(x => UnityEngine.Random.value)
            .Take(Mathf.Min(3, ActiveLocalRequests.Count))
            .ToList();
        currentLocalNews = ActiveLocalNews.Count > 0 ? ActiveLocalNews[UnityEngine.Random.Range(0, ActiveLocalNews.Count)] : null;


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
        currentLocalNews.willBeShown = false;
        currentLocalNews.hasBeenShown = true;

        //open UI
        PlayerPopUpUiManager.Instance.OpenDailyTidals();
        UiLoader.Instance.CallOpenNewsEvent();
        //accept quests
        foreach (var request in currentLocalRequest)
        {
            QuestRuntimeManager.Instance.MarkQuestAsOnGoing(request.questData);
        }
    }

    
}
