using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [Header("Orbit")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float height = 2f;
    [SerializeField] private float rotateSpeed = 200f;
    [SerializeField] private float minPitch = -45f;
    [SerializeField] private float maxPitch = 60f;
    [Header("Follow")]
    [SerializeField] private float followSpeed = 10f;

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

    void LateUpdate()
    {
        if (target == null) return;

        var mouse = Mouse.current;
        if (mouse != null)
        {
            var delta = mouse.delta.ReadValue();
            yaw += delta.x * rotateSpeed * Time.deltaTime;
            pitch -= delta.y * rotateSpeed * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 idealPos = target.position + Vector3.up * height + rot * new Vector3(0f, 0f, -distance);
        transform.position = Vector3.Lerp(transform.position, idealPos, followSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * (height * 0.5f));
    }
}
