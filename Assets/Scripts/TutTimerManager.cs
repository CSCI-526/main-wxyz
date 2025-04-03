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
    //是否触发了燃烧塔教学阶段
    private bool burningTowerTutorialTriggered = false;
    //是否触发了能量塔教学阶段
    private bool energyTowerTutorialTriggered = false;

    public GameObject burningTowerPanel;  // 拖拽你的 TowerPanel 进来
    private bool burningTowerUIPopped = false;  // 防止重复弹窗




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


                /*if (buyButton != null)
                    buyButton.gameObject.SetActive(false);*/

                /*if (continueButton != null)
                    continueButton.gameObject.SetActive(true);*/

                /*helpText.text = "You have a Level 3 Tower! Press Continue to proceed...";*/
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
        // 添加新的教学阶段：引导生成燃烧塔
        if (towerLevel3TutorialTriggered && !burningTowerTutorialTriggered)
        {
            burningTowerTutorialTriggered = true;

            helpText.text = "You have a Level 3 Tower! Now let's buy a new tower:Burning Tower! ";

            if (uiManager != null)
                uiManager.TogglePauseGameNoPanel();

            if (buttonPulseAnimation != null)
                buttonPulseAnimation.StartPulsing();

            if (gameManager != null)
                gameManager.AddCoin(100);
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

        // Energy Tower 教学触发条件
        if (burningTowerUIPopped && !energyTowerTutorialTriggered && ShouldTriggerEnergyTowerTutorial())
        {
            energyTowerTutorialTriggered = true;
            burningTowerTutorialTriggered = false; //关闭燃烧塔教学的控制权


            helpText.text = "Did you notice some enemies are really hard to kill?";
            
            if (uiManager != null) 
                uiManager.TogglePauseGameNoPanel();
            if (buttonPulseAnimation != null) 
                buttonPulseAnimation.StartPulsing();
            if (gameManager != null) 
                gameManager.AddCoin(100);
            StartCoroutine(DelayedEnergyTowerTip());
        }


    }

    private System.Collections.IEnumerator DelayedEnergyTowerTip()

    {
        yield return new WaitForSeconds(3f);
        helpText.text = "Now it's time to introduce the Energy Tower!";
    }
    private bool ShouldTriggerEnergyTowerTutorial()
    {
        bool hasLevel2Burning = false;
        bool hasThickEnemy = false;

        List<TowerController> towers = boardManager.GetAllTowersOnBoard();
        foreach (var tower in towers)
        {
            if (tower != null && tower.towerName.Contains("TutBurningTower") && tower.rankValue >= 2)
            {
                hasLevel2Burning = true;
                break;
            }
        }

        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.IsAlive && enemy.enemyData != null && enemy.enemyData.maxHealth >= 100)
            {
                hasThickEnemy = true;
                break;
            }
        }

        return hasLevel2Burning && hasThickEnemy;
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

    //燃烧塔教学
    public bool IsBurningTowerPhase()
    {
        return burningTowerTutorialTriggered;
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

    //能量塔教学
    public bool IsEnergyTowerPhase()
    {
        return energyTowerTutorialTriggered;
    }

    public void OnContinueFromPanel()
    {
        // 关闭 Panel
        if (burningTowerPanel != null)
            burningTowerPanel.SetActive(false);

        // 恢复游戏
        if (uiManager != null)
            uiManager.TogglePauseGameNoPanel();
    }



}
