using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public BoardManager boardManager;
    public EnemySpawn enemyManager;
    public List<GameObject> towerPrefabs; // Changed to a list of tower prefabs
    public UIManager uiManager; // UIManager connection
    public TimerManager timerManager; //连接 TimerManager
    public FirebaseManager FirebaseManager;

    public int playerGold = 10;
    public int spawnCost = 10;
    public int playerHealth = 100;

    private bool hasLost = false;

    /*** analytic ***/
    private float damageFromCanonTower = 0f;
    private float damageFromBurningTower = 0f;
    private float damageFromSlowTower = 0f;
    private float damageFromEnergyTower = 0f;
    private List<TowersOnTile> towersOnTileList = new List<TowersOnTile>();
    // private int CanonTowerNum = 0;
    // private int BurningTowerNum = 0;
    // private int SlowTowerNum = 0;
    // private int EnergyTowerNum = 0;
    // private int GoldTowerNum = 0;
    private float score = 0f;
    private float mergeCount = 0f;
    private float changeColorCount = 0f;
    /*** analytic ***/
    public AudioSource audioSource; // 连接AudioSource
    public AudioClip healthDecreaseClip; // 连接减少血量的音效


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Time.timeScale = 1f;
        boardManager.CreateGrid();
        bool spawned = SpawnRandomTower();
        enemyManager.EnemySpawnConfigInit();
        StartCoroutine(enemyManager.SpawnWaves());
        uiManager.UpdateHealthUI(); // Update health UI at game start
        FirebaseManager.ReadData();
        StartCoroutine(RecordTowersOnTile());
        // string json = "{ \"BetaRankList\": { \"1\": { \"score\": 10000, \"surviveTime\": 30 }, \"2\": { \"score\": 5000, \"surviveTime\": 20 } } }";
        // PlayerPrefs.SetString("RankList", json);
        // PlayerPrefs.Save();
        // FirebaseManager.SaveData(); // 存储数据
    }

    void Update()
    {
        if (playerHealth <= 0)
        {
            Time.timeScale = 0f;
            SendDataFirebase();
            ShowFailScreen();
            Time.timeScale = 0f;
        }
    }

    public void SendDataFirebase()
    {

        float playerTime = timerManager.GetElapsedTime();
        // TileController[,] tiles = boardManager.tiles;
        // for (int x = 0; x < tiles.GetLength(0); x++)
        // {
        //     for (int y = 0; y < tiles.GetLength(1); y++)
        //     {
        //         TileController tile = tiles[x, y];
        //         if (tile.towerOnTile != null)
        //         {
        //             Debug.Log(tile.towerOnTile.towerName);
        //             switch (tile.towerOnTile.towerName)
        //             {
        //                 case "Gold":
        //                     GoldTowerNum++;
        //                     break;
        //                 case "Canon":
        //                     CanonTowerNum++;
        //                     break;
        //                 case "Burn":
        //                     BurningTowerNum++;
        //                     break;
        //                 case "Energy":
        //                     EnergyTowerNum++;
        //                     break;
        //                 case "Frozen":
        //                     SlowTowerNum++;
        //                     break;
        //                 default:
        //                     CanonTowerNum++;
        //                     break;
        //             }
        //         }
        //     }
        // }

        // Debug.Log(
        //     "PlayerTime: " + playerTime +
        //     "s score: " + score +
        //     " DamageFromCanonTower: " + damageFromCanonTower+ 
        //     " DamageFromBurningTower: " + damageFromBurningTower +
        //     " DamageFromFrozenTower: " + damageFromSlowTower +
        //     " DamageFromEnergyTower: " + damageFromEnergyTower +
        //     " GoldTower number: " + GoldTowerNum +
        //     " mergeCount: " + mergeCount +
        //     " changeColorCount: " + changeColorCount
        // );

        // Debug.Log("tower recording every minute:");
        // foreach (TowersOnTile towersOnTile in towersOnTileList)
        // {
        //     Debug.Log(
        //         "CanonTowerNum: " + towersOnTile.canonTowerNum + 
        //         " goldTowerNum: " + towersOnTile.goldTowerNum + 
        //         " burningTowerNum: " + towersOnTile.burningTowerNum + 
        //         " frozenTowerNum: " + towersOnTile.frozenTowerNum + 
        //         " energyTowerNum: " + towersOnTile.energyTowerNum);

        // }

        var data = new
        {
            PlayerTime = playerTime,
            Score = score,
            DamageFromCanonTower = damageFromCanonTower,
            DamageFromBurningTower = damageFromBurningTower,
            DamageFromFrozenTower = damageFromSlowTower,
            DamageFromEnergyTower = damageFromEnergyTower,
            MergeCount = mergeCount,
            ChangeColorCount = changeColorCount,
            TowersOnTileList = towersOnTileList
        };

        string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
        Debug.Log("Data sent to Firebase: " + jsonString);
        FirebaseManager.SaveData(jsonString);

        // string json = $"\{\"playerTime\":{playerTime},\"score\":{score},\"damageFromCanonTower\":{damageFromCanonTower},\"damageFromBurningTower\":{damageFromBurningTower},\"damageFromSlowTower\":{damageFromSlowTower},\"mergeCount\":{mergeCount}\}";
        // string json = string.Format(
        //     "{{\"playerTime\": \"{0}\", \"score\": {1}, \"damageFromCanonTower\":{2}, \"damageFromBurningTower\":{3}, \"damageFromSlowTower\":{4}, \"damageFromEnergyTower\":{5}, \"mergeCount\":{6}, \"changeColorCount\":{7}}}", 
        //     playerTime, score, damageFromCanonTower, damageFromBurningTower, damageFromSlowTower, 
        //     damageFromEnergyTower, mergeCount, changeColorCount
        // );
        // FirebaseManager.SaveData(json);
        // FirebaseManager.SaveData(
        //     playerTime, 
        //     score, 
        //     damageFromCanonTower, damageFromBurningTower, damageFromSlowTower,
        //     mergeCount
        // );
    }

    private void ShowFailScreen()
    {
        if (!hasLost)
        {
            Debug.Log("Player lose !!!");
            hasLost = true;

            //存储最终存活时间  
            if (timerManager != null)
            {
                timerManager.SaveFinalTime();
            }
            else
            {
                Debug.LogError("TimerManager is NULL! Time not saved.");
            }

            PlayerPrefs.SetFloat("FinalScore", score);
            PlayerPrefs.Save();

            StartCoroutine(DelayedLoadGameOver());
        }
    }

    private IEnumerator DelayedLoadGameOver()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        SceneManager.LoadScene(2);
    }


    public bool SpawnRandomTower()
    {
        if (playerGold < spawnCost)
        {
            return false;
        }
        List<Vector2Int> availableTiles = new List<Vector2Int>();
        for (int row = 1; row < boardManager.rows - 1; row++)
        {
            for (int col = 1; col < boardManager.columns - 1; col++)
            {
                if (boardManager.tiles[row, col].towerOnTile == null)
                {
                    availableTiles.Add(new Vector2Int(col, row));
                }
            }
        }
        if (availableTiles.Count == 0)
        {
            Debug.Log("No available tiles to spawn a tower.");
            return false;
        }

        Vector2Int chosenTile = availableTiles[Random.Range(0, availableTiles.Count)];
        int randomCol = chosenTile.x;
        int randomRow = chosenTile.y;

        float spacing = boardManager.tileSpacing;
        float offsetX = (boardManager.columns - 1) / 2.0f;
        float offsetY = (boardManager.rows - 1) / 2.0f;
        Vector3 spawnPos = new Vector3((randomCol - offsetX) * spacing, (offsetY - randomRow) * spacing, 0);

        if (towerPrefabs == null || towerPrefabs.Count == 0)
        {
            Debug.LogError("No tower prefabs assigned in GameManager.");
            return false;
        }

        int randomIndex = Random.Range(0, towerPrefabs.Count);
        GameObject towerObj = Instantiate(towerPrefabs[randomIndex], spawnPos, Quaternion.identity);
        TowerController towerController = towerObj.GetComponent<TowerController>();
        towerController.gridPosition = new Vector2Int(randomCol, randomRow);

        boardManager.tiles[randomRow, randomCol].towerOnTile = towerController;
        Vector3 worldPos = towerObj.transform.position;

        return true;
    }


    public bool UpgradeRandomTower(TowerController towerToUpgrade, GameObject TargetTower)
    {
        int currentRank = towerToUpgrade.rankValue;

        Vector2Int pos = towerToUpgrade.gridPosition;
        boardManager.tiles[pos.y, pos.x].towerOnTile = null;

        float spacing = boardManager.tileSpacing;
        float offsetX = (boardManager.columns - 1) / 2.0f;
        float offsetY = (boardManager.rows - 1) / 2.0f;
        Vector3 spawnPos = new Vector3((pos.x - offsetX) * spacing, (offsetY - pos.y) * spacing, 0);

        Destroy(towerToUpgrade.gameObject);

        int randomIndex = Random.Range(0, towerPrefabs.Count);
        GameObject newTowerObj = Instantiate(TargetTower, spawnPos, Quaternion.identity);
        TowerController newTowerController = newTowerObj.GetComponent<TowerController>();
        newTowerController.gridPosition = pos;

        newTowerController.rankValue = currentRank;

        newTowerController.UpdateAppearance();
        newTowerController.ReplaceTowerBase();

        boardManager.tiles[pos.y, pos.x].towerOnTile = newTowerController;

        return true;
    }



    public void AddCoin(int num)
    {
        if (playerGold + num > 9999)
        {
            playerGold = 9999;
        }
        else
        {
            playerGold += num;
        }

        // 确保 UI 刷新
        if (uiManager != null)
        {
            uiManager.UpdateGoldUI();
        }
        else
        {
            Debug.LogError("UIManager is NULL! Gold UI not updated.");
        }
    }



    public void DeductCost()
    {
        playerGold -= spawnCost;
        Debug.Log("Tower spawned! Spent " + spawnCost + " gold. Remaining gold: " + playerGold);
        spawnCost += 15;
        Debug.Log("Next spawn will cost: " + spawnCost);
    }
    public void ModifyCost(int cost)
    {
        spawnCost = cost;
        Debug.Log("Tower cost modified to: " + spawnCost);
    }

    public void ReduceHealth(int damage)
    {
        if (playerHealth > 0)
        {
            playerHealth -= damage;

            if (audioSource != null && healthDecreaseClip != null)
            {
                audioSource.PlayOneShot(healthDecreaseClip);
            }
        }
        Debug.Log("Player health after damage: " + playerHealth);
        uiManager.UpdateHealthUI();
    }


    /*** analytic ***/
    IEnumerator RecordTowersOnTile()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f);
            Debug.Log("Recording all towers on tile...");
            TowersOnTile towersOnTileTemp = new TowersOnTile();
            TileController[,] tiles = boardManager.tiles;
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    TileController tile = tiles[x, y];
                    if (tile.towerOnTile != null)
                    {
                        Debug.Log("Tower on tile name: " + tile.towerOnTile.towerName);
                        switch (tile.towerOnTile.towerName)
                        {
                            case "Gold":
                                towersOnTileTemp.goldTowerNum++;
                                break;
                            case "Canon":
                                towersOnTileTemp.canonTowerNum++;
                                break;
                            case "BurningTower":
                                towersOnTileTemp.burningTowerNum++;
                                break;
                            case "Energy":
                                towersOnTileTemp.energyTowerNum++;
                                break;
                            case "FrozenTower":
                                towersOnTileTemp.frozenTowerNum++;
                                break;
                            default:
                                towersOnTileTemp.canonTowerNum++;
                                break;
                        }
                    }
                }
            }
            towersOnTileList.Add(towersOnTileTemp);
        }
    }
    public void AddDamageFromCanonTower(float damage)
    {
        damageFromCanonTower += damage;
    }

    public void AddDamageFromBurningTower(float damage)
    {
        damageFromBurningTower += damage;
    }

    public void AddDamageFromSlowTower(float damage)
    {
        damageFromSlowTower += damage;
    }

    public void AddDamageFromEnergyTower(float damage)
    {
        damageFromEnergyTower += damage;
    }


    public void AddScore(float scoreFromEnemy)
    {
        score += scoreFromEnemy;
    }

    public void AddMergeCount()
    {
        mergeCount += 1f;
    }

    public void AddChangeColorCount()
    {
        changeColorCount += 1f;
    }
    /*** analytic ***/

}

[System.Serializable]
public class TowersOnTile
{
    public int canonTowerNum;
    public int goldTowerNum;
    public int burningTowerNum;
    public int frozenTowerNum;
    public int energyTowerNum;
}
