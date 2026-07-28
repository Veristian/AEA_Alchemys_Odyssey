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
    private Vector2 lastInputDir = Vector3.zero;
    // [SerializeField, Range(0f,1f)] private float directionChangeThreshold = 0.1f; 
    bool usingThirdPerson;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inputManager = InputManager.Instance;
        
        var trigger = FindAnyObjectByType<WhilePivotTrigger>();
        if (trigger == null)
        {
            usingThirdPerson = true;
        }
        else
            usingThirdPerson = false;
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
        if (inputManager.Movement != lastInputDir || usingThirdPerson)
        {
            orientation.forward = viewDir.normalized;       
        }
        lastInputDir = InputManager.Instance.Movement;
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