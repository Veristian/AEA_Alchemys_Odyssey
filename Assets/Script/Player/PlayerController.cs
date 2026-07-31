using System;
using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float sprintSpeed = 8f;
    public float groundDrag = 6f;
    public float airMultiplier = 0.5f;
    public float gravityMultiplier = 2.5f;

    [Header("Jump")]
    public float jumpForce = 12f;
    public float jumpCooldown = 0.3f;
    public float playerHeight = 2f;
    public LayerMask whatIsGround;

    [Header("References")]
    public InputManager inputManager;
    public Transform orientation;
    public Transform cameraTarget;
    public Transform chemyAnimatedObj;


    // [HideInInspector] public float walkSpeed;
    // [HideInInspector] public float sprintSpeed;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private float IdleWait = 10f;
    [SerializeField] private float speedThrehold = 2f; // Adjust as needed
    AudioSource audioSource;

    private Coroutine idleRoutine;

    private Rigidbody rb;
    private bool grounded;
    private bool readyToJump = true;
    private Vector3 moveDirection;

    [Header("Footsteps")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip runClip;

    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.3f;
    [SerializeField] private float volumeLerpSpeed = 10f;
    [SerializeField] private float pitchLerpSpeed = 10f;

    [Header("Slope Movement")]
    [SerializeField] private float maxSlopeAngle = 45f;
    [SerializeField] private float slopeRayDistance = 1.5f;

    private RaycastHit slopeHit;
    private bool onSlope;
    private bool isSprinting => InputManager.Instance.SprintIsHeld && grounded;
    private CapsuleCollider capsuleCollider;

    private void UpdateFootsteps()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        float speed = flatVel.magnitude;

        // Speed normalized from 0 to moveSpeed
        float normalizedSpeed = Mathf.Clamp01(speed / (isSprinting ? sprintSpeed : moveSpeed));

        if (runClip != null && walkClip != null)
        {
            if (isSprinting)
            {
                if (footstepSource.clip != runClip)
                    footstepSource.clip = runClip;
            }
            else
            {
                if (footstepSource.clip != walkClip)
                    footstepSource.clip = walkClip;
            }
        }
        // Play only when moving
        if (normalizedSpeed > 0.05f)
        {
            if (!footstepSource.isPlaying)
                footstepSource.Play();
        }
        else
        {
            if (footstepSource.isPlaying)
                footstepSource.Pause();
        }

        // Smoothly adjust volume and pitch
        footstepSource.volume = Mathf.Lerp(
            footstepSource.volume,
            normalizedSpeed,
            Time.deltaTime * volumeLerpSpeed);

        footstepSource.pitch = Mathf.Lerp(
            footstepSource.pitch,
            Mathf.Lerp(minPitch, maxPitch, normalizedSpeed),
            Time.deltaTime * pitchLerpSpeed);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        rb.freezeRotation = true;
        capsuleCollider = GetComponent<CapsuleCollider>();
        if (inputManager == null)
            inputManager = InputManager.Instance;

    }

    private void Start()
    {
        idleRoutine = StartCoroutine(WaitAndChooseRandom());
    }

    private void Update()
    {
        if (!inputManager.canTakeInputs) return;

        // Ground Check
        Vector3 capsuleCenter = capsuleCollider.bounds.center;
        float rayDistance = capsuleCollider.bounds.extents.y + 0.5f;

        grounded = Physics.Raycast(
            capsuleCenter,
            Vector3.down,
            rayDistance,
            whatIsGround
        );
        MyInput();
        SpeedControl();

        // Handle drag
        rb.drag = grounded ? groundDrag : 0;

        UpdateFootsteps();

        

    }

    private void FixedUpdate()
    {
        MovePlayer();
        ApplyExtraGravity();

        float CurrentSpeed = rb.velocity.magnitude;
        animator.SetFloat("Speed",CurrentSpeed > 3 ? isSprinting ? 15 : 6 : 0);  

        if (chemyAnimatedObj.localPosition.x > 0.07f || chemyAnimatedObj.localPosition.x < -0.07f)
        {
            chemyAnimatedObj.localPosition = Vector3.Lerp(chemyAnimatedObj.localPosition, new Vector3(0, chemyAnimatedObj.localPosition.y, chemyAnimatedObj.localPosition.z), Time.fixedDeltaTime);
        }
        if (chemyAnimatedObj.localPosition.y > 0.07f || chemyAnimatedObj.localPosition.y < -0.07f)
        {
            chemyAnimatedObj.localPosition = Vector3.Lerp(chemyAnimatedObj.localPosition, new Vector3(chemyAnimatedObj.localPosition.x, 0, chemyAnimatedObj.localPosition.z), Time.fixedDeltaTime);
        }
        if (chemyAnimatedObj.localPosition.z > 0.07f || chemyAnimatedObj.localPosition.z < -0.07f)
        {
            chemyAnimatedObj.localPosition = Vector3.Lerp(chemyAnimatedObj.localPosition, new Vector3(chemyAnimatedObj.localPosition.x, chemyAnimatedObj.localPosition.y, 0), Time.fixedDeltaTime);
        }
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

        // Calculate movement direction relative to camera
        moveDirection =
            orientation.forward * moveInput.y +
            orientation.right * moveInput.x;

        if (moveInput.magnitude < 0.1f)
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(-flatVel * 15f, ForceMode.Acceleration);
            return;
        }

        moveDirection.Normalize();

        // Adjust movement to follow terrain slope
        Vector3 finalDirection;

        if (GetSlopeMoveDirection(moveDirection, out Vector3 slopeDirection))
        {
            finalDirection = slopeDirection;
        }
        else
        {
            finalDirection = moveDirection;
        }

        float forceMultiplier = grounded ? 10f : 10f * airMultiplier;

        rb.AddForce(
            finalDirection * (isSprinting ? sprintSpeed : moveSpeed) * forceMultiplier,
            ForceMode.Force
        );
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > (isSprinting ? sprintSpeed : moveSpeed))
        {
            Vector3 limitedVel = flatVel.normalized * (isSprinting ? sprintSpeed : moveSpeed);

            rb.velocity = new Vector3(
                limitedVel.x,
                rb.velocity.y,
                limitedVel.z
            );
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

            int randomNumber = UnityEngine.Random.Range(1, 3);

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

    private void ApplyExtraGravity()
    {
        if (!grounded)
        {
            // Extra downward force so the player falls faster than Unity�s default gravity
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);
        }
    }
    private bool GetSlopeMoveDirection(Vector3 direction, out Vector3 slopeDirection)
    {
        slopeDirection = direction;

        if (Physics.Raycast(
            transform.position,
            Vector3.down,
            out slopeHit,
            playerHeight * 0.5f + slopeRayDistance,
            whatIsGround))
        {
            float slopeAngle = Vector3.Angle(slopeHit.normal, Vector3.up);

            if (slopeAngle <= maxSlopeAngle && slopeAngle > 0.1f)
            {
                onSlope = true;

                // Project movement direction onto the terrain surface
                slopeDirection = Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
                return true;
            }
        }

        onSlope = false;
        return false;
    }
}