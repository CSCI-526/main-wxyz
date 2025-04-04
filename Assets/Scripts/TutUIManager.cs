using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TutUIManager : MonoBehaviour
{
    public TutGameManager gameManager; //连接 GameManager   
    public TextMeshProUGUI goldText;  //金币UI 
    public TextMeshProUGUI towerCostText; //购买塔的价格UI
    public TextMeshProUGUI goldWarningText; //金币不足的提示文本
    public TextMeshProUGUI timerText; //计时器UI
    public GameObject pausePanel; //暂停窗口  
    public bool isPaused =false; 
    private bool isTimerRunning = true;//计时器是否运行
    public TextMeshProUGUI playerHealthText; //血量UI
    public Button buyButton; // 连接 Buy 按钮
    public TutTimerManager timerManager; // 连接 TimerManager


    void Start()
    {
        UpdateGoldUI();
        UpdateTowerCostUI(); //初始化塔价格显示
        pausePanel.SetActive(false);                 //默认隐藏暂停窗口
        UpdateHealthUI(); //初始化血量UI
    }
    //更新金币UI
    public void UpdateGoldUI()
    {
        goldText.text = "Gold: " + gameManager.playerGold;
        UpdateBuyButtonState(); //让按钮状态随金币更新
    }
    //更新塔价格UI
    public void UpdateTowerCostUI()
    {
        towerCostText.text = ": " + gameManager.spawnCost;
    }
    //购买塔
    /*public void OnBuyTowerClicked()
    {
        if (!buyButton.interactable) return; //如果按钮是灰色的，直接返回（不执行）

        if (gameManager.SpawnRandomTower()) //成功生成塔
        {
            gameManager.DeductCost();
            UpdateGoldUI();
            UpdateTowerCostUI();
            UpdateBuyButtonState(); //购买后更新按钮状态
        }
        else
        {
            Debug.Log("Board is full. Cannot buy tower.");
        }
    }*/

    public void OnBuyTowerClicked()
    {
        if (!buyButton.interactable) return;

        if (timerManager != null)
        {
            if (timerManager.IsInTutorialPhase())
            {
                bool success = gameManager.SpawnSpecificTower("Cannon");
                if (success)
                {
                    gameManager.DeductCost();
                    UpdateGoldUI();
                    UpdateTowerCostUI();
                    UpdateBuyButtonState();
                }
                return;
            }
            else if (timerManager.IsBurningTowerPhase())
            {
                bool success = gameManager.SpawnSpecificTower("TutBurningTower");
                if (success)
                {
                    gameManager.DeductCost();
                    UpdateGoldUI();
                    UpdateTowerCostUI();
                    UpdateBuyButtonState();
                }
                return;
            }
            else if (timerManager.IsEnergyTowerPhase())
            {
                bool success = gameManager.SpawnSpecificTower("TutEnergyTower");
                if (success)
                {
                    gameManager.DeductCost();
                    UpdateGoldUI();
                    UpdateTowerCostUI();
                    UpdateBuyButtonState();
                }
                return;
<<<<<<< Updated upstream
            
            }
=======
            }

>>>>>>> Stashed changes
        }

        if (gameManager.SpawnRandomTower())
        {
            gameManager.DeductCost();
            UpdateGoldUI();
            UpdateTowerCostUI();
            UpdateBuyButtonState();
        }
        else
        {
            Debug.Log("Board is full. Cannot buy tower.");
        }
    }

    //让"金币不足"提示2秒后消失
    IEnumerator HideGoldWarning()
    {
        yield return new WaitForSeconds(2f);
        goldWarningText.gameObject.SetActive(false);
    }
    //暂停游戏
    public void TogglePauseGame()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;

        if (isPaused)
            timerManager.PauseTimer();  // **暂停计时器**
        else
            timerManager.ResumeTimer(); // **继续计时**
    }

    public void TogglePauseGameNoPanel()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;

        if (isPaused)
            timerManager.PauseTimer();  // **暂停计时器**
        else
            timerManager.ResumeTimer(); // **继续计时**
    }

    //继续游戏
    public void ContinueGame()
    {
        TogglePauseGame();
    }


  //退出游戏
    public void QuitGame()
    {
        Debug.Log("退出游戏...");
        Application.Quit();
    }
    public void ReturnToMenu()
    {
        Debug.Log("返回主菜单...");
        Time.timeScale =1; //恢复游戏速度，防止主菜单被暂停
        SceneManager.LoadScene(0);
        //加载索引为0的MainMenu
    }
    public void UpdateHealthUI()
    {
        playerHealthText.text = "Lives Remaining: " + gameManager.playerHealth;
    }
    public void RestartGame()
    {
        Debug.Log("Restarting Game...");
        Time.timeScale = 1; // 确保时间正常
        GameManager.Instance.SendDataFirebase();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // 重新加载当前关卡
    }
    public void UpdateBuyButtonState()
    {
        //金币足够且棋盘未满
        if (gameManager.playerGold >= gameManager.spawnCost)
        {
            buyButton.interactable = true; //按钮可点击
            buyButton.image.color = new Color(1f, 0.84f, 0f, 1f); //金色
        }
        else //金币不足 或者 棋盘满了
        {
            buyButton.interactable = false; //按钮禁用
            buyButton.image.color = new Color(0.5f, 0.5f, 0.5f, 1f); //灰色
        }
    }
    
}
