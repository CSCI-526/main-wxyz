using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BoardManager boardManager;
    public GameObject towerPrefab;

    void Start()
    {
        boardManager.CreateGrid();
        SpawnRandomTower();
    }

    void SpawnRandomTower()
    {
        int randomRow = Random.Range(1, boardManager.rows - 1);
        int randomCol = Random.Range(1, boardManager.columns - 1);
        float spacing = boardManager.tileSpacing;

        float offsetX = (boardManager.columns - 1) / 2.0f;
        float offsetY = (boardManager.rows - 1) / 2.0f;

        Vector3 spawnPos = new Vector3(randomCol - offsetX, randomRow - offsetY, 0) * spacing;
        GameObject towerObj = Instantiate(towerPrefab, spawnPos, Quaternion.identity);
        TowerController towerController = towerObj.GetComponent<TowerController>();

        boardManager.tiles[randomRow, randomCol].towerOnTile = towerController;
    }
}
