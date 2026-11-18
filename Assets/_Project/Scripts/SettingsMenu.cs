using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

// SettingsMenu - UI cài đặt, tạm dừng game (timeScale = 0), slider độ nhạy chuột
public class SettingsMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private TextMeshProUGUI sensitivityValueText;
    [SerializeField] private Button resumeButton;

    private PlayerController playerController;
    private bool isOpen = false;

    void Awake()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        Time.timeScale = 1f;

        var player = GameObject.FindWithTag("Player");
        if (player != null)
            playerController = player.GetComponent<PlayerController>();

        if (mouseSensitivitySlider != null && playerController != null)
        {
            mouseSensitivitySlider.value = playerController.GetMouseSensitivity();
            mouseSensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            UpdateSensitivityText(playerController.GetMouseSensitivity());
        }

        if (resumeButton != null)
            resumeButton.onClick.AddListener(CloseSettings);
    }

    void Update()
    {
        // Mở/đóng cài đặt bằng phím Esc
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isOpen) CloseSettings();
            else OpenSettings();
        }

        if (isOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void OpenSettings()
    {
        isOpen = true;
        Time.timeScale = 0f; // Tạm dừng game
        if (settingsPanel != null) settingsPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CloseSettings()
    {
        isOpen = false;
        Time.timeScale = 1f; // Tiếp tục game
        if (settingsPanel != null) settingsPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnSensitivityChanged(float value)
    {
        if (playerController != null)
            playerController.SetMouseSensitivity(value);
        UpdateSensitivityText(value);
    }

    void UpdateSensitivityText(float value)
    {
        if (sensitivityValueText != null)
            sensitivityValueText.text = $"Độ nhạy chuột: {value:F1}";
    }
}
