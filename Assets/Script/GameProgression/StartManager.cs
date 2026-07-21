using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    bool started;
    public void OnPressStart()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (started)
        {
            SceneLoadingManager.Instance.LoadScene("ChemyHouse");
        }
        else
        {
            SceneLoadingManager.Instance.LoadScene("OpenWorld");
        }
    }

    public void SetStarted(bool hasStart)
    {
        started = hasStart;
    }

}
