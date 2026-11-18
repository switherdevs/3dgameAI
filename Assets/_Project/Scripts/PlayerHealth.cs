using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Transform checkpoint;
    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }

    public System.Action<int> OnHealthChanged;
    public System.Action OnDie;

    void Awake()
    {
        CurrentHealth = maxHealth;
        if (checkpoint == null)
        {
            var cp = GameObject.FindWithTag("SpawnPoint");
            if (cp != null) checkpoint = cp.transform;
        }
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        OnHealthChanged?.Invoke(CurrentHealth);
        if (CurrentHealth <= 0)
        {
            OnDie?.Invoke();
        }
    }

    public void Respawn()
    {
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth);

        if (checkpoint != null)
        {
            transform.position = checkpoint.position + Vector3.up * 1f;
            var rb = GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = Vector3.zero;
        }
    }
}
