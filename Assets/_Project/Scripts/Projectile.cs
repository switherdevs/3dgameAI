using UnityEngine;

// Projectile - Đạn phát sáng, tự hủy khi va chạm hoặc hết thời gian
public class Projectile : MonoBehaviour
{
    [SerializeField] private int damage = 25;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private GameObject impactEffectPrefab;
    [SerializeField] private GameObject muzzleVfxPrefab;   // Hiệu ứng VFX khi đạn vừa phát ra từ nòng súng
    [SerializeField] private GameObject trailVfxPrefab;    // Hiệu ứng VFX khi đạn bay (đặt làm child của đạn)
    [SerializeField] private LayerMask shootableMask;

    private float spawnTime;

    void Awake()
    {
        spawnTime = Time.time;

        // Tự phát sáng màu vàng qua emissive material nếu có renderer
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            var mat = renderer.material;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.yellow * 2f);
        }

        // VFX nòng súng (phát ra 1 lần ngay khi đạn instantiate)
        if (muzzleVfxPrefab != null)
        {
            var vfx = Instantiate(muzzleVfxPrefab, transform.position, transform.rotation);
            Destroy(vfx, 1f);
        }

        // VFX hông đạn bay theo (gắn làm child để di chuyển cùng đạn)
        if (trailVfxPrefab != null)
        {
            Instantiate(trailVfxPrefab, transform.position, transform.rotation, transform);
        }
    }

    void Update()
    {
        if (Time.time - spawnTime > lifeTime)
        {
            Destroy(gameObject);
        }
    }

void OnCollisionEnter(Collision collision)
    {
        // Ignore the shooter so the projectile can leave the player's collider.
        if (collision.collider.CompareTag("Player")) return;

        // Hiệu ứng va chạm
        ContactPoint contact = collision.contacts[0];
        BulletHole.Create(contact.point, contact.normal);
        if (impactEffectPrefab != null)
        {
            var instance = Instantiate(impactEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal));
            var particleSystem = instance.GetComponent<ParticleSystem>();
            if (particleSystem != null) particleSystem.Play(true);
            Destroy(instance, 2f);
        }

        // Gây sát thương nếu trúng enemy
        var enemyHealth = collision.collider.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
