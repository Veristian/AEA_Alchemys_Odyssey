using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private TutorialData _tutorialData;
    public TutorialData tutorialData
    {
        get {return _tutorialData;}
        set
        {
            _tutorialData = tutorialData;
            totalSlides = tutorialData.GetSlideAmount();
            currentIndex = 0;
        }
    }
    private int totalSlides;
    private int currentIndex;

    //next
    public (int,TutorialSlide) Next()
    {
        if (currentIndex <= tutorialData.GetSlideAmount())
            currentIndex++;
        return (currentIndex, tutorialData.tutorialSlides[currentIndex]);
    }
    //prev
    public (int,TutorialSlide) Previous()
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


}
