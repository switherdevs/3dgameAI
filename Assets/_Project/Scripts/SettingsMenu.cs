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

    private ThirdPersonCamera thirdPersonCamera;
    private bool isOpen = false;
    private PlayerController playerController;

    void Awake()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        Time.timeScale = 1f;

        thirdPersonCamera = Camera.main != null ? Camera.main.GetComponent<ThirdPersonCamera>() : null;
        var player = GameObject.FindWithTag("Player");
        if (player != null) playerController = player.GetComponent<PlayerController>();

        if (mouseSensitivitySlider != null && thirdPersonCamera != null)
        {
            mouseSensitivitySlider.value = thirdPersonCamera.GetSensitivity();
            mouseSensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            UpdateSensitivityText(thirdPersonCamera.GetSensitivity());
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
        if (playerController != null) playerController.enabled = false;
        if (settingsPanel != null) settingsPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CloseSettings()
    {
        isOpen = false;
        Time.timeScale = 1f; // Tiếp tục game
        if (playerController != null) playerController.enabled = true;
        if (settingsPanel != null) settingsPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnSensitivityChanged(float value)
    {
        if (thirdPersonCamera != null)
            thirdPersonCamera.SetSensitivity(value);
        UpdateSensitivityText(value);
    }

    void UpdateSensitivityText(float value)
    {
        if (sensitivityValueText != null)
            sensitivityValueText.text = $"Độ nhạy chuột: {value:F1}";
    }
}
