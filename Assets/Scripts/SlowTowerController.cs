using System.Collections;
using UnityEngine;

public class SlowTowerController : TowerController
{
    private BoardManager board;
    public int rankValue = 1; // 塔的等级
    public float slowDuration = 3f; // 减速持续时间
    public float slowEffectAmount = 0.5f; // 减速百分比 (50%减速)

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
    float GetSlowEffectAmount()
    {
        switch (rankValue)
        {
            case 1: return 0.5f; // 50%减速
            case 2: return 0.6f; // 60%减速
            case 3: return 0.7f; // 70%减速
            case 4: return 0.8f; // 80%减速
            default: return 0.5f; // 默认为 1 级减速效果
        }
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

            // 只有当 Tile 的状态为 0 时才更改为减速状态（1）
            if (selectedTile.GetTileState() == 0)
            {
                selectedTile.SetTileState(1);

                // 变色并设置减速状态
                SpriteRenderer sr = selectedTile.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = Color.cyan; // 变蓝色表示减速
                }

                // 施加减速效果
                ApplySlowEffect(selectedTile);

                // 减速持续时间后恢复颜色并重置状态
                yield return new WaitForSeconds(slowDuration);
                if (sr != null)
                {
                    sr.color = Color.white;
                }
                selectedTile.SetTileState(0); // 恢复 Tile 状态为无状态（0）
            }
        }
    }


    void ApplySlowEffect(TileController tile)
    {
        float slowAmount = GetSlowEffectAmount(); // 根据塔等级获取减速效果强度
        foreach (Enemy enemy in tile.GetEnemiesOnTile())
        {
            enemy.EnemySlowEffect(slowAmount, slowDuration); // 直接调用敌人的减速函数
            enemy.slowEffectRender(); // 让敌人变色为蓝色，表示减速效果
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

                    // 排除起点和终点
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
