// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class TeleportTrigger : MonoBehaviour
// {
//     public InputManager inputManager;
//     public GameObject OverlayCanvas;

//     [Header("Scene Transition Settings")]
//     [Tooltip("Name of the scene to load")]
//     public string targetSceneName;

//     [Tooltip("Name of the Empty GameObject in the target scene to spawn at")]
//     public string spawnPointName = "PlayerSpawnPoint";

//     private bool playerInside = false;

//     private void Awake()
//     {
//         if (inputManager == null)
//             inputManager = InputManager.Instance;
//     }
//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             playerInside = true;
//         }
//         //OverlayCanvas.SetActive(true);
//         if (OverlayCanvas != null)
//         {
//             UITransitionManager.Instance.FadeIn(OverlayCanvas);
//         }
        
//     }

//     private void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             playerInside = false;
//         }
//         //OverlayCanvas.SetActive(false);
//         if (OverlayCanvas != null)
//         {
//             UITransitionManager.Instance.FadeOut(OverlayCanvas);
//         }
//     }

//     private void Update()
//     {
//         if (!inputManager.canTakeInputs) return;
//         if (playerInside && inputManager.InteractWasPressed)
//         {
//             if (!string.IsNullOrEmpty(targetSceneName))
//             {
//                 PlayerSpawnManager.Instance.SetSpawnInfo(spawnPointName);
//                 //SceneManager.LoadScene(targetSceneName);
//                 SceneLoadingManager.Instance.LoadScene(targetSceneName);
//             }
//             else
//             {
//                 Debug.LogWarning("Target scene name is not set");
//             }
//         }
//     }
// }
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TeleportTrigger : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private string interactionText = "Enter";
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private GameObject overlayCanvas;

    [Header("Scene Transition Settings")]
    [Tooltip("Name of the scene to load")]
    [SerializeField] private string targetSceneName;

    [Tooltip("Name of the Empty GameObject in the target scene to spawn at")]
    [SerializeField] private string spawnPointName = "PlayerSpawnPoint";


    private void Awake()
    {
        int layer = LayerMask.NameToLayer("Interactable");
        if (layer != -1)
            gameObject.layer = layer;

        SphereCollider col = GetComponent<SphereCollider>();
        col.isTrigger = true;
    }

    public string text => interactionText;

    public bool interactable => isInteractable;

    public void Interact(GameObject interactor)
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning("Target scene name is not set.");
            return;
        }

        PlayerSpawnManager.Instance.SetSpawnInfo(spawnPointName);
        SceneLoadingManager.Instance.LoadScene(targetSceneName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (overlayCanvas != null)
            UITransitionManager.Instance.FadeIn(overlayCanvas);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (overlayCanvas != null)
            UITransitionManager.Instance.FadeOut(overlayCanvas);
    }

    public void SetInteractable(bool value)
    {
        isInteractable = value;
    }

    public void ToggleInteractable()
    {
        isInteractable = !isInteractable;
    }
}