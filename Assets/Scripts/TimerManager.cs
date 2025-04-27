using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public TextMeshProUGUI timerText; // 计时器UI文本
    private float elapsedTime = 0f; // 记录经过的时间（秒）
    private bool isTimerRunning = true; // 计时器是否在运行

    void Start()
    {
        if (timerText == null)
        {
            Debug.LogError("TimerText is NULL! Make sure it's assigned in the Inspector.");
        }
    }

    void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    // 更新计时器 UI
    void UpdateTimerUI()
    {
        if (timerText == null)
        {
            return;
        }

        int minutes = Mathf.FloorToInt((elapsedTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 1000) % 1000);

        timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }


    // 暂停计时器
    public void PauseTimer()
    {
        isTimerRunning = false;
    }

    // 继续计时器
    public void ResumeTimer()
    {
        isTimerRunning = true;
    }

    // 重新启动计时器
    public void ResetTimer()
    {
        elapsedTime = 0f;
        isTimerRunning = true;
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
