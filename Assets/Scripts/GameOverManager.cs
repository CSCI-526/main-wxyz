using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public TextMeshProUGUI survivalTimeText; // 存活时间 UI

    void Start()
    {
        // 读取存活时间
        float finalTime = PlayerPrefs.GetFloat("FinalSurvivalTime", 0f);
        int hours = Mathf.FloorToInt(finalTime / 3600);
        int minutes = Mathf.FloorToInt((finalTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(finalTime % 60);

        // 显示存活时间
        survivalTimeText.text = $"You survived {hours:00}:{minutes:00}:{seconds:00}";
    }
    
    public void RestartGame()
    {
        Debug.Log("Restarting Game...");
        Time.timeScale =1;//确保时间正常
        SceneManager.LoadScene(1); //重新加载游戏场景
        TimerManager timerManager = FindObjectOfType<TimerManager>();
        if (timerManager != null)
        {
            timerManager.ResetTimer();
        }
        else
        {
            Debug.LogError("TimerManager is NULL! Timer not reset.");
        }
    }
    public void ReturnToMenu()
    {
        Debug.Log("Returning to Main Menu...");
        Time.timeScale = 1; // 确保时间恢复正常
        SceneManager.LoadScene(0);
    }
}
