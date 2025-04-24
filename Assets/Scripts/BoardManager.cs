using UnityEngine;
using System.Collections.Generic;
public class BoardManager : MonoBehaviour
{
    public int rows = 7;
    public int columns = 7;
    public float tileSpacing = 1.1f;
    public GameObject tilePrefab;
    public TileController[,] tiles;
    public GameManager gameManager;

    public TileController monsterSpawnTile;
    public TileController monsterTurnTile1;
    public TileController monsterTurnTile2;
    public TileController monsterTurnTile3;
    public TileController monsterDestTile;
    [SerializeField] private Sprite monsterRoadSprite;   
    [SerializeField] private Sprite innerTileSprite;    
    void Start()
    {
    }

    public void CreateGrid()
    {
        tiles = new TileController[rows, columns];

        float offsetX = (columns - 1) / 2.0f;
        float offsetY = (rows - 1) / 2.0f;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 pos = new Vector3((j - offsetX) * tileSpacing,
                                        (offsetY - i) * tileSpacing,
                                        0f);
                GameObject tileObj = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                TileController tc  = tileObj.GetComponent<TileController>();
                tc.gridPosition    = new Vector2Int(j, i);

                SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    bool isOuter = (i == 0 || i == rows - 1 || j == 0 || j == columns - 1);
                    sr.sprite = isOuter ? monsterRoadSprite : innerTileSprite;
                }

                if (i == 0 && j == 0)                         // spawn
                {
                    if (sr) sr.color = Color.red;
                    monsterSpawnTile = tc;
                }
                else if (i == 1 && j == 0)                    // destination
                {
                    if (sr) sr.color = Color.green;
                    monsterDestTile = tc;
                }
                else if (i == 0 && j == columns - 1)          
                {
                    if (sr) sr.color = Color.white;
                    monsterTurnTile1 = tc;
                }
                else if (i == rows - 1 && j == columns - 1)   // turn 2
                {
                    if (sr) sr.color = Color.white;
                    monsterTurnTile2 = tc;
                }
                else if (i == rows - 1 && j == 0)             
                {
                    if (sr) sr.color = Color.white;
                    monsterTurnTile3 = tc;
                }
                else if (sr && (i == 0 || i == rows - 1 || j == 0 || j == columns - 1))
                {
                    sr.color = Color.white;
                }

                tiles[i, j] = tc;
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
                    // Move left
                    for (int j = 1; j < columns - 1; j++)
                    {
                        TowerController tower = tiles[i, j].towerOnTile;
                        if (tower != null)
                        {
                            int currentPos = j;
                            // slide as far left as possible
                            while (currentPos > 1 && tiles[i, currentPos - 1].towerOnTile == null)
                            {
                                currentPos--;
                            }

                            // check if we can merge
                            if (currentPos > 1 && tiles[i, currentPos - 1].towerOnTile != null)
                            {
                                TowerController destTower = tiles[i, currentPos - 1].towerOnTile;
                            
                                if (!hasMerged[i, currentPos - 1] &&
                                    destTower.rankValue == tower.rankValue &&
                                    destTower.rankValue < 4 &&
                                    destTower.towerName == tower.towerName)
                                {
                                    // Merge
                                    tiles[i, j].towerOnTile = null;
                                    anyMoved = true;

                                    // Upgrade the destination tower
                                    destTower.UpgradeTower();

                                    hasMerged[i, currentPos - 1] = true;

                                    // Destroy the old tower
                                    Destroy(tower.gameObject);
                                    continue;
                                }
                            }
                          
                            if (currentPos != j)
                            {
                                tiles[i, j].towerOnTile = null;
                                tiles[i, currentPos].towerOnTile = tower;
                                tower.transform.position = tiles[i, currentPos].transform.position;
                                tower.gridPosition = new Vector2Int(currentPos, i);
                                anyMoved = true;
                            }
                        }
                    }
                }
                else if (direction.x > 0) 
                {
                    // Move right
                    for (int j = columns - 2; j >= 1; j--)
                    {
                        TowerController tower = tiles[i, j].towerOnTile;
                        if (tower != null)
                        {
                            int currentPos = j;
                            // slide as far right as possible
                            while (currentPos < columns - 2 && tiles[i, currentPos + 1].towerOnTile == null)
                            {
                                currentPos++;
                            }

                           
                            if (currentPos < columns - 2 && tiles[i, currentPos + 1].towerOnTile != null)
                            {
                                TowerController destTower = tiles[i, currentPos + 1].towerOnTile;

                                if (!hasMerged[i, currentPos + 1] &&
                                    destTower.rankValue == tower.rankValue &&
                                    destTower.rankValue < 4 &&
                                    destTower.towerName == tower.towerName)
                                {
                                    // Merge
                                    tiles[i, j].towerOnTile = null;
                                    anyMoved = true;

                                    destTower.UpgradeTower();
                                    hasMerged[i, currentPos + 1] = true;

                                    Destroy(tower.gameObject);
                                    continue;
                                }
                            }
                           
                            if (currentPos != j)
                            {
                                tiles[i, j].towerOnTile = null;
                                tiles[i, currentPos].towerOnTile = tower;
                                tower.transform.position = tiles[i, currentPos].transform.position;
                                tower.gridPosition = new Vector2Int(currentPos, i);
                                anyMoved = true;
                            }
                        }
                    }
                }
            }
        }
        else if (direction.y != 0)
        {
            if (direction.y < 0) 
            {
                // Move down
                for (int j = 1; j < columns - 1; j++)
                {
                    for (int i = rows - 2; i >= 1; i--)
                    {
                        TowerController tower = tiles[i, j].towerOnTile;
                        if (tower != null)
                        {
                            int currentPos = i;
                            // slide as far down as possible
                            while (currentPos < rows - 2 && tiles[currentPos + 1, j].towerOnTile == null)
                            {
                                currentPos++;
                            }

                            // check if we can merge
                            if (currentPos < rows - 2 && tiles[currentPos + 1, j].towerOnTile != null)
                            {
                                TowerController destTower = tiles[currentPos + 1, j].towerOnTile;
                                // Check hasMerged
                                if (!hasMerged[currentPos + 1, j] &&
                                    destTower.rankValue == tower.rankValue &&
                                    destTower.rankValue < 4 &&
                                    destTower.towerName == tower.towerName)
                                {
                                    // Merge
                                    tiles[i, j].towerOnTile = null;
                                    anyMoved = true;

                                    destTower.UpgradeTower();
                                    hasMerged[currentPos + 1, j] = true;

                                    Destroy(tower.gameObject);
                                    continue;
                                }
                            }
                            
                            if (currentPos != i)
                            {
                                tiles[i, j].towerOnTile = null;
                                tiles[currentPos, j].towerOnTile = tower;
                                tower.transform.position = tiles[currentPos, j].transform.position;
                                tower.gridPosition = new Vector2Int(j, currentPos);
                                anyMoved = true;
                            }
                        }
                    }
                }
            }
            else if (direction.y > 0) 
            {
                // Move up
                for (int j = 1; j < columns - 1; j++)
                {
                    for (int i = 1; i < rows - 1; i++)
                    {
                        TowerController tower = tiles[i, j].towerOnTile;
                        if (tower != null)
                        {
                            int currentPos = i;
                            while (currentPos > 1 && tiles[currentPos - 1, j].towerOnTile == null)
                            {
                                currentPos--;
                            }

                            // check if we can merge
                            if (currentPos > 1 && tiles[currentPos - 1, j].towerOnTile != null)
                            {
                                TowerController destTower = tiles[currentPos - 1, j].towerOnTile;
                                // Check hasMerged
                                if (!hasMerged[currentPos - 1, j] &&
                                    destTower.rankValue == tower.rankValue &&
                                    destTower.rankValue < 4 &&
                                    destTower.towerName == tower.towerName)
                                {
                                    // Merge
                                    tiles[i, j].towerOnTile = null;
                                    anyMoved = true;

                                    destTower.UpgradeTower();
                                    hasMerged[currentPos - 1, j] = true;

                                    Destroy(tower.gameObject);
                                    continue;
                                }
                            }
                            if (currentPos != i)
                            {
                                tiles[i, j].towerOnTile = null;
                                tiles[currentPos, j].towerOnTile = tower;
                                tower.transform.position = tiles[currentPos, j].transform.position;
                                tower.gridPosition = new Vector2Int(j, currentPos);
                                anyMoved = true;
                            }
                        }
                    }
                }
            }
        }

        if (anyMoved)
        {
            Debug.Log("Towers moved and merged within the inner grid.");
        }
        if (gameManager != null)
        {
            int towerCount = GetAllTowersOnBoard().Count;
            gameManager.uiManager.SetTowerCost(towerCount * 10);
        }
    }


    public void LightAllTileHovers()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (tiles[i, j] != null)
                {
                    tiles[i, j].LightOnHover();
                }
            }
        }
    }

    public void DisableAllTileHovers()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (tiles[i, j] != null)
                {
                    tiles[i, j].DisableHover();
                }
            }
        }
    }


    public void UpdateTileHoverStatesAllButOne(TileController excludedTile)
    {
        LightAllTileHovers();

        if (excludedTile != null)
        {
            if (IsInnerTile(excludedTile.gridPosition.y, excludedTile.gridPosition.x))
            {
                if (excludedTile.towerOnTile != null)
                {
                    excludedTile.DisableHover();
                }
            }
        }
    }

    public void UpdateTileHoverStates(TileController currentTile)
    {
        DisableAllTileHovers();
        if (currentTile != null)
        {
            currentTile.LightOnHover();
        }
    }

    public TileController GetTileUnderPosition(Vector3 worldPosition)
    {
        float offsetX = (columns - 1) / 2.0f;
        float offsetY = (rows - 1) / 2.0f;
        
        Vector3 localPos = worldPosition - transform.position;
        int j = Mathf.RoundToInt((localPos.x / tileSpacing) + offsetX);
        int i = Mathf.RoundToInt(offsetY - (localPos.y / tileSpacing));

        if (IsInside(i, j))
        {   
            
            return tiles[i, j];
        }

        return null;
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
        List<TowerController> towers = new List<TowerController>();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                TowerController tower = tiles[i, j].towerOnTile;
                if (tower != null)
                {
                    towers.Add(tower);
                }
            }
        }

        return towers;
    }
}