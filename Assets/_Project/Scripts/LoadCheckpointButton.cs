using UnityEngine;

public class LoadCheckpointButton : MonoBehaviour
{
    void Start()
    {
        var btn = GetComponent<UnityEngine.UI.Button>();
        if (btn != null)
        {
            var gui = FindFirstObjectByType<GameUI>();
            if (gui != null)
            {
                btn.onClick.AddListener(() => gui.LoadCheckpoint());
            }
        }
    }
}
