using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportTrigger : MonoBehaviour
{
    public InputManager inputManager;

    [Header("Scene Transition Settings")]
    [Tooltip("Name of the scene to load")]
    public string targetSceneName;

    [Tooltip("Name of the Empty GameObject in the target scene to spawn at")]
    public string spawnPointName = "PlayerSpawnPoint";

    private bool playerInside = false;

    private void Awake()
    {
        if (inputManager == null)
            inputManager = InputManager.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    private void Update()
    {
        if (!inputManager.canTakeInputs) return;
        if (playerInside && inputManager.InteractWasPressed)
        {
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                PlayerSpawnManager.Instance.SetSpawnInfo(spawnPointName);
                SceneManager.LoadScene(targetSceneName);
            }
            else
            {
                Debug.LogWarning("Target scene name is not set");
            }
        }
    }
}