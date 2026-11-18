using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private float range = 100f;
    [SerializeField] private int damage = 25;
    [SerializeField] private float fireRate = 0.2f;

    private Transform cam;
    private float nextFireTime;

    void Awake()
    {
        var main = Camera.main;
        if (main != null) cam = main.transform;
    }

    void Update()
    {
        if (cam == null) return;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Fire()
    {
        if (Physics.Raycast(cam.position, cam.forward, out var hit, range))
        {
            var enemyHealth = hit.collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }
}
