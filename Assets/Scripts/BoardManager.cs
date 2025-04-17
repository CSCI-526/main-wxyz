using UnityEngine;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int rows = 7;
    public int columns = 7;
    public float tileSpacing = 1.1f;
    public GameObject tilePrefab;

    [Header("Tile Sprites")]
    public Sprite roadTileSprite;
    public Sprite boardTileSprite;

    public TileController[,] tiles;
    public GameManager gameManager;

    public TileController monsterSpawnTile;
    public TileController monsterTurnTile1;
    public TileController monsterTurnTile2;
    public TileController monsterTurnTile3;
    public TileController monsterDestTile;

    void Start() { }

    public void CreateGrid()
    {
        tiles = new TileController[rows, columns];
        float offsetX = (columns - 1) / 2f;
        float offsetY = (rows - 1) / 2f;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 pos = new Vector3((j - offsetX) * tileSpacing, (offsetY - i) * tileSpacing, 0f);
                GameObject tileObj = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                TileController tc = tileObj.GetComponent<TileController>();
                SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();

                tc.gridPosition = new Vector2Int(j, i);
                tiles[i, j] = tc;

                bool isOuter = (i == 0 || i == rows - 1 || j == 0 || j == columns - 1);
                if (sr != null)
                {
                    sr.sprite = isOuter ? roadTileSprite : boardTileSprite;
                    sr.color = Color.white;
                }

                if (i == 0 && j == 0)
                {
                    if (sr != null) sr.color = Color.red;
                    monsterSpawnTile = tc;
                }
                else if (i == 1 && j == 0)
                {
                    if (sr != null) sr.color = Color.green;
                    monsterDestTile = tc;
                }
                else if (i == 0 && j == columns - 1)
                {
                    monsterTurnTile1 = tc;
                }
                else if (i == rows - 1 && j == columns - 1)
                {
                    monsterTurnTile2 = tc;
                }
                else if (i == rows - 1 && j == 0)
                {
                    monsterTurnTile3 = tc;
                }
            }
        }
    }

    public void MoveTowers(Vector2Int direction)
    {
        bool[,] hasMerged = new bool[rows, columns];
        bool anyMoved = false;

        if (direction.x != 0)
        {
            for (int i = 1; i < rows - 1; i++)
            {
                if (direction.x < 0)
                {
                    for (int j = 1; j < columns - 1; j++)
                        HandleHorizontalMove(i, j, -1, hasMerged, ref anyMoved);
                }
                else
                {
                    for (int j = columns - 2; j >= 1; j--)
                        HandleHorizontalMove(i, j, 1, hasMerged, ref anyMoved);
                }
            }
        }
        else if (direction.y != 0)
        {
            if (direction.y < 0)
            {
                for (int j = 1; j < columns - 1; j++)
                    for (int i = rows - 2; i >= 1; i--)
                        HandleVerticalMove(i, j, 1, hasMerged, ref anyMoved);
            }
            else
            {
                for (int j = 1; j < columns - 1; j++)
                    for (int i = 1; i < rows - 1; i++)
                        HandleVerticalMove(i, j, -1, hasMerged, ref anyMoved);
            }
        }

        if (anyMoved) Debug.Log("Towers moved and merged within the inner grid.");
    }

    void HandleHorizontalMove(int row, int col, int dir, bool[,] merged, ref bool anyMoved)
    {
        TowerController tower = tiles[row, col].towerOnTile;
        if (tower == null) return;

        int currentPos = col;
        while (currentPos - dir > 0 && currentPos - dir < columns - 1 && tiles[row, currentPos - dir].towerOnTile == null)
            currentPos -= dir;

        if (currentPos - dir > 0 && currentPos - dir < columns - 1)
        {
            TowerController dest = tiles[row, currentPos - dir].towerOnTile;
            if (dest != null && !merged[row, currentPos - dir] && dest.rankValue == tower.rankValue && dest.rankValue < 4 && dest.towerName == tower.towerName)
            {
                tiles[row, col].towerOnTile = null;
                dest.UpgradeTower();
                merged[row, currentPos - dir] = true;
                Destroy(tower.gameObject);
                anyMoved = true;
                return;
            }
        }

        if (currentPos != col)
        {
            tiles[row, col].towerOnTile = null;
            tiles[row, currentPos].towerOnTile = tower;
            tower.transform.position = tiles[row, currentPos].transform.position;
            tower.gridPosition = new Vector2Int(currentPos, row);
            anyMoved = true;
        }
    }

    void HandleVerticalMove(int row, int col, int dir, bool[,] merged, ref bool anyMoved)
    {
        TowerController tower = tiles[row, col].towerOnTile;
        if (tower == null) return;

        int currentPos = row;
        while (currentPos - dir > 0 && currentPos - dir < rows - 1 && tiles[currentPos - dir, col].towerOnTile == null)
            currentPos -= dir;

        if (currentPos - dir > 0 && currentPos - dir < rows - 1)
        {
            TowerController dest = tiles[currentPos - dir, col].towerOnTile;
            if (dest != null && !merged[currentPos - dir, col] && dest.rankValue == tower.rankValue && dest.rankValue < 4 && dest.towerName == tower.towerName)
            {
                tiles[row, col].towerOnTile = null;
                dest.UpgradeTower();
                merged[currentPos - dir, col] = true;
                Destroy(tower.gameObject);
                anyMoved = true;
                return;
            }
        }

        if (currentPos != row)
        {
            tiles[row, col].towerOnTile = null;
            tiles[currentPos, col].towerOnTile = tower;
            tower.transform.position = tiles[currentPos, col].transform.position;
            tower.gridPosition = new Vector2Int(col, currentPos);
            anyMoved = true;
        }
    }

    public void LightAllTileHovers()
    {
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
                tiles[i, j]?.LightOnHover();
    }

    public void DisableAllTileHovers()
    {
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
                tiles[i, j]?.DisableHover();
    }

    public void UpdateTileHoverStatesAllButOne(TileController excludedTile)
    {
        LightAllTileHovers();
        if (excludedTile != null && IsInnerTile(excludedTile.gridPosition.y, excludedTile.gridPosition.x) && excludedTile.towerOnTile != null)
            excludedTile.DisableHover();
    }

    public void UpdateTileHoverStates(TileController currentTile)
    {
        DisableAllTileHovers();
        currentTile?.LightOnHover();
    }

    public TileController GetTileUnderPosition(Vector3 worldPos)
    {
        float offsetX = (columns - 1) / 2f;
        float offsetY = (rows - 1) / 2f;
        Vector3 local = worldPos - transform.position;
        int j = Mathf.RoundToInt(local.x / tileSpacing + offsetX);
        int i = Mathf.RoundToInt(offsetY - local.y / tileSpacing);
        return IsInside(i, j) ? tiles[i, j] : null;
    }

    public bool IsInside(int row, int col)
    {
        return row >= 0 && row < rows && col >= 0 && col < columns;
    }

    public bool IsInnerTile(int row, int col)
    {
        return row > 0 && row < rows - 1 && col > 0 && col < columns - 1;
    }

    public List<TowerController> GetAllTowersOnBoard()
    {
        List<TowerController> list = new List<TowerController>();
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
                if (tiles[i, j].towerOnTile != null)
                    list.Add(tiles[i, j].towerOnTile);
        return list;
    }
}
