using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutTimerManager : MonoBehaviour
{
    // UI Texts
    public TextMeshProUGUI timerText;   
    public TextMeshProUGUI helpText;    
    
    private float elapsedTime = 0f;     
    private bool isTimerRunning = true; 

    private bool helpTextUpdated = false; 
    private bool pulseTriggered = false;  
    private bool mergeTutorialTriggered = false;

    private bool rewardGold = false;


    private bool towerLevel3TutorialTriggered = false;

    public TutUIManager uiManager;
    public ButtonPulseAnimation buttonPulseAnimation;
    public TutGameManager gameManager;
    public BoardManager boardManager; 
    public Button buyButton;         
    public Button continueButton;    
    
    void Start()
    {
        if (timerText == null)
            Debug.LogError("TimerText is NULL! Assign it in the Inspector.");
        if (helpText == null)
            Debug.LogError("HelpText is NULL! Assign it in the Inspector.");


        if (continueButton != null)
            continueButton.gameObject.SetActive(false);
    }
    
    void Update()
    {
        if (!isTimerRunning) return;
        
        // Update elapsed time
        elapsedTime += Time.deltaTime;
        UpdateTimerUI();

        if (!helpTextUpdated && elapsedTime >= 3f)
        {
            helpText.text = "Buy your first tower";
            helpTextUpdated = true;
        }
        

        if (!pulseTriggered && elapsedTime >= 3f)
        {
            pulseTriggered = true;
            
            if (uiManager != null)
                uiManager.TogglePauseGameNoPanel();
            
            if (buttonPulseAnimation != null)
                buttonPulseAnimation.StartPulsing();
            
            if (gameManager != null)
                gameManager.AddCoin(10);
        }
        

        if (!mergeTutorialTriggered && gameManager != null && gameManager.playerGold >= 20)
        {
            mergeTutorialTriggered = true;
            
            if (uiManager != null)
                uiManager.TogglePauseGameNoPanel();
            
            if (buttonPulseAnimation != null)
                buttonPulseAnimation.StartPulsing();
            
            helpText.text = "Let's buy another tower";
        }


        if (!towerLevel3TutorialTriggered && boardManager != null)
        {
            if (HasLevel3Tower(boardManager))
            {
                towerLevel3TutorialTriggered = true;


                if (buyButton != null)
                    buyButton.gameObject.SetActive(false);

                if (continueButton != null)
                    continueButton.gameObject.SetActive(true);

                helpText.text = "You have a Level 3 Tower! Press Continue to proceed...";
            }
            if (HasLevel2Tower(boardManager))
            {

                helpText.text = "You have a level 2 tower! Keep buying more towers to merge and upgrade!";
                if (!rewardGold)
                {
                    gameManager.AddCoin(100);
                    rewardGold = true;
                }
                
            }
        }
    }
    

    private bool HasLevel3Tower(BoardManager board)
    {
        List<TowerController> towers = board.GetAllTowersOnBoard();
        foreach (var tower in towers)
        {
            if (tower.rankValue == 3)
            {
                return true;
            }
        }
        return false;
    }
    private bool HasLevel2Tower(BoardManager board)
    {
        List<TowerController> towers = board.GetAllTowersOnBoard();
        foreach (var tower in towers)
        {
            if (tower.rankValue == 2)
            {
                return true;
            }
        }
        return false;
    }

    // Update the timer UI text
    void UpdateTimerUI()
    {
        if (timerText == null)
            return;
        
        int hours = Mathf.FloorToInt(elapsedTime / 3600);
        int minutes = Mathf.FloorToInt((elapsedTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        
        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
    
    public void PauseTimer()
    {
        isTimerRunning = false;
    }

    public void ResumeTimer()
    {
        isTimerRunning = true;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        isTimerRunning = true;
        helpTextUpdated = false;
        pulseTriggered = false;
        mergeTutorialTriggered = false;
        towerLevel3TutorialTriggered = false;
        UpdateTimerUI();
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
    
    public void SaveFinalTime()
    {
        PlayerPrefs.SetFloat("FinalSurvivalTime", elapsedTime);
        PlayerPrefs.Save();
    }
}
