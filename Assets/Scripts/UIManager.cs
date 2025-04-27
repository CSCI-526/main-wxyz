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
    public bool isPaused = false;
    private bool isTimerRunning = true;//计时器是否运行
    public TextMeshProUGUI playerHealthText; //血量UI
    public Button buyButton; // 连接 Buy 按钮
    public TimerManager timerManager; // 连接 TimerManager
    public Image pauseButtonImage; // 连接按钮上的Image组件
    public Sprite pauseSprite;
    public Sprite playSprite;
    private bool wasPausedBeforeMenu;




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
        goldText.text = "x " + gameManager.playerGold;
        UpdateBuyButtonState(); //让按钮状态随金币更新
    }

    public void SetTowerCost(int newCost)
    {
        gameManager.ModifyCost(newCost);   // change the actual cost value
        UpdateTowerCostUI();               // refresh text
        UpdateBuyButtonState();            // refresh button interactivity
    }
    //更新塔价格UI
    public void UpdateTowerCostUI()
    {
        towerCostText.text = ": " + gameManager.spawnCost;
    }
    //购买塔
    public void OnBuyTowerClicked()
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
        Time.timeScale = isPaused ? 0 : 1;

        if (isPaused)
        {
            timerManager.PauseTimer();
            pauseButtonImage.sprite = playSprite;
        }
        else
        {
            timerManager.ResumeTimer();
            pauseButtonImage.sprite = pauseSprite;
        }
        UpdateBuyButtonState();
    }

    public void PauseGameOnly()
    {
        wasPausedBeforeMenu = isPaused; // 记录打开菜单前的状态
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0;
        timerManager.PauseTimer();
        UpdateBuyButtonState();
    }



    //继续游戏
    public void ContinueGame()
    {
        pausePanel.SetActive(false);

        if (wasPausedBeforeMenu)
        {
            // 如果菜单前本来就暂停的，继续暂停状态
            isPaused = true;
            Time.timeScale = 0;
            timerManager.PauseTimer();
            pauseButtonImage.sprite = playSprite;
        }
        else
        {
            // 如果菜单前是运行的，继续运行状态
            isPaused = false;
            Time.timeScale = 1;
            timerManager.ResumeTimer();
            pauseButtonImage.sprite = pauseSprite;
        }

        UpdateBuyButtonState();
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
        Time.timeScale = 1; //恢复游戏速度，防止主菜单被暂停
        SceneManager.LoadScene(0);
        //加载索引为0的MainMenu
    }
    public void UpdateHealthUI()
    {
        playerHealthText.text = " x " + gameManager.playerHealth;
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
        if (isPaused)
        {
            buyButton.interactable = false; // 暂停时禁用购买
            buyButton.image.color = new Color(0.5f, 0.5f, 0.5f, 1f); // 灰色
        }
        else if (gameManager.playerGold >= gameManager.spawnCost)
        {
            buyButton.interactable = true;
            buyButton.image.color = new Color(1f, 0.84f, 0f, 1f); // 金色
        }
        else
        {
            buyButton.interactable = false;
            buyButton.image.color = new Color(0.5f, 0.5f, 0.5f, 1f); // 灰色
        }
    }

}
