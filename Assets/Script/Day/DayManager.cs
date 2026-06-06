using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Headline
{
    // public string headlineId;
    public GameObject headlinePrefab;

    public RequirementList requirementsToBeShown;
}
[Serializable]
public class LocalRequest
{
    // public string requestId;
    public GameObject requestPrefab;
    public QuestData questData;
    public string followUpNewsId;

    public RequirementList requirementsToBeShown;
}
[Serializable]
public class LocalNews
{
    public string newsId;
    public GameObject newsPrefab;
    public RequirementList requirementsToBeShown;
}
public class DayManager : Singleton<DayManager>
{
    [SerializeField, ReadOnly] private int day;
    public int Day
    {
        get => day;
        private set
        {
            day = value;
            OnDayChanged?.Invoke(day);
        }
    }
    public event System.Action<int> OnDayChanged;

    public List<Headline> AllHeadlines;
    public List<LocalRequest> AllLocalRequests;
    public List<LocalNews> AllLocalNews;

    [ReadOnly] public List<Headline> ActiveHeadlines;
    [ReadOnly] public List<LocalRequest> ActiveLocalRequests;
    [ReadOnly] public List<LocalNews> ActiveLocalNews;

    public Headline currentHeadline;
    public LocalRequest currentLocalRequest;
    public LocalNews currentLocalNews;

    [ReadOnly] public Headline previousHeadline;
    [ReadOnly] public LocalRequest previousLocalRequest;
    [ReadOnly] public LocalNews previousLocalNews;
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

    private void PoolActiveElements()
    {
        ActiveHeadlines = AllHeadlines.FindAll(h => h.requirementsToBeShown.AreAllMet());
        ActiveLocalRequests = AllLocalRequests.FindAll(r => r.requirementsToBeShown.AreAllMet());
        ActiveLocalNews = AllLocalNews.FindAll(n => n.requirementsToBeShown.AreAllMet());
    }

    private void StartNewDay()
    {
        PoolActiveElements();

        previousHeadline = currentHeadline;
        previousLocalRequest = currentLocalRequest;
        previousLocalNews = currentLocalNews;

        currentHeadline = ActiveHeadlines.Count > 0 ? ActiveHeadlines[UnityEngine.Random.Range(0, ActiveHeadlines.Count)] : null;
        currentLocalRequest = ActiveLocalRequests.Count > 0 ? ActiveLocalRequests[UnityEngine.Random.Range(0, ActiveLocalRequests.Count)] : null;
        currentLocalNews = ActiveLocalNews.Count > 0 ? ActiveLocalNews[UnityEngine.Random.Range(0, ActiveLocalNews.Count)] : null;
    }

    






    
}
