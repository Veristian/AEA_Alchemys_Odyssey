using UnityEngine;
using Cinemachine;

public class CameraPivotManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    // Currently active pivot
    [SerializeField] private Transform currentPivot;

    // Previous pivot (for easy back-and-forth toggling)
    [SerializeField] public Transform previousPivot;

    private void Awake()
    {
        if (virtualCamera == null)
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        // Initialize with whatever is currently set on the camera
        if (virtualCamera != null && virtualCamera.Follow != null)
        {
            currentPivot = virtualCamera.Follow;
            previousPivot = currentPivot;
        }
    }

    /// <summary>
    /// Switch to a new pivot. Automatically saves the old one for toggling.
    /// </summary>
    public void SwitchToPivot(Transform newPivot)
    {
        if (virtualCamera == null || newPivot == null) return;
        if (newPivot == currentPivot) return; // Already there

        // Save current as previous
        previousPivot = currentPivot;

        // Update current
        currentPivot = newPivot;

        // Apply to Cinemachine
        virtualCamera.Follow = currentPivot;

        Debug.Log($"Camera switched to pivot: {newPivot.name}");
    }

    /// <summary>
    /// Toggle between the current and previous pivot (great for the same trigger)
    /// </summary>
    public void TogglePivot()
    {
        if (previousPivot == null) return;

        Transform temp = currentPivot;
        currentPivot = previousPivot;
        previousPivot = temp;

        virtualCamera.Follow = currentPivot;

        Debug.Log($"Camera toggled to: {currentPivot.name}");
    }

    /// <summary>
    /// Force set a specific pivot without affecting toggle history
    /// </summary>
    public void SetPivot(Transform pivot)
    {
        if (virtualCamera == null || pivot == null) return;

        currentPivot = pivot;
        previousPivot = pivot; // Reset history
        virtualCamera.Follow = pivot;
    }

    // Optional: Public read-only access
    public Transform CurrentPivot => currentPivot;
}