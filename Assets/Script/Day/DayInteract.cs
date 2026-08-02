using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayInteract : MonoBehaviour
{
    bool isSaving;
    public void Sleep()
    {
        if (QuestRuntimeManager.Instance.IsAnyMainQuestOngoing()) return;
        StartCoroutine(SleepRoutine());
    }

    IEnumerator SleepRoutine()
    {
        if (isSaving) yield break;
        QuestRuntimeManager.Instance.SubmitQuest("finish_bed_nomainrequest");
        QuestRuntimeManager.Instance.SubmitQuest("finish_bed_thatsall");
        isSaving = true;
        AudioController.Instance.PlaySFX("Sleep");
        InputManager.Instance.DisableInputs();
        UITransitionManager.Instance.FadeIn(SceneLoadingManager.Instance.loadingScreen, 1f);
        yield return new WaitForSeconds(2f);
        DayManager.Instance.SaveAndProceedNextDay();
        InputManager.Instance.EnableInputs();
        UITransitionManager.Instance.FadeOut(SceneLoadingManager.Instance.loadingScreen, 1f);
        isSaving = false;
    }

    public void News()
    {
        DayManager.Instance.ViewNewsAndAcceptNews();
        QuestRuntimeManager.Instance.SubmitQuest("finish_bed_nomainrequest");
    }
}
