using UnityEngine;
using System.Collections;

public class TutFrozenTowerController : TowerController
{
    public float slowDuration = 3f;
    public Sprite frozenTileSprite;

    private BoardManager board;
    //public int rankValue = 1; // 塔的等级
    public float slowDuration = 3f; // 减速持续时间
    public Sprite[] freezeFrames;  // 四帧动画
    private SpriteRenderer slowTowerRenderer;

    void Start()
    {
        base.Start();
        slowTowerRenderer = GetComponent<SpriteRenderer>();
        board = FindObjectOfType<BoardManager>();
        if (board == null) { Debug.LogError("BoardManager not found"); return; }
        StartCoroutine(SlowRandomBorderTile());
    }

    float GetSlowEffectAmount()
    {
        switch (rankValue)
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

            TileController[] borderTiles = GetBorderTiles();
            if (borderTiles.Length == 0) continue;

            TileController selectedTile = borderTiles[Random.Range(0, borderTiles.Length)];
            float slowAmount = GetSlowEffectAmount();

            // float SlowEffectAmount = GetSlowEffectAmount();
            // selectedTile.SetTileState(1, SlowEffectAmount, slowDuration);


            
            // 变色并设置减速状态
            SpriteRenderer sr = selectedTile.GetComponent<SpriteRenderer>();
            Sprite originalSprite = sr ? sr.sprite : null;
            if (sr && frozenTileSprite) sr.sprite = frozenTileSprite;

            selectedTile.SetTileState(1, slowAmount, slowDuration);
            selectedTile.ApplyEffect(slowAmount, slowDuration);
            selectedTile.StartCoroutine(selectedTile.ApplyEffectForDuration());

           /* // 减速持续时间后恢复颜色并重置状态
            yield return new WaitForSeconds(slowDuration);
            if (sr != null)
            {
                sr.color = Color.white;
            }
            selectedTile.SetTileState(0); // 恢复 Tile 状态为无状态（0）*/

            StartCoroutine(PlayFreezeAnimation());

            /***             ***/

            if (sr && originalSprite) sr.sprite = originalSprite;
            selectedTile.SetTileState(0);
            /***             ***/
            
            yield return new WaitForSeconds(5f); // 每 3 秒触发一次

            // yield return new WaitForSeconds(slowDuration);

            // if (sr && originalSprite) sr.sprite = originalSprite;
            // selectedTile.SetTileState(0);
        }
    }

    IEnumerator PlayFreezeAnimation()
    {
        if (freezeFrames == null || freezeFrames.Length < 4 || slowTowerRenderer == null)
            yield break;

        slowTowerRenderer.sprite = freezeFrames[0];
        yield return new WaitForSeconds(0.1f);

        slowTowerRenderer.sprite = freezeFrames[1];
        yield return new WaitForSeconds(0.1f);

        slowTowerRenderer.sprite = freezeFrames[2];
        yield return new WaitForSeconds(0.1f);

        slowTowerRenderer.sprite = freezeFrames[3];
        float holdDuration = slowDuration - 0.3f;
        yield return new WaitForSeconds(holdDuration);

        slowTowerRenderer.sprite = freezeFrames[0];
    } 
    TileController[] GetBorderTiles()
    {
        if (board == null || board.tiles == null) return new TileController[0];

        var list = new System.Collections.Generic.List<TileController>();
        int maxCols = Mathf.Min(7, board.columns);
        for (int j = 0; j < maxCols; j++)
        {
            TileController tile = board.tiles[0, j];
            if (tile != null && tile != board.monsterSpawnTile && tile != board.monsterDestTile && tile.GetTileState() == 0)
                list.Add(tile);
        }
        return list.ToArray();
    }
}
