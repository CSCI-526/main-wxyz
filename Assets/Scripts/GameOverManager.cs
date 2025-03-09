using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
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
