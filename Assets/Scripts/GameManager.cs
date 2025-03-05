using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public BoardManager boardManager;
    public EnemySpawn enemyManager;
    public GameObject[] towerPrefabs;
    public UIManager uiManager; //添加UIManager连接

    public int playerGold = 9999; 
    public int spawnCost = 10;
    public int playerHealth = 10; 

    private bool hasLost = false;

    void Start()
    {
        Time.timeScale = 1f;
        boardManager.CreateGrid();
        bool spawned = SpawnRandomTower();
        enemyManager.EnemySpawnConfigInit();
        StartCoroutine(enemyManager.SpawnWaves());
        uiManager.UpdateHealthUI(); //游戏开始时更新血量UI
    }

    void Update()
    {
        if (playerHealth <= 0)
        {
            ShowFailScreen();
            Time.timeScale = 0f;
        }

        // ✅ 监听键盘输入，让塔可以移动
        HandleKeyboardInput();
    }

    /// <summary>
    /// 监听方向键输入并移动塔
    /// </summary>
    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("上移动作触发");
            boardManager.MoveTowers(Vector2Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("下移动作触发");
            boardManager.MoveTowers(Vector2Int.down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("左移动作触发");
            boardManager.MoveTowers(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("右移动作触发");
            boardManager.MoveTowers(Vector2Int.right);
        }
    }

    private void ShowFailScreen()
    {
        if (!hasLost)
        {
            Debug.Log("Player lose !!!");
            hasLost = true;
            uiManager.ShowGameOverUI();
            //失败触发GameOver
        }
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

        int randomTowerIndex = Random.Range(0, towerPrefabs.Length);
        GameObject selectedTowerPrefab = towerPrefabs[randomTowerIndex];

        Vector2Int chosenTile = availableTiles[Random.Range(0, availableTiles.Count)];
        int randomCol = chosenTile.x;
        int randomRow = chosenTile.y;

        float spacing = boardManager.tileSpacing;
        float offsetX = (boardManager.columns - 1) / 2.0f;
        float offsetY = (boardManager.rows - 1) / 2.0f;
        Vector3 spawnPos = new Vector3((randomCol - offsetX) * spacing, (offsetY - randomRow) * spacing, 0);

        GameObject towerObj = Instantiate(selectedTowerPrefab, spawnPos, Quaternion.identity);
        TowerController towerController = towerObj.GetComponent<TowerController>();
        towerController.gridPosition = new Vector2Int(randomCol, randomRow);

        boardManager.tiles[randomRow, randomCol].towerOnTile = towerController;

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
    }

    public void DeductCost()
    {
        playerGold -= spawnCost;
        Debug.Log("Tower spawned! Spent " + spawnCost + " gold. Remaining gold: " + playerGold);
        spawnCost += 10;
        Debug.Log("Next spawn will cost: " + spawnCost);
    }
    public void ReduceHealth(int damage)
    {
        playerHealth -= damage;
        Debug.Log("Player health after damage: " + playerHealth);
        uiManager.UpdateHealthUI();
    }

}
