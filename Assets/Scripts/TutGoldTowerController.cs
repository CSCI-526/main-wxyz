using UnityEngine;
public class TutGoldTowerController : TowerController
{
    public int goldPerCycle = 10; // 每次生产的金币数量
    public float generateInterval = 5f; // 生产金币的间隔时间

    private float lastGenerateTime;
    private TutGameManager tutgameManager;

    void Start()
    {
        base.Start();
        tutgameManager = FindObjectOfType<TutGameManager>();
        lastGenerateTime = Time.time;
    }

    void Update()
    {
        // Debug.Log("GoldTower Update Running...");
        GenerateGold();
    }

    void GenerateGold()
    {
        if (Time.time - lastGenerateTime >= generateInterval)
        {
            //Debug.Log("GoldTower Generating Gold...");
            if (tutgameManager != null)
            {
                tutgameManager.AddCoin(goldPerCycle);
                //Debug.Log("Gold Tower added " + goldPerCycle + " coins.");
            }
            else
            {
                Debug.LogError("GoldTowerController: gameManager is NULL!");
            }
            lastGenerateTime = Time.time;
        }
    }

    void OnDestroy()
    {
        if (tutgameManager != null)
        {
            int bonusGold = 60 * (int)Mathf.Pow(2, rankValue - 1);
            tutgameManager.AddCoin(bonusGold);
        }
    }
}