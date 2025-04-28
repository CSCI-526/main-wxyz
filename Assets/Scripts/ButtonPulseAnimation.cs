using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ButtonPulseAnimation : MonoBehaviour
{
    public float pulseSpeed = 2f;      
    public float pulseAmplitude = 0.1f;   
    
    private Vector3 originalScale;      
    private bool isPulsing = false;       

    public TutUIManager uiManager;

    public TextMeshProUGUI helpText;

    // public GameObject DragButtonPanel;

    // public GameObject ButtonPanel;

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
        
        if (helpText != null && helpText.text == "Buy your first tower!")
        {
            helpText.text = "Towers will generate at random positions on the board.";
        }
        if (helpText != null && helpText.text == "Let's buy another tower!")
        {
            // helpText.text = "USE ↑, ↓, →, ← keys to merge tower!";
            helpText.text = "Use <sprite name=\"WASDKeys\"> / <sprite name=\"ArrowKeys\"> to merge towers!";
        }
        if (helpText != null && helpText.text == "You have a Level 3 Tower!\nLet's buy and explore more towers!")
        {
            helpText.text = "";
        }
        if (helpText != null && helpText.text == "Let's buy a new tower!")
        {
            helpText.text = "";
        }
        if (uiManager != null && uiManager.isPaused)
            uiManager.TogglePauseGameNoPanel();
        else
            Debug.LogWarning("UIManager reference not set in ButtonPulseAnimation.");
    }
    
}
