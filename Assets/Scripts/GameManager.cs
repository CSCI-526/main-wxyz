using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public BoardManager boardManager;
    public GameObject towerPrefab;

    public int playerGold = 9999; 
    public int spawnCost = 10;   

    void Start()
    {
        boardManager.CreateGrid();
        bool spawned = SpawnRandomTower();
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

        GameObject towerObj = Instantiate(towerPrefab, spawnPos, Quaternion.identity);
        TowerController towerController = towerObj.GetComponent<TowerController>();
        towerController.gridPosition = new Vector2Int(randomCol, randomRow);

        boardManager.tiles[randomRow, randomCol].towerOnTile = towerController;

        return true;
    }




    public void DeductCost()
    {
        playerGold -= spawnCost;
        Debug.Log("Tower spawned! Spent " + spawnCost + " gold. Remaining gold: " + playerGold);
        spawnCost += 10;
        Debug.Log("Next spawn will cost: " + spawnCost);
    }
}
