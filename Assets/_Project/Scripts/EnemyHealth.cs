using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    public int CurrentHealth { get; private set; }

    public System.Action OnDie;

    void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (CurrentHealth <= 0) return;
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        if (CurrentHealth <= 0)
        {
            OnDie?.Invoke();
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}
