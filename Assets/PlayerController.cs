using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 9f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("Camera")]
    public CameraFollow cameraFollow;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isMoving;
    private bool isRunning;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        DontDestroyOnLoad(gameObject);

        if (Camera.main != null && cameraFollow == null)
            cameraFollow = Camera.main.GetComponent<CameraFollow>();
    }

    void Update()
    {
        HandleGravity();
        HandleMovement();
        UpdateAnimations();
    }

    void HandleGravity()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(moveX, 0, moveZ);
        isMoving = inputDir.magnitude > 0.1f;
        isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving;

        Vector3 moveDir = Vector3.zero;

        if (isMoving && cameraFollow != null)
        {
            Vector3 camForward = cameraFollow.transform.forward;
            Vector3 camRight = cameraFollow.transform.right;
            camForward.y = 0; camRight.y = 0;
            camForward.Normalize(); camRight.Normalize();

            moveDir = (camForward * moveZ + camRight * moveX).normalized;

            if (moveDir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), rotationSpeed * Time.deltaTime);
        }

        float speed = isRunning ? runSpeed : walkSpeed;
        Vector3 finalMove = moveDir * speed + velocity;
        controller.Move(finalMove * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator?.SetTrigger("Jump");
        }
    }

    void UpdateAnimations()
    {
        if (animator == null) return;

        animator.SetBool("IsWalking", isMoving && !isRunning);
        animator.SetBool("IsRunning", isMoving && isRunning);
        animator.SetFloat("Speed", isMoving ? (isRunning ? 9f : 0.5f) : 0f);

        if (Input.GetKeyDown(KeyCode.E)) animator.SetTrigger("Pickup");
    }

    public void TeleportPlayer(Vector3 targetPosition, Quaternion? newRotation = null)
    {
        if (controller == null) return;

        controller.enabled = false;

        RaycastHit hit;
        Vector3 rayStart = targetPosition + Vector3.up * 10f;
        if (Physics.Raycast(rayStart, Vector3.down, out hit, 50f))
            targetPosition.y = hit.point.y + controller.skinWidth;

        transform.position = targetPosition;

        if (newRotation.HasValue)
            transform.rotation = newRotation.Value;

        controller.enabled = true;
        controller.Move(Vector3.zero);
        velocity = Vector3.zero;

        if (cameraFollow == null)
            cameraFollow = FindObjectOfType<CameraFollow>();

        cameraFollow?.SnapBehindPlayer();

        if (cameraFollow != null)
        {
            Vector3 forward = cameraFollow.transform.forward;
            forward.y = 0;
            if (forward != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(forward);
        }
    }
}
