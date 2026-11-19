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

    [Header("Ammo")]
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private int reserveAmmo = 90;
    [SerializeField] private int ammoPerPickup = 30;
    [SerializeField] private float reloadTime = 1.5f;
    [SerializeField] private Vector3 reloadPosition = new Vector3(0f, 0.3f, 0f);

    private int currentAmmo;

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
        currentAmmo = magazineSize;
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
        if (keyboard != null && keyboard.rKey.wasPressedThisFrame)
            StartReload();

        if (keyboard != null && keyboard[toggleKey].wasPressedThisFrame)
        {
            isAutomatic = !isAutomatic;
            Debug.Log($"[Gun] Chế độ: {(isAutomatic ? "AUTO (tự động)" : "SEMI (đơn lẻ))")}");
        }

        if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;
            var muzzleLight = muzzle.GetComponent<Light>();
            if (muzzleLight != null) muzzleLight.intensity = 18f * Mathf.Clamp01(flashTimer / flashDuration);
            if (flashTimer <= 0f)
            {
                if (muzzleLight != null) muzzleLight.intensity = 0f;
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
                int loaded = Mathf.Min(magazineSize - currentAmmo, reserveAmmo);
                currentAmmo += loaded;
                reserveAmmo -= loaded;
                OnAmmoChanged?.Invoke(currentAmmo, reserveAmmo);
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
        if (isReloading || currentAmmo >= magazineSize || reserveAmmo <= 0) return;
        isReloading = true;
        reloadTimer = 0f;
        Debug.Log("[Gun] Đang nạp đạn...");
    }

    void Fire()
    {
        if (currentAmmo <= 0 || isReloading || bulletPrefab == null || cam == null) return;

        currentAmmo--;
        OnAmmoChanged?.Invoke(currentAmmo, reserveAmmo);
        if (bulletPrefab != null && cam != null)
        {
            Vector3 aimPoint = cam.position + cam.forward * range;
            var hits = Physics.RaycastAll(cam.position, cam.forward, range, ~0, QueryTriggerInteraction.Ignore);
            float nearest = range;
            foreach (var hit in hits)
            {
                if (hit.transform.root == transform.root) continue;
                if (hit.distance < nearest)
                {
                    nearest = hit.distance;
                    aimPoint = hit.point;
                }
            }

            Vector3 direction = (aimPoint - muzzle.position).normalized;
            var muzzleLight = muzzle.GetComponent<Light>();
            if (muzzleLight != null) muzzleLight.intensity = 18f;
            var bullet = Instantiate(bulletPrefab, muzzle.position, Quaternion.LookRotation(direction));
            var rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                rb.angularVelocity = Vector3.zero;
                rb.linearVelocity = direction * bulletSpeed;
            }
            foreach (var shooterCollider in transform.root.GetComponentsInChildren<Collider>())
            {
                foreach (var bulletCollider in bullet.GetComponentsInChildren<Collider>())
                    Physics.IgnoreCollision(shooterCollider, bulletCollider);
            }
            Destroy(bullet, bulletLife);
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
            var particleSystem = flash.GetComponent<ParticleSystem>();
            if (particleSystem != null) particleSystem.Play(true);
        }
        else if (muzzleFlashPrefab != null)
        {
            var instance = Instantiate(muzzleFlashPrefab, muzzle.position, muzzle.rotation, muzzle);
            instance.name = "MuzzleFlash";
            var particleSystem = instance.GetComponent<ParticleSystem>();
            if (particleSystem != null) particleSystem.Play(true);
            Destroy(instance, 2f);
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

    public System.Action<int, int> OnAmmoChanged;
    public int CurrentAmmo => currentAmmo;
    public int ReserveAmmo => reserveAmmo;
    public bool IsAutomatic => isAutomatic;
    public void SetAutomatic(bool value) => isAutomatic = value;
    public bool IsReloading => isReloading;

    public void AddReserveAmmo(int amount)
    {
        if (amount <= 0) return;
        reserveAmmo += amount;
        OnAmmoChanged?.Invoke(currentAmmo, reserveAmmo);
    }
}
