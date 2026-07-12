using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonHoverInstaller : MonoBehaviour
{
    private void Awake()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            if (button.GetComponent<ButtonHoverScale>() == null)
            {
                button.gameObject.AddComponent<ButtonHoverScale>();
            }
        }
    }
}
