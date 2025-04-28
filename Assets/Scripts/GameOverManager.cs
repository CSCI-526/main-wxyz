using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Newtonsoft.Json;
using System.Collections.Generic;

public class GameOverManager : MonoBehaviour
{
    public TextMeshProUGUI survivalTimeText; // 存活时间 UI
    public FirebaseManager FirebaseManager;
    public TextMeshProUGUI[] topRankTexts;  //绑定前3名的Text组件


    private string playerName = "Steve";
    private List<RankData> rankList = new List<RankData>();
    
    void Start()
    {
        // 显示存活时间
        float finalTime = PlayerPrefs.GetFloat("FinalSurvivalTime", 0f);
        int hours = Mathf.FloorToInt(finalTime / 3600);
        int minutes = Mathf.FloorToInt((finalTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(finalTime % 60);
        int milliseconds = Mathf.FloorToInt((finalTime * 1000) % 1000); // 🔥 补上这一句！

        survivalTimeText.text = $"You survived {minutes:00}:{seconds:00}:{milliseconds:000}";

        // 获取玩家排名
        float finalScore = PlayerPrefs.GetFloat("FinalScore", 0f);
        
        GetRanking(playerName, finalScore, finalTime);
    }


    private void GetRanking(string name, float finalScore, float finalTime)
    {
        string jsonString = PlayerPrefs.GetString("RankList","");
        Debug.Log("Original rank list: " + jsonString);
        if (!string.IsNullOrEmpty(jsonString) && jsonString != "null")
        {
            rankList = JsonConvert.DeserializeObject<List<RankData>>(jsonString);
        }
        RankData newRank = new RankData(name, finalScore, finalTime);
        rankList.Add(newRank);
        rankList.Sort((x, y) => y.surviveTime.CompareTo(x.surviveTime));
        if (rankList.Count > 3) rankList.RemoveAt(rankList.Count - 1);
        string updatedJson = JsonConvert.SerializeObject(rankList);
        Debug.Log("Rank list: " + updatedJson);
        FirebaseManager.RepalceData(updatedJson);
        UpdateRankUI(); 
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
    private void UpdateRankUI()
    {
        for (int i = 0; i < topRankTexts.Length; i++)
        {
            if (i < rankList.Count)
            {
                RankData data = rankList[i];
                int minutes = Mathf.FloorToInt(data.surviveTime / 60f);
                int seconds = Mathf.FloorToInt(data.surviveTime % 60f);
                int milliseconds = Mathf.FloorToInt((data.surviveTime * 1000) % 1000);
                topRankTexts[i].text = $"Rank {i + 1}   {minutes:00}: {seconds:00}: {milliseconds:000}";

            }
            else
            {
                topRankTexts[i].text = $"Rank {i + 1}: ---";
            }
        }
    }




}



[System.Serializable]
public class RankData
{
    public string name;
    public float score;
    public float surviveTime;

    public RankData(string name, float score, float surviveTime)
    {
        this.name = name;
        this.score = score;
        this.surviveTime = surviveTime;
    }
}
