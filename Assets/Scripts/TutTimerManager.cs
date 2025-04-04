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

    private bool cannonTowerUIPopped = false;
    private bool towerLevel3TutorialTriggered = false;
    private bool frozenTowerTutorialTriggered = false;
    private bool frozenTowerTutorialcomplete = false;

    //是否触发了燃烧塔教学阶段
    private bool burningTowerTutorialcomplete = false;
    private bool burningTowerTutorialTriggered = false;

    //是否触发了能量塔教学阶段
    private bool energyTowerTutorialcomplete = false;
    private bool energyTowerTutorialTriggered = false;

    //是否触发了金币塔教学阶段
    private bool goldTowerTutorialTriggered = false;
    private bool goldTowerTutorialcomplete = false;
    private bool goldTowerUIPopped = false;

    public GameObject goldTowerPanel;

    public GameObject cannonTowerPanel;

    public GameObject frozenTowerPanel;
    private bool frozenTowerUIPopped = false;

    public GameObject burningTowerPanel;  // 拖拽你的 TowerPanel 进来
    private bool burningTowerUIPopped = false;  // 防止重复弹窗

    public GameObject energyTowerPanel;  
    private bool energyTowerUIPopped = false;





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

        if (!helpTextUpdated && elapsedTime >= 5f)
        {
            helpText.text = "Buy your first tower";
            helpTextUpdated = true;
        }

        if (!cannonTowerUIPopped && HasCannonTower(boardManager))
        {
            cannonTowerUIPopped = true;
            

            if (cannonTowerPanel != null)
                cannonTowerPanel.SetActive(true);
            
            if (uiManager != null)
                uiManager.TogglePauseGameNoPanel();
        }
        

        if (!pulseTriggered && elapsedTime >= 5f)
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
            TutGameManager.Instance.setSpawnFlag(false);
        }

        if (!towerLevel3TutorialTriggered && boardManager != null)
        {
            if (HasLevel3Tower(boardManager))
            {
                towerLevel3TutorialTriggered = true;


                /*if (buyButton != null)
                    buyButton.gameObject.SetActive(false);*/

                /*if (continueButton != null)
                    continueButton.gameObject.SetActive(true);*/

                /*helpText.text = "You have a Level 3 Tower! Press Continue to proceed...";*/
            }
            if (HasLevel2Tower(boardManager))
            {

                helpText.text = "You have a level 2 tower! Keep buying and merging to get a level 3 tower!";
                if (!rewardGold)
                {
                    gameManager.AddCoin(50);
                    rewardGold = true;
                    TutGameManager.Instance.setSpawnFlag(true);
                }

                if(gameManager.playerGold >= 40)
                {
                    if (buttonPulseAnimation != null)
                        buttonPulseAnimation.StartPulsing();
                }
                
            }
        }

        if (towerLevel3TutorialTriggered && !frozenTowerTutorialTriggered)
        {
            frozenTowerTutorialTriggered = true;

            helpText.text = "You have a Level 3 Tower! Now let's buy a new tower: Frozen Tower!";

            TutGameManager.Instance.setSpawnFlag(true);
            
            if (uiManager != null)
                uiManager.TogglePauseGameNoPanel();
            
            if (buttonPulseAnimation != null)
                buttonPulseAnimation.StartPulsing();
            
            if (gameManager != null)
                gameManager.AddCoin(50);
        }
        // 当检测到 Frozen Tower 出现时，弹出相应的面板（防止重复）
        if (!frozenTowerUIPopped && HasFrozenTower(boardManager))
        {
            frozenTowerUIPopped = true;

            if (frozenTowerPanel != null)
                frozenTowerPanel.SetActive(true);
            
            if (uiManager != null)
                uiManager.TogglePauseGameNoPanel();
        }

        // 添加新的教学阶段：引导生成燃烧塔
        if (towerLevel3TutorialTriggered && !burningTowerTutorialTriggered && !frozenTowerPanel.activeSelf && HasFrozenTower(boardManager) && gameManager.playerGold >= 25)
        {
            burningTowerTutorialTriggered = true;
            frozenTowerTutorialcomplete = true;

            TutGameManager.Instance.setSpawnFlag(true);

            helpText.text = "Now let's buy a new tower: Burning Tower! ";

            if (uiManager != null)
                uiManager.TogglePauseGameNoPanel();

            if (buttonPulseAnimation != null)
                buttonPulseAnimation.StartPulsing();

            if (gameManager != null)
                gameManager.AddCoin(35);
        }
        // 首次出现 Burning Tower 时弹出 UI(先注释掉)
        if (!burningTowerUIPopped && HasBurningTower(boardManager))
        {
            burningTowerUIPopped = true;

            if (burningTowerPanel != null)
                burningTowerPanel.SetActive(true);
            
            if (uiManager != null)
                uiManager.TogglePauseGameNoPanel();

        }

        //能量塔生成
        if (towerLevel3TutorialTriggered && !energyTowerTutorialTriggered && !burningTowerPanel.activeSelf && HasBurningTower(boardManager) && gameManager.playerGold >= 40)
        {
            energyTowerTutorialTriggered = true;
            burningTowerTutorialcomplete = true;

            TutGameManager.Instance.setSpawnFlag(true);

            helpText.text = "How about buy a Energy Tower! ";

            if (uiManager != null)
                uiManager.TogglePauseGameNoPanel();

            if (buttonPulseAnimation != null)
                buttonPulseAnimation.StartPulsing();

            if (gameManager != null)
                gameManager.AddCoin(30);
        }
        
        if (!energyTowerUIPopped && HasEnergyTower(boardManager))
        {
            energyTowerUIPopped = true;

            if (energyTowerPanel != null)
                energyTowerPanel.SetActive(true);

            if (uiManager != null)
                uiManager.TogglePauseGameNoPanel();

        }

        //金币塔生成

        if (towerLevel3TutorialTriggered && !goldTowerTutorialTriggered && !energyTowerPanel.activeSelf && HasEnergyTower(boardManager) && gameManager.playerGold >= 45)
        {
            goldTowerTutorialTriggered = true;
            energyTowerTutorialcomplete = true;
            helpText.text = "Now let's try the Gold Tower! It generates gold over time!";
            TutGameManager.Instance.setSpawnFlag(true);

            if (uiManager != null)
                uiManager.TogglePauseGameNoPanel();

            if (buttonPulseAnimation != null)
                buttonPulseAnimation.StartPulsing();

            if (gameManager != null)
                gameManager.AddCoin(35);
        }
        if (!goldTowerUIPopped && HasGoldTower(boardManager))
        {
            goldTowerUIPopped = true;
            if (goldTowerPanel != null)
                goldTowerPanel.SetActive(true);

            if (uiManager != null)
                uiManager.TogglePauseGameNoPanel();
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

    //操作教学
    public bool IsInTutorialPhase()
    {
        return !towerLevel3TutorialTriggered;
    }

    public bool IsFrozenTowerPhase()
    {
        return !frozenTowerTutorialcomplete;
    }

    //燃烧塔教学
    public bool IsBurningTowerPhase()
    {
        return !burningTowerTutorialcomplete;
    }
    //能量塔教学

    public bool IsEnergyTowerPhase()
    {
        return !energyTowerTutorialcomplete;
    }


    public bool IsGoldTowerPhase()
    {
        return !goldTowerTutorialcomplete;
    }

    private bool HasCannonTower(BoardManager board)
    {
        List<TowerController> towers = board.GetAllTowersOnBoard();
        foreach (var tower in towers)
        {
            if (tower != null && tower.towerName.Contains("Tank"))
            {
                return true;
            }
        }
        return false;
    }

    // 检测 Frozen Tower 是否存在
    private bool HasFrozenTower(BoardManager board)
    {
        List<TowerController> towers = board.GetAllTowersOnBoard();
        foreach (var tower in towers)
        {
            if (tower != null && tower.towerName.Contains("TutFrozenTower"))
            {
                return true;
            }
        }
        return false;
    }
    private bool HasBurningTower(BoardManager board)
    {
        List<TowerController> towers = board.GetAllTowersOnBoard();
        foreach (var tower in towers)
        {
            if (tower != null && tower.towerName.Contains("TutBurningTower"))
            {
                return true;
            }
        }
        return false;
    }

    private bool HasEnergyTower(BoardManager board)
    {
        List<TowerController> towers = board.GetAllTowersOnBoard();
        foreach (var tower in towers)
        {
            if (tower != null && tower.towerName.Contains("TutEnergyTower"))
            {
                return true;
            }
        }
        return false;
    }

    private bool HasGoldTower(BoardManager board)
    {
        List<TowerController> towers = board.GetAllTowersOnBoard();
        foreach (var tower in towers)
        {
            if (tower != null && tower.towerName.Contains("TutGoldTower")) 
            {
                return true;
            }
        }
        return false;
    }


    public void OnContinueFromPanel()
    {
        if (cannonTowerPanel != null)
            cannonTowerPanel.SetActive(false);

        if (frozenTowerPanel != null)
            frozenTowerPanel.SetActive(false);

        // 关闭 Panel
        if (burningTowerPanel != null)
            burningTowerPanel.SetActive(false);

        if (energyTowerPanel != null)
            energyTowerPanel.SetActive(false);

        if (goldTowerPanel != null)
            goldTowerPanel.SetActive(false);


        // 恢复游戏
        if (uiManager != null)
            uiManager.TogglePauseGameNoPanel();
    }
}