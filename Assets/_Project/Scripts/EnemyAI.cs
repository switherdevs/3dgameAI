using UnityEngine;

// EnemyAI v1.3 - Di chuyển mượt hơn, Rigidbody interpolation
[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Combat")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float chaseRange = 18f;
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private int attackDamage = 15;

    [Header("Smooth Movement")]
    [SerializeField] private float rotationSmoothSpeed = 10f;
    [SerializeField] private float positionSmoothSpeed = 8f;

    [Header("Fall Protection")]
    [SerializeField] private float fallResetY = -20f;
    private Vector3 startPosition;

    private Rigidbody rb;
    private float nextAttackTime;
    private Vector3 smoothVelocity;
    private Quaternion targetRotation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.maxDepenetrationVelocity = 1f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        startPosition = transform.position;

        if (player == null)
        {
            var go = GameObject.FindWithTag("Player");
            if (go != null)
            {
                player = go.transform;
                playerHealth = go.GetComponent<PlayerHealth>();
            }
        }
        else if (playerHealth == null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Fall protection
        if (transform.position.y < fallResetY)
        {
            transform.position = startPosition;
            rb.linearVelocity = Vector3.zero;
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > chaseRange) return;

        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0f;

        // Xoay mượt hơn với Slerp
        if (dir.sqrMagnitude > 0.0001f)
        {
            targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothSpeed * Time.fixedDeltaTime);
        }

        if (dist > attackRange)
        {
            // Chase - di chuyển mượt với SmoothDamp
            Vector3 targetPos = transform.position + dir * (moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(Vector3.SmoothDamp(transform.position, targetPos, ref smoothVelocity, 1f / positionSmoothSpeed));
        }
        else
        {
            // Dừng lại khi tấn công
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

            if (Time.time >= nextAttackTime && playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                nextAttackTime = Time.time + attackRate;
            }
        }
    }
}
