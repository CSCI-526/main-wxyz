using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int rows = 7;
    public int columns = 7;
    public float tileSpacing = 1.1f;
    public GameObject tilePrefab;
    public TileController[,] tiles;

    public TileController monsterSpawnTile;
    public TileController monsterTurnTile1;
    public TileController monsterTurnTile2;
    public TileController monsterTurnTile3;
    public TileController monsterDestTile;

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
                Vector3 pos = new Vector3((j - offsetX) * tileSpacing, (offsetY - i) * tileSpacing, 0);
                GameObject tileObj = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                TileController tc = tileObj.GetComponent<TileController>();
                tc.gridPosition = new Vector2Int(j, i);

                if (i == 0 && j == 0)
                {
                    SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
                    if (sr != null) { sr.color = Color.red; }
                    monsterSpawnTile = tc;
                }
                else if (i == 1 && j == 0)
                {
                    SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
                    if (sr != null) { sr.color = Color.green; }
                    monsterDestTile = tc;
                }
                else if (i == 0 && j == columns - 1)
                {
                    SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
                    if (sr != null) { sr.color = Color.white; }
                    monsterTurnTile1 = tc;
                }                
                else if (i == rows - 1 && j == columns - 1)
                {
                    SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
                    if (sr != null) { sr.color = Color.white; }
                    monsterTurnTile2 = tc;
                }
                else if (i == rows - 1 && j == 0)
                {
                    SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
                    if (sr != null) { sr.color = Color.white; }
                    monsterTurnTile3 = tc;
                }
                else if (i == 0 || i == rows - 1 || j == 0 || j == columns - 1)
                {
                    SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
                    if (sr != null) { sr.color = Color.white; }
                }
                tiles[i, j] = tc;
            }
        }
    }

    public bool IsInside(int row, int col)
    {
        return row >= 0 && row < rows && col >= 0 && col < columns;
    }

    public bool IsInnerTile(int row, int col)
    {
        return row > 0 && row < rows - 1 && col > 0 && col < columns - 1;
    }

    public void MoveTowers(Vector2Int direction)
    {
        bool anyMoved = false;

        // Horizontal movement
        if (direction.x != 0)
        {
            for (int i = 1; i < rows - 1; i++)
            {
                // Moving left
                if (direction.x < 0)
                {
                    for (int j = 1; j < columns - 1; j++)
                    {
                        TowerController tower = tiles[i, j].towerOnTile;
                        if (tower != null)
                        {
                            int currentPos = j;
                            while (currentPos > 1 && tiles[i, currentPos - 1].towerOnTile == null)
                            {
                                currentPos--;
                            }
                            if (currentPos > 1 && tiles[i, currentPos - 1].towerOnTile != null)
                            {
                                TowerController destTower = tiles[i, currentPos - 1].towerOnTile;
                                if (destTower.rankValue == tower.rankValue && destTower.rankValue < 4)
                                {
                                    destTower.UpgradeTower();
                                    Destroy(tower.gameObject);
                                    tiles[i, j].towerOnTile = null;
                                    anyMoved = true;
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
                // Moving right
                else if (direction.x > 0)
                {

                    for (int j = columns - 2; j >= 1; j--)
                    {
                        TowerController tower = tiles[i, j].towerOnTile;
                        if (tower != null)
                        {
                            int currentPos = j;
                            while (currentPos < columns - 2 && tiles[i, currentPos + 1].towerOnTile == null)
                            {
                                currentPos++;
                            }
                            if (currentPos < columns - 2 && tiles[i, currentPos + 1].towerOnTile != null)
                            {
                                TowerController destTower = tiles[i, currentPos + 1].towerOnTile;
                                if (destTower.rankValue == tower.rankValue && destTower.rankValue < 4)
                                {
                                    destTower.UpgradeTower();
                                    Destroy(tower.gameObject);
                                    tiles[i, j].towerOnTile = null;
                                    anyMoved = true;
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
        // Vertical movement
        else if (direction.y != 0)
        {

            if (direction.y < 0)
            {
                for (int j = 1; j < columns - 1; j++)
                {
  
                    for (int i = rows - 2; i >= 1; i--)
                    {
                        TowerController tower = tiles[i, j].towerOnTile;
                        if (tower != null)
                        {
                            int currentPos = i;
                            while (currentPos < rows - 2 && tiles[currentPos + 1, j].towerOnTile == null)
                            {
                                currentPos++;
                            }
                            if (currentPos < rows - 2 && tiles[currentPos + 1, j].towerOnTile != null)
                            {
                                TowerController destTower = tiles[currentPos + 1, j].towerOnTile;
                                if (destTower.rankValue == tower.rankValue && destTower.rankValue < 4)
                                {
                                    destTower.UpgradeTower();
                                    Destroy(tower.gameObject);
                                    tiles[i, j].towerOnTile = null;
                                    anyMoved = true;
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
                            if (currentPos > 1 && tiles[currentPos - 1, j].towerOnTile != null)
                            {
                                TowerController destTower = tiles[currentPos - 1, j].towerOnTile;
                                if (destTower.rankValue == tower.rankValue && destTower.rankValue < 4)
                                {
                                    destTower.UpgradeTower();
                                    Destroy(tower.gameObject);
                                    tiles[i, j].towerOnTile = null;
                                    anyMoved = true;
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
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        float gridWidth = columns * tileSpacing;
        float gridHeight = rows * tileSpacing;
        Vector3 center = transform.position;
        Vector3 origin = center - new Vector3(gridWidth / 2, gridHeight / 2, 0);

        for (int i = 0; i <= rows; i++)
        {
            Vector3 start = origin + new Vector3(0, i * tileSpacing, 0);
            Vector3 end = start + new Vector3(gridWidth, 0, 0);
            Gizmos.DrawLine(start, end);
        }

        for (int j = 0; j <= columns; j++)
        {
            Vector3 start = origin + new Vector3(j * tileSpacing, 0, 0);
            Vector3 end = start + new Vector3(0, gridHeight, 0);
            Gizmos.DrawLine(start, end);
        }
    }
}
