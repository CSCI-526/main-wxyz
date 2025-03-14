using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public TextMeshProUGUI timerText; //计时器UI文本
    private float elapsedTime = 0f; //记录经过的时间（秒）
    private bool isTimerRunning = true; //计时器是否在运行
  
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
    //更新计时器 UI
    void UpdateTimerUI()
    {
        if (timerText == null)
        {
            return;
        }

        int hours = Mathf.FloorToInt(elapsedTime / 3600);
        int minutes = Mathf.FloorToInt((elapsedTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        
        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
    

      

    //暂停计时器
    public void PauseTimer()
    {
        isTimerRunning = false;
    }
    //继续计时器
    public void ResumeTimer()
    {
        isTimerRunning = true;
    }

    //重新启动计时器
    public void ResetTimer()
    {
        elapsedTime = 0f;
        isTimerRunning = true; //确保重新开始时计时器继续
        UpdateTimerUI();
    }
    public float GetElapsedTime()
    {
        return elapsedTime;
    }     //存储存活时间
    public void SaveFinalTime()
    {
        PlayerPrefs.SetFloat("FinalSurvivalTime", elapsedTime);
        PlayerPrefs.Save();
    }

}
