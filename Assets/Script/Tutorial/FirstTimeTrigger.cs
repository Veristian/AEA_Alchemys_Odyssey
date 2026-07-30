using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class FirstTimeTrigger : Singleton<FirstTimeTrigger>
{
    [System.Serializable]
    public class TriggerEvent
    {
        public string id;
        public UnityEvent onFirstTime;
    }

    [SerializeField] private List<TriggerEvent> triggers = new List<TriggerEvent>();

    /// <summary>
    /// Activates an event only the first time the given ID is triggered.
    /// </summary>
    public static bool TryActivate(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("FirstTimeTrigger: ID is empty.");
            return false;
        }

        // Already activated
        if (PlayerPrefs.GetInt(id, 0) == 1)
        {
            return false;
        }

        // Mark as activated
        PlayerPrefs.SetInt(id, 1);
        PlayerPrefs.Save();

        // Find and invoke the corresponding event
        if (Instance != null)
        {
            foreach (TriggerEvent trigger in Instance.triggers)
            {
                if (trigger.id == id)
                {
                    trigger.onFirstTime?.Invoke();
                    break;
                }
            }
        }

        return true;
    }

    public static void CallTryActivate(string id)
    {
        TryActivate(id);
    }

    /// <summary>
    /// Checks whether a trigger has already been activated.
    /// </summary>
    public static bool HasActivated(string id)
    {
        return PlayerPrefs.GetInt(id, 0) == 1;
    }

    /// <summary>
    /// Resets a specific trigger.
    /// </summary>
    public static void Reset(string id)
    {
        PlayerPrefs.DeleteKey(id);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Resets all first-time triggers.
    /// </summary>
    public static void ResetAll()
    {
        foreach (TriggerEvent trigger in Instance.triggers)
        {
            PlayerPrefs.DeleteKey(trigger.id);
        }

        PlayerPrefs.Save();
    }
    public void ResetAllButton()
    {
        ResetAll();
    }
}