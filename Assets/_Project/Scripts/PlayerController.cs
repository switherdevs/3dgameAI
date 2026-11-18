using UnityEngine;
using UnityEngine.InputSystem;

// PlayerController v1.5 - New Input System (PlayerInput), nhảy, sprint, chuột, Rigidbody mượt
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float jumpForce = 8f;

    [Header("Mouse")]
    [SerializeField] private float mouseSensitivity = 2f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck; // con nhỏ đặt dưới chân, để trống nếu không dùng
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundMask = ~0; // Mặc định va chạm mọi layer, nhảy hoạt động ngay

    [Header("Fall Protection")]
    [SerializeField] private float fallResetY = -20f;
    [SerializeField] private Transform spawnPoint;

    [Header("Cursor")]
    [SerializeField] private bool hideCursor = true;

    private Rigidbody rb;
    private PlayerHealth health;
    private Transform cam;
    private PlayerInput playerInput;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isGrounded;
    private bool isSprinting;
    private float yaw;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.maxDepenetrationVelocity = 1f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        health = GetComponent<PlayerHealth>();
        health.OnDie += () => enabled = false;

        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
            Debug.LogWarning("[PlayerController] Thiếu PlayerInput component trên Player.");

        var main = Camera.main;
        if (main != null) cam = main.transform;

        if (spawnPoint == null)
        {
            var sp = GameObject.FindWithTag("SpawnPoint");
            if (sp != null) spawnPoint = sp.transform;
        }

        yaw = transform.eulerAngles.y;

        if (hideCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Gọi tự động bởi PlayerInput (Behavior = Send Messages) khi action Move thay đổi
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    public void OnJump()
    {
        if (isGrounded)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }

    void FixedUpdate()
    {
        if (cam == null) return;

        // Kiểm tra mặt đất bằng CheckSphere dưới chân (không bị sai do gốc nhân vật nằm trên đất)
        Vector3 checkPos = groundCheck != null ? groundCheck.position : transform.position + Vector3.down * 0.6f;
        isGrounded = Physics.CheckSphere(checkPos, groundCheckRadius, groundMask);

        // Xoay nhân vật theo chuột (yaw) từ New Input Look
        yaw += lookInput.x * mouseSensitivity;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        float currentSpeed = isSprinting && moveInput.sqrMagnitude > 0.0001f ? sprintSpeed : moveSpeed;

        Vector3 forward = cam.forward; forward.y = 0f;
        Vector3 right = cam.right; right.y = 0f;
        forward.Normalize();
        right.Normalize();
        Vector3 move = (forward * moveInput.y + right * moveInput.x) * (currentSpeed * 10f);

        // Giữ nguyên vận tốc y (để nhảy không bị override)
        Vector3 targetVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        rb.linearVelocity = targetVelocity;

        // Giới hạn tốc độ ngang
        Vector3 horizontal = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        float maxSpeed = currentSpeed * 1.5f;
        if (horizontal.sqrMagnitude > maxSpeed * maxSpeed)
        {
            horizontal = horizontal.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(horizontal.x, rb.linearVelocity.y, horizontal.z);
        }

        // Fall protection
        if (transform.position.y < fallResetY)
        {
            RespawnPlayer();
        }
    }

    void RespawnPlayer()
    {
        Vector3 pos = spawnPoint != null ? spawnPoint.position : new Vector3(0, 1, 0);
        transform.position = pos + Vector3.up * 1f;
        rb.linearVelocity = Vector3.zero;
    }

    public float GetMouseSensitivity() => mouseSensitivity;
    public void SetMouseSensitivity(float value) => mouseSensitivity = value;
}
