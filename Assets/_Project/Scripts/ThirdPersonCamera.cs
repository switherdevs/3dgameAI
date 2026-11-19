using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [Header("Orbit")]
    [SerializeField] private float distance = 3.5f;
    [SerializeField] private float height = 1.7f;
    [SerializeField] private float shoulderOffset = 0.65f;
    [SerializeField] private float lookHeight = 1.4f;
    [SerializeField] private float rotateSpeed = 0.1f;
    [SerializeField] private float sensitivity = 1f;
    [SerializeField] private float minPitch = -45f;
    [SerializeField] private float maxPitch = 60f;
    [Header("Follow")]
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float collisionRadius = 0.25f;
    [SerializeField] private float collisionPadding = 0.15f;
    [SerializeField] private LayerMask collisionMask = ~0;

    private float yaw;
    private float pitch;

    void Awake()
    {
        if (target == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null) target = player.transform;
        }
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    public float GetSensitivity() => sensitivity;
    public void SetSensitivity(float value) => sensitivity = Mathf.Max(0f, value);

    void LateUpdate()
    {
        if (target == null) return;

        var mouse = Mouse.current;
        if (mouse != null)
        {
            var delta = mouse.delta.ReadValue();
            yaw += delta.x * rotateSpeed * sensitivity;
            pitch -= delta.y * rotateSpeed * sensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 focus = target.position + Vector3.up * lookHeight;
        Vector3 idealPos = focus + rot * new Vector3(shoulderOffset, height - lookHeight, -distance);
        Vector3 cameraPos = idealPos;
        Vector3 offset = idealPos - focus;
        int mask = collisionMask & ~(1 << target.gameObject.layer);
        if (Physics.SphereCast(focus, collisionRadius, offset.normalized, out RaycastHit hit, offset.magnitude, mask, QueryTriggerInteraction.Ignore))
            cameraPos = focus + offset.normalized * Mathf.Max(0.1f, hit.distance - collisionPadding);
        transform.position = cameraPos;
        transform.rotation = Quaternion.LookRotation(focus + rot * Vector3.right * shoulderOffset - transform.position, Vector3.up);
    }
}
