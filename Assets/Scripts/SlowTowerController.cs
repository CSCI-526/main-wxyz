using System.Collections;
using UnityEngine;

public class SlowTowerController : TowerController
{
    private BoardManager board;
    public int rankValue = 1; // 塔的等级
    public float slowDuration = 3f; // 减速持续时间

    void Start()
    {
        board = FindObjectOfType<BoardManager>();
        if (board == null)
        {
            Debug.LogError("BoardManager not found in the scene!");
            return;
        }

        // 启动周期性减速效果
        StartCoroutine(SlowRandomBorderTile());
    }

    // 获取减速效果强度，依赖于塔的等级
    public float GetSlowEffectAmount()
    {
        TowerController tower = GetComponent<TowerController>(); // 获取 TowerController
        if (tower != null)
        {
            switch (tower.rankValue)  // 直接使用 TowerController 的 rankValue
            {
                case 1: return 0.8f;  // 50% 减速 (小数形式)
                case 2: return 0.6f;  // 40% 减速 (小数形式)
                case 3: return 0.4f;  // 30% 减速 (小数形式)
                case 4: return 0.2f;  // 20% 减速 (小数形式)
                default: return 0.8f; // 默认 20% 减速 (小数形式)
            }
        }
        return 0.8f; // 默认 20% 减速 (小数形式)
    }


    IEnumerator SlowRandomBorderTile()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f); // 每 3 秒触发一次

            TileController[] borderTiles = GetBorderTiles();
            if (borderTiles.Length == 0) continue;

            // 选取一个随机边界 Tile
            TileController selectedTile = borderTiles[Random.Range(0, borderTiles.Length)];

            float SlowEffectAmount = GetSlowEffectAmount();
            selectedTile.SetTileState(1, SlowEffectAmount, slowDuration);


            
            // 变色并设置减速状态
            SpriteRenderer sr = selectedTile.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = Color.cyan; // 变蓝色表示减速
            }

            selectedTile.ApplySlowEffect(SlowEffectAmount, slowDuration);

            // 减速持续时间后恢复颜色并重置状态
            yield return new WaitForSeconds(slowDuration);
            if (sr != null)
            {
                sr.color = Color.white;
            }
            selectedTile.SetTileState(0); // 恢复 Tile 状态为无状态（0）
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