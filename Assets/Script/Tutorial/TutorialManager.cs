using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    private TutorialData _tutorialData;
    public TutorialData tutorialData
    {
        get => _tutorialData;
        set
        {
            _tutorialData = value;
            totalSlides = _tutorialData.GetSlideAmount();
            currentIndex = 0;
        }
    }
    private int totalSlides;
    private int currentIndex;
    public bool HasNext => currentIndex < totalSlides - 1;
    public bool HasPrevious => currentIndex > 0;

    public int CurrentIndex => currentIndex;
    public int TotalSlides => totalSlides;

    //next
    public (int, TutorialSlide) Next()
    {
        if (currentIndex < totalSlides - 1)
            currentIndex++;

        return (currentIndex, tutorialData.tutorialSlides[currentIndex]);
    }    
    //prev
    public (int, TutorialSlide) Previous()
    {
        if (currentIndex > 0)
            currentIndex--;

        return (currentIndex, tutorialData.tutorialSlides[currentIndex]);
    }
    //set new
    public (int,TutorialSlide) SetTutorial(TutorialData tutorialData)
    {
        this.tutorialData =  tutorialData;
        return (currentIndex, tutorialData.tutorialSlides[currentIndex]);
    }
    public (int,TutorialSlide) SetTutorial(string tutorialId)
    {
        var tutorialData = DataManager.Instance.tutorialDatas.Find(t => t.tutorialId == tutorialId);
        if (tutorialData == null) return (0, null);
        this.tutorialData =  tutorialData;
        return (currentIndex, tutorialData.tutorialSlides[currentIndex]);
    }
    


}
