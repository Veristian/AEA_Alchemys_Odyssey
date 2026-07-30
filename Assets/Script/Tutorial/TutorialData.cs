using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[Serializable]
public class TutorialSlide
{
    public string tutorialTitle;
    public VideoClip tutorialVideo;
    [TextArea]
    public string tutorialDescription;
}
[CreateAssetMenu(fileName = "New Tutorial", menuName = "Tutorial/Create New Tutorial")]
public class TutorialData : ResourceData
{
    [Header("Tutorial")]
    public string tutorialId;
    public List<TutorialSlide> tutorialSlides;

    public int GetSlideAmount()
    {
        return tutorialSlides.Count;
    }
}
