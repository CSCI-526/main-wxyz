using System.Collections;
using UnityEngine;

public class BurningTowerController : TowerController
{
    private BoardManager board;
    public int towerLevel = 1;
    public float burnDuration = 3f;

    void Start()
    {
        board = FindObjectOfType<BoardManager>();
        if (board == null)
        {
            Debug.LogError("BoardManager not found in the scene!");
            return;
        }

        StartCoroutine(BurnRandomBorderTile());
    }

    public int GetBurnDamage()
    {
        TowerController tower = GetComponent<TowerController>(); // 获取 TowerController
        if (tower != null)
        {
            switch (tower.rankValue)  // 直接使用 TowerController 的 rankValue
            {
                case 1: return 40;
                case 2: return 60;
                case 3: return 80;
                case 4: return 100;
                default: return 40;
            }
        }
        return 40;
    }


    IEnumerator BurnRandomBorderTile()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            TileController[] borderTiles = GetBorderTiles();
            if (borderTiles.Length == 0) continue;

            TileController selectedTile = borderTiles[Random.Range(0, borderTiles.Length)];

            int burnDamage = GetBurnDamage();
            selectedTile.SetTileState(2, burnDamage, burnDuration);

            SpriteRenderer sr = selectedTile.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = Color.red;
            }

            // 立即对Tile上的所有敌人施加燃烧效果
            selectedTile.ApplyBurnEffect(burnDamage, burnDuration);

            yield return new WaitForSeconds(burnDuration);

            if (sr != null)
            {
                sr.color = Color.white;
            }
            selectedTile.SetTileState(0);
        }
    }

    TileController[] GetBorderTiles()
    {
        if (board == null || board.tiles == null) return new TileController[0];

        System.Collections.Generic.List<TileController> borderTiles = new System.Collections.Generic.List<TileController>();

        for (int i = 0; i < board.rows; i++)
        {
            for (int j = 0; j < board.columns; j++)
            {
                if (i == 0 || i == board.rows - 1 || j == 0 || j == board.columns - 1)
                {
                    TileController tile = board.tiles[i, j];

                    if (tile != board.monsterSpawnTile && tile != board.monsterDestTile)
                    {
                        borderTiles.Add(tile);
                    }
                }
            }
        }
        return borderTiles.ToArray();
    }
}
