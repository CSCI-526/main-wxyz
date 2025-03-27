using System.Collections;
using UnityEngine;

public class BurningTowerController : TowerController
{
    private BoardManager board;
    public float burnDuration = 3f;

    void Start()
    {
        base.Start();
        board = FindObjectOfType<BoardManager>();
        if (board == null)
        {
            Debug.LogError("BoardManager not found in the scene!");
            return;
        }

        StartCoroutine(BurnRandomBorderTile());
    }

    public float GetBurnDamage()
    {
        TowerController tower = GetComponent<TowerController>(); // 获取 TowerController
        if (tower != null)
        {
            switch (tower.rankValue)  // 直接使用 TowerController 的 rankValue
            {
                case 1: return 20;
                case 2: return 25;
                case 3: return 35;
                case 4: return 50;
                default: return 20;
            }
        }
        return 20;
    }


    IEnumerator BurnRandomBorderTile()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            TileController[] borderTiles = GetBorderTiles();
            if (borderTiles.Length == 0) continue;

            TileController selectedTile = borderTiles[Random.Range(0, borderTiles.Length)];

            float burnDamage = GetBurnDamage();
            selectedTile.SetTileState(2, burnDamage, burnDuration);

            SpriteRenderer sr = selectedTile.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = Color.magenta;
            }

            // 立即对Tile上的所有敌人施加燃烧效果
            selectedTile.ApplyEffect(burnDamage, burnDuration);

            // 使用协程处理燃烧效果的持续时间
            selectedTile.StartCoroutine(selectedTile.ApplyEffectForDuration());

            /*yield return new WaitForSeconds(burnDuration);

            if (sr != null)
            {
                sr.color = Color.white;
            }
            selectedTile.SetTileState(0);*/
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
                // 只选取四周的 Tile
                if (i == 0 || i == board.rows - 1 || j == 0 || j == board.columns - 1)
                {
                    TileController tile = board.tiles[i, j];

                    // 确保 tile 存在，并且不是起点、终点，并且状态为 0
                    if (tile != null && tile != board.monsterSpawnTile && tile != board.monsterDestTile && tile.GetTileState() == 0)
                    {
                        borderTiles.Add(tile);
                    }
                }
            }
        }
        return borderTiles.ToArray();
    }
}
