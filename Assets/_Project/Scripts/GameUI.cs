using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI enemiesText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI weaponText;
    [SerializeField] private Image healthFill;

    [Header("Dead UI")]
    [SerializeField] private GameObject deadPanel;
    [SerializeField] private TextMeshProUGUI deadTitleText;
    [SerializeField] private TextMeshProUGUI deadHintText;
    [SerializeField] private UnityEngine.UI.Button loadButton;

    private PlayerHealth playerHealth;
    private WeaponInventory inventory;
    private Gun playerGun;
    private bool isDead = false;

    void Awake()
    {
        foreach (var text in GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (text.name == "HPText") healthText = text;
            else if (text.name == "EnemiesText") enemiesText = text;
            else if (text.name == "AmmoText") ammoText = text;
        }
        if (healthFill == null)
        {
            var fill = transform.Find("HealthPanel/HealthBar/Fill");
            if (fill != null) healthFill = fill.GetComponent<Image>();
        }
        if (deadPanel == null)
        {
            var t = transform.Find("DeadPanel");
            if (t != null) deadPanel = t.gameObject;
        }
        else if (deadPanel.transform.parent != transform)
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

        if (weaponText == null && transform.Find("AmmoPanel/WeaponText") != null)
            weaponText = transform.Find("AmmoPanel/WeaponText").GetComponent<TextMeshProUGUI>();
        if (weaponText == null && ammoText != null)
        {
            var label = new GameObject("WeaponText", typeof(RectTransform), typeof(TextMeshProUGUI));
            label.transform.SetParent(ammoText.transform.parent, false);
            var rect = label.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(12f, -8f);
            rect.sizeDelta = new Vector2(280f, 30f);
            weaponText = label.GetComponent<TextMeshProUGUI>();
            weaponText.font = ammoText.font;
            weaponText.fontSize = 18f;
            weaponText.color = Color.white;
        }
        if (deadPanel != null) deadPanel.SetActive(false);

        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            inventory = player.GetComponent<WeaponInventory>();
            playerGun = inventory != null ? inventory.SelectedWeapon : player.GetComponentInChildren<Gun>();
            if (inventory != null) inventory.OnSelectedWeaponChanged += OnWeaponChanged;
        }
        if (ammoText == null && transform.Find("AmmoPanel/AmmoText") != null)
            ammoText = transform.Find("AmmoPanel/AmmoText").GetComponent<TextMeshProUGUI>();
        if (healthText == null && transform.Find("HealthPanel/HPText") != null)
            healthText = transform.Find("HealthPanel/HPText").GetComponent<TextMeshProUGUI>();
        if (enemiesText == null && transform.Find("HealthPanel/EnemiesText") != null)
            enemiesText = transform.Find("HealthPanel/EnemiesText").GetComponent<TextMeshProUGUI>();
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += RefreshHealth;
            playerHealth.OnDie += OnPlayerDie;
        }
        RefreshHealth(playerHealth != null ? playerHealth.CurrentHealth : 0);
        RefreshAmmo();
        RefreshWeapon();
        RefreshEnemies();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        RefreshEnemies();
        RefreshAmmo();
        RefreshWeapon();

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
        if (playerHealth == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null) playerHealth = player.GetComponent<PlayerHealth>();
        }
        if (healthText == null)
        {
            var text = transform.Find("HealthPanel/HPText");
            if (text != null) healthText = text.GetComponent<TextMeshProUGUI>();
        }
        if (healthFill == null)
        {
            var fill = transform.Find("HealthPanel/HealthBar/Fill");
            if (fill != null) healthFill = fill.GetComponent<Image>();
        }
        int maximum = playerHealth != null ? playerHealth.MaxHealth : 100;
        if (healthText != null) healthText.text = $"HP: {current:000}";
        if (healthFill != null) healthFill.fillAmount = maximum > 0 ? (float)current / maximum : 0f;
    }

    void RefreshAmmo()
    {
        if (inventory == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null) inventory = player.GetComponent<WeaponInventory>();
        }
        if (inventory != null) playerGun = inventory.SelectedWeapon;
        else if (playerGun == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null) playerGun = player.GetComponentInChildren<Gun>();
        }
        if (ammoText == null)
        {
            var ammo = transform.Find("AmmoPanel/AmmoText");
            if (ammo != null) ammoText = ammo.GetComponent<TextMeshProUGUI>();
        }
        if (ammoText != null && playerGun != null)
            ammoText.text = $"{playerGun.CurrentAmmo:00}  /  {playerGun.ReserveAmmo:000}";
    }

    void OnWeaponChanged(Gun gun)
    {
        playerGun = gun;
        RefreshWeapon();
        RefreshAmmo();
    }

    void RefreshWeapon()
    {
        if (weaponText == null)
        {
            var label = transform.Find("AmmoPanel/WeaponText");
            if (label != null) weaponText = label.GetComponent<TextMeshProUGUI>();
        }
        if (weaponText != null)
            weaponText.text = playerGun != null ? playerGun.gameObject.name : "NO WEAPON";
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
