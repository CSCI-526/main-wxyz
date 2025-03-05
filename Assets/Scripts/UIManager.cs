using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager; //连接 GameManager   
    public TextMeshProUGUI goldText;  //金币UI 
    public TextMeshProUGUI towerCostText; //购买塔的价格UI
    public TextMeshProUGUI goldWarningText; //金币不足的提示文本
    public TextMeshProUGUI timerText; //计时器UI
    public GameObject pausePanel; //暂停窗口  
    public bool isPaused =false; 
    private float elapsedTime = 0f; //记录游戏时间（秒）
    private bool isTimerRunning = true;//计时器是否运行
    public TextMeshProUGUI playerHealthText; //血量UI
    public GameObject gameOverPanel;

    void Start()
    {
        gameOverPanel.SetActive(false); //开始时隐藏GameOver界面
        UpdateGoldUI();
        UpdateTowerCostUI(); //初始化塔价格显示
        goldWarningText.gameObject.SetActive(false); //默认隐藏警告文本
        pausePanel.SetActive(false);                 //默认隐藏暂停窗口
        StartCoroutine(UpdateTimer());               //启动计时器
        UpdateHealthUI(); //初始化血量UI
    }
    IEnumerator UpdateTimer()
    {
        while (true)
        {
            if (isTimerRunning)
            {
                elapsedTime += Time.deltaTime;
                UpdateTimerUI();
            }
            yield return null;
        }
    }
    //更新计时器UI
    void UpdateTimerUI()
    {
        int hours = Mathf.FloorToInt(elapsedTime / 3600);
        int minutes = Mathf.FloorToInt((elapsedTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
    //更新金币UI
    public void UpdateGoldUI()
    {
        goldText.text = "Gold: " + gameManager.playerGold;
    }
    //更新塔价格UI
    public void UpdateTowerCostUI()
    {
        towerCostText.text = "NewTowerCost: " + gameManager.spawnCost;
    }
    //购买塔
    public void OnBuyTowerClicked()
    {
        if (gameManager.SpawnRandomTower()) //成功生成塔
        {
            gameManager.DeductCost();
            UpdateGoldUI();
            UpdateTowerCostUI();   //更新塔价格UI
        }
        else
        {
            ShowGoldWarning(); //显示金币不足提示
        }
    }

    //显示金币不足提示
    public void ShowGoldWarning()
    {
        goldWarningText.gameObject.SetActive(true);
        StopCoroutine("HideGoldWarning");
        StartCoroutine(HideGoldWarning());
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
        isPaused =!isPaused;
        isTimerRunning =!isPaused; //计时器暂停
        pausePanel.SetActive(isPaused); //显示/隐藏暂停窗口
        Time.timeScale = isPaused ? 0 : 1; //暂停/恢复游戏
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
    public void ShowGameOverUI()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0; //停止游戏
    }

    
}
