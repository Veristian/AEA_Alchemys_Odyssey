using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CameraPivotTrigger : MonoBehaviour
{
    [Header("Target Pivot")]
    [SerializeField] private Transform targetPivot;

    [Header("Behavior")]
    [SerializeField] private bool toggleMode = false;   // If true, uses Toggle instead of SwitchTo

    private CameraPivotManager cameraManager;

    private void Start()
    {
        cameraManager = FindObjectOfType<CameraPivotManager>();

        if (cameraManager == null)
            Debug.LogError("CameraPivotManager not found in scene!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || cameraManager == null || targetPivot == null)
            return;

        if (toggleMode)
        {
            cameraManager.TogglePivot();
        }
        else
        {
            cameraManager.SwitchToPivot(targetPivot);
            targetPivot = cameraManager.previousPivot; // Save current as previous for toggling back
        }
    }

    private void OnDrawGizmos()
    {
        if (targetPivot != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, targetPivot.position);
        }
    }
}