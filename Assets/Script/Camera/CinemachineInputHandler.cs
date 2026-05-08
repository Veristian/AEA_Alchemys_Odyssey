using UnityEngine;

public class CinemachineInputHandler : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;        
    public Transform playerObj;          // The player model
    public Transform player;             // Root player transform
    public InputManager inputManager;

    [Header("Camera Settings")]
    public float rotationSpeed = 12f;

    [Header("Camera Styles")]
    public GameObject thirdPersonCam;    // Cinemachine Virtual Cam 


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    private void Update()
    {
        if (!inputManager.canTakeInputs) return;

        HandleOrientation();
        HandlePlayerRotation();
    }


    private void HandleOrientation()
    {
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;       
    }

    private void HandlePlayerRotation()
    {
        Vector2 moveInput = inputManager.Movement;

        {
            Vector3 inputDir = orientation.forward * moveInput.y + orientation.right * moveInput.x;

            if (inputDir.sqrMagnitude > 0.01f)
            {
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            }
        }

    }

}