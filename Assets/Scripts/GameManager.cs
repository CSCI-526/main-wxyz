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
      int rows = boardManager.rows;
      int columns = boardManager.columns;
      float spacing = boardManager.tileSpacing;

      int randomRow = Random.Range(0, rows);
      int randomCol = Random.Range(0, columns);

      float offsetX = (columns - 1) / 2.0f;
      float offsetY = (rows - 1) / 2.0f;

      Vector3 spawnPos = new Vector3(randomCol - offsetX, randomRow - offsetY, 0) * spacing;

      GameObject towerObj = Instantiate(towerPrefab, spawnPos, Quaternion.identity);

      TowerController towerController = towerObj.GetComponent<TowerController>();

      boardManager.tiles[randomRow, randomCol].towerOnTile = towerController;
  }

}
