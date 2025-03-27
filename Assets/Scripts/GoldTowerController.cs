using UnityEngine;
public class GoldTowerController : TowerController
{
    public int goldPerCycle = 5; // 每次生产的金币数量
    public float generateInterval = 5f; // 生产金币的间隔时间

    private float lastGenerateTime;
    private GameManager gameManager;

    void Start()
    {
        base.Start();
        gameManager = FindObjectOfType<GameManager>(); // 获取游戏管理器
        lastGenerateTime = Time.time; // 记录初始时间
    }

    void Update()
    {
        //Debug.Log("GoldTower Update Running...");
        GenerateGold();
    }

    void GenerateGold()
    {
        if (Time.time - lastGenerateTime >= generateInterval)
        {
            //Debug.Log("GoldTower Generating Gold...");
            if (gameManager != null)
            {
                gameManager.AddCoin(goldPerCycle);
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
        if (gameManager != null)
        {
            int bonusGold = 60 * (int)Mathf.Pow(2, rankValue - 1);
            gameManager.AddCoin(bonusGold);
        }
    }
}
