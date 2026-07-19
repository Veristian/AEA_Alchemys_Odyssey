using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float groundDrag = 6f;
    public float airMultiplier = 0.5f;

    [Header("Jump")]
    public float jumpForce = 12f;
    public float jumpCooldown = 0.3f;
    public float playerHeight = 2f;
    public LayerMask whatIsGround;

    [Header("References")]
    public InputManager inputManager;
    public Transform orientation;
    public Transform cameraTarget;


    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private float IdleWait = 10f;
    [SerializeField] private float speedThrehold = 2f; // Adjust as needed
    [Header("Audio")]
    [SerializeField] private string footstepSound = "DirtFootstep";


    private Coroutine idleRoutine;

    private Rigidbody rb;
    private bool grounded;
    private bool readyToJump = true;
    private Vector3 moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (inputManager == null)
            inputManager = InputManager.Instance;

        walkSpeed = moveSpeed;
        sprintSpeed = moveSpeed * 1.5f;
    }

    private void Start()
    {
        idleRoutine = StartCoroutine(WaitAndChooseRandom());
    }

    private void Update()
    {
        if (!inputManager.canTakeInputs) return;

        // Ground Check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();

        // Handle drag
        rb.drag = grounded ? groundDrag : 0;

    }

    private void FixedUpdate()
    {
        MovePlayer();


       float CurrentSpeed = rb.velocity.magnitude;
       animator.SetFloat("Speed", CurrentSpeed);  

        
    }

    private void MyInput()
    {
        Vector2 moveInput = inputManager.Movement;

        // Jump
        if (inputManager.JumpWasPressed && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        Vector2 moveInput = inputManager.Movement;

        // Calculate movement direction relative to orientation (camera forward)
        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // Reset Y velocity for consistent jump height
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    public bool IsGrounded() => grounded;
    public Vector3 GetVelocity() => rb.velocity;

    IEnumerator WaitAndChooseRandom()
    {
        while (true)
        {
            while (isMoving())
            {
                yield return null; // Wait until the player is not moving
            }
            // Wait for set seconds
            yield return new WaitForSeconds(IdleWait);

            // Choose a random number. 

            int randomNumber = Random.Range(1, 3);

            // Perform actions based on the chosen number
            if (randomNumber == 1)
            {
                animator.SetTrigger("IdleHop");
            }
            else if (randomNumber == 2)
            {
                animator.SetTrigger("IdleStretch");
            }
        }

    }

    public float GetCurrentSpeed()
    {
        return rb.velocity.magnitude;
    }

    bool isMoving()
    {
        return GetCurrentSpeed() > speedThrehold; // Adjust threshold as needed
    }

    public void PlayFootstepSound()
    {
        AudioController.Instance.PlaySFX(footstepSound);
    }
}