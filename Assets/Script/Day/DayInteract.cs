using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayInteract : MonoBehaviour
{
    bool isSaving;
    public void Sleep()
    {
        StartCoroutine(SleepRoutine());
    }

    IEnumerator SleepRoutine()
    {
        if (isSaving) yield break;
        isSaving = true;
        InputManager.Instance.DisableInputs();
        UITransitionManager.Instance.FadeIn(SceneLoadingManager.Instance.loadingScreen, 1f);
        DayManager.Instance.SaveAndProceedNextDay();
        yield return new WaitForSeconds(2f);
        InputManager.Instance.EnableInputs();
        UITransitionManager.Instance.FadeOut(SceneLoadingManager.Instance.loadingScreen, 1f);
        isSaving = false;
    }

    public void News()
    {
        DayManager.Instance.ViewNewsAndAcceptNews();
    }
}
