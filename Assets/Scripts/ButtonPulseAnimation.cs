using UnityEngine;
using TMPro;

public class ButtonPulseAnimation : MonoBehaviour
{
    public float pulseSpeed = 2f;      
    public float pulseAmplitude = 0.1f;   
    
    private Vector3 originalScale;      
    private bool isPulsing = false;       

    public TutUIManager uiManager;

    public TextMeshProUGUI helpText;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (isPulsing)
        {
            float scaleOffset = Mathf.Sin(Time.unscaledTime * pulseSpeed) * pulseAmplitude;
            transform.localScale = originalScale * (1 + scaleOffset);
        }
    }

    public void StartPulsing()
    {
        isPulsing = true;
    }

    public void StopPulsing()
    {
        isPulsing = false;
        transform.localScale = originalScale;
    }

    public void OnButtonClick()
    {
        StopPulsing();
        
        if (helpText != null && helpText.text == "Buy your first tower")
        {
            helpText.text = "Tank Tower will shoot the first enemy";
        }
        if (helpText != null && helpText.text == "Let's buy another tower")
        {
            helpText.text = "USE ↑ ,↓,→,← keys to merge tower";
        }

        if (uiManager != null && uiManager.isPaused)
            uiManager.TogglePauseGameNoPanel();
        else
            Debug.LogWarning("UIManager reference not set in ButtonPulseAnimation.");
    }
    public void TransistToNewScene()
    {
        Debug.Log("Jump to next level");
    }
}
