using UnityEngine;
using UnityEngine.InputSystem;

// Gun v1.3 - Bắn đạn vật lý (projectile), nòng súng sáng, hiệu ứng va chạm, recoil, reload
[RequireComponent(typeof(AudioSource))]
public class Gun : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private float range = 100f;
    [SerializeField] private int damage = 25;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private Transform muzzle;
    [SerializeField] private LayerMask shootableMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 30f;
    [SerializeField] private float bulletLife = 3f;

    [Header("Fire Mode")]
    [SerializeField] private bool isAutomatic = false;
    [SerializeField] private Key toggleKey = Key.V;

    [Header("Recoil")]
    [SerializeField] private float recoilKick = 0.15f;
    [SerializeField] private float recoilReturnSpeed = 10f;
    [SerializeField] private float recoilRise = 0.05f;

    [Header("Reload")]
    [SerializeField] private float reloadTime = 1.5f;
    [SerializeField] private Vector3 reloadPosition = new Vector3(0f, 0.3f, 0f);

    [Header("Effects")]
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private GameObject impactEffectPrefab;
    [SerializeField] private float flashDuration = 0.05f;

    private Transform gunTransform;
    private Vector3 gunIdlePosition;
    private Vector3 recoilOffset;
    private Transform cam;
    private float nextFireTime;
    private float flashTimer = 0f;
    private bool isReloading = false;
    private float reloadTimer = 0f;
    private int lastFireFrame = -1;

    void Awake()
    {
        var main = Camera.main;
        if (main != null) cam = main.transform;

        gunTransform = transform;
        gunIdlePosition = gunTransform.localPosition;

        // Fallback: use this transform when no separate muzzle point is assigned.
        if (muzzle == null) muzzle = transform;

        if (GetComponent<AudioSource>() == null)
            gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (cam == null && Camera.main != null) cam = Camera.main.transform;

        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard[toggleKey].wasPressedThisFrame)
        {
            isAutomatic = !isAutomatic;
            Debug.Log($"[Gun] Chế độ: {(isAutomatic ? "AUTO (tự động)" : "SEMI (đơn lẻ))")}");
        }

        if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0f)
            {
                var flash = muzzle.Find("MuzzleFlash");
                if (flash != null) flash.gameObject.SetActive(false);
            }
        }

        if (isReloading)
        {
            reloadTimer += Time.deltaTime;
            float t = reloadTimer / reloadTime;
            float moveT = t < 0.5f ? t * 2f : (1f - t) * 2f;
            gunTransform.localPosition = Vector3.Lerp(gunIdlePosition, reloadPosition, moveT);

            if (t >= 1f)
            {
                isReloading = false;
                reloadTimer = 0f;
                gunTransform.localPosition = gunIdlePosition;
                Debug.Log("[Gun] Nạp đạn xong");
            }
            return;
        }

        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, recoilReturnSpeed * Time.deltaTime);
        gunTransform.localPosition = gunIdlePosition + recoilOffset;

        bool wantFire = false;
        if (isAutomatic)
        {
            var mouse = Mouse.current;
            if (mouse != null && mouse.leftButton.isPressed)
                wantFire = true;
        }
        else
        {
            var mouse = Mouse.current;
            if (mouse != null && mouse.leftButton.wasPressedThisFrame)
                wantFire = true;
        }

        if (wantFire && lastFireFrame != Time.frameCount && Time.time >= nextFireTime)
        {
            lastFireFrame = Time.frameCount;
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    void StartReload()
    {
        isReloading = true;
        reloadTimer = 0f;
        Debug.Log("[Gun] Đang nạp đạn...");
    }

    void Fire()
    {
        if (bulletPrefab != null && cam != null)
        {
            // Tạo đạn vật lý phát sáng
            var bullet = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
            var rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = muzzle.forward * bulletSpeed;
            Destroy(bullet, bulletLife);
        }
        else if (cam != null)
        {
            // Fallback: raycast nếu không có bulletPrefab
            if (Physics.Raycast(cam.position, cam.forward, out var hit, range, shootableMask))
            {
                var enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                    enemyHealth.TakeDamage(damage);
                ShowImpactEffect(hit.point, hit.normal);
            }
        }

        recoilOffset = new Vector3(0f, recoilRise, -recoilKick);
        gunTransform.localPosition = gunIdlePosition + recoilOffset;

        ShowMuzzleFlash();
    }

    void ShowMuzzleFlash()
    {
        flashTimer = flashDuration;
        var flash = muzzle.Find("MuzzleFlash");
        if (flash != null)
        {
            flash.gameObject.SetActive(true);
        }
        else if (muzzleFlashPrefab != null)
        {
            var instance = Instantiate(muzzleFlashPrefab, muzzle.position, muzzle.rotation, muzzle);
            instance.name = "MuzzleFlash";
            Destroy(instance, flashDuration);
        }
    }

    void ShowImpactEffect(Vector3 position, Vector3 normal)
    {
        if (impactEffectPrefab != null)
        {
            var instance = Instantiate(impactEffectPrefab, position, Quaternion.LookRotation(normal));
            Destroy(instance, 2f);
        }
    }

    public bool IsAutomatic => isAutomatic;
    public void SetAutomatic(bool value) => isAutomatic = value;
    public bool IsReloading => isReloading;
}
