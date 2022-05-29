
using UnityEngine;
using TMPro;

/// <summary>
/// FPS Display script
/// </summary>

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    private float deltaTime = 0.0f;

    private void Start()
    {
        if (fpsText == null)
            fpsText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} fps", fps);
        fpsText.text = text;
    }
}
