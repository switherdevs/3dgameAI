using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class GameUI : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI enemiesText;

    [Header("Dead UI")]
    [SerializeField] private GameObject deadPanel;
    [SerializeField] private TextMeshProUGUI deadTitleText;
    [SerializeField] private TextMeshProUGUI deadHintText;
    [SerializeField] private UnityEngine.UI.Button loadButton;

    private PlayerHealth playerHealth;
    private bool isDead = false;

    void Awake()
    {
        if (healthText == null)
        {
            var t = transform.Find("HPText");
            if (t != null) healthText = t.GetComponent<TextMeshProUGUI>();
        }
        if (enemiesText == null)
        {
            var t = transform.Find("EnemiesText");
            if (t != null) enemiesText = t.GetComponent<TextMeshProUGUI>();
        }
        if (deadPanel == null)
        {
            var t = transform.Find("DeadPanel");
            if (t != null) deadPanel = t.gameObject;
        }
        if (deadTitleText == null)
        {
            var t = transform.Find("DeadPanel/DeadTitle");
            if (t != null) deadTitleText = t.GetComponent<TextMeshProUGUI>();
        }
        if (deadHintText == null)
        {
            var t = transform.Find("DeadPanel/DeadHint");
            if (t != null) deadHintText = t.GetComponent<TextMeshProUGUI>();
        }
        if (loadButton == null)
        {
            var t = transform.Find("DeadPanel/LoadCheckpoint");
            if (t != null) loadButton = t.GetComponent<UnityEngine.UI.Button>();
        }

        if (deadPanel != null) deadPanel.SetActive(false);

        var player = GameObject.FindWithTag("Player");
        if (player != null) playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += RefreshHealth;
            playerHealth.OnDie += OnPlayerDie;
        }
        RefreshHealth(playerHealth != null ? playerHealth.CurrentHealth : 0);
        RefreshEnemies();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        RefreshEnemies();

        if (isDead)
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    void RefreshHealth(int current)
    {
        if (healthText != null) healthText.text = "HP: " + current;
    }

    void RefreshEnemies()
    {
        if (enemiesText != null)
        {
            int count = UnityEngine.Object.FindObjectsByType<EnemyHealth>(UnityEngine.FindObjectsSortMode.None).Length;
            enemiesText.text = "Enemies: " + count;
        }
    }

    void OnPlayerDie()
    {
        isDead = true;
        if (deadPanel != null) deadPanel.SetActive(true);
        if (deadTitleText != null) deadTitleText.text = "BẠN ĐÃ CHẾT";
        if (deadHintText != null) deadHintText.text = "Nhấn LOAD CHECKPOINT để hồi sinh";
    }

    public void LoadCheckpoint()
    {
        if (playerHealth != null)
        {
            playerHealth.Respawn();
        }
        if (deadPanel != null) deadPanel.SetActive(false);
        isDead = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
