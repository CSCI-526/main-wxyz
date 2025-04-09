using System.Collections;
using UnityEngine;

public class TutFrozenTowerController : TowerController
{
    private BoardManager board;
    //public int rankValue = 1; // 塔的等级
    public float slowDuration = 3f; // 减速持续时间

    void Start()
    {
        base.Start();
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
                case 1: return 0.8f;
                case 2: return 0.6f;
                case 3: return 0.4f;
                case 4: return 0.2f;
                default: return 0.8f;
            }
        }
        return 0.8f;
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

            selectedTile.ApplyEffect(SlowEffectAmount, slowDuration);
            // 使用协程处理减速效果的持续时间
            selectedTile.StartCoroutine(selectedTile.ApplyEffectForDuration());

            /* // 减速持续时间后恢复颜色并重置状态
             yield return new WaitForSeconds(slowDuration);
             if (sr != null)
             {
                 sr.color = Color.white;
             }
             selectedTile.SetTileState(0); // 恢复 Tile 状态为无状态（0）*/
        }
    }

    TileController[] GetBorderTiles()
    {
        if (board == null || board.tiles == null) return new TileController[0];

        System.Collections.Generic.List<TileController> borderTiles = new System.Collections.Generic.List<TileController>();

        int row = 0; // 第一行
        int maxCols = Mathf.Min(7, board.columns); // 防止列数小于4

        for (int j = 0; j < maxCols; j++)
        {
            TileController tile = board.tiles[row, j];

            // 确保 tile 存在，并且不是起点、终点，并且状态为 0
            if (tile != null && tile != board.monsterSpawnTile && tile != board.monsterDestTile && tile.GetTileState() == 0)
            {
                borderTiles.Add(tile);
            }
        }

        return borderTiles.ToArray();
    }
}