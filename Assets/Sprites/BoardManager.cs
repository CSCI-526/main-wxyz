using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int rows = 7;
    public int columns = 7;
    public float tileSpacing = 1.1f;
    public GameObject tilePrefab; 
    public TileController[,] tiles;

    public TileController monsterSpawnTile;
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
                    if (sr != null)
                    {
                        sr.color = Color.red;
                    }
                    monsterSpawnTile = tc;
                }

                else if (i == 1 && j == 0)
                {
                    SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.color = Color.green;
                    }
                    monsterDestTile = tc;
                }
                else if (i == 0 || i == rows - 1 || j == 0 || j == columns - 1)
                {
                    SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.color = Color.white;
                    }
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
        bool moved;
        do
        {
            moved = false;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int row = (direction.y > 0) ? (rows - 1 - i) : i;
                    int col = (direction.x > 0) ? (columns - 1 - j) : j;
                    TileController currentTile = tiles[row, col];
                    if (currentTile.towerOnTile != null)
                    {
                        int newRow = row + direction.y;
                        int newCol = col + direction.x;
                        if (IsInside(newRow, newCol) && IsInnerTile(newRow, newCol) &&
                            tiles[newRow, newCol].towerOnTile == null)
                        {
                            tiles[newRow, newCol].towerOnTile = currentTile.towerOnTile;
                            currentTile.towerOnTile = null;
                            tiles[newRow, newCol].towerOnTile.transform.position =
                                tiles[newRow, newCol].transform.position;
                            moved = true;
                        }
                    }
                }
            }
        } while (moved);
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
