using System.Collections;
using UnityEngine;

public class SlowTowerController : TowerController
{
    public float slowDuration = 3f;
    public Sprite slowTileSprite;

    private BoardManager board;
    //public int rankValue = 1; // 塔的等级
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
            case 1: return 0.8f;
            case 2: return 0.6f;
            case 3: return 0.4f;
            case 4: return 0.2f;
            default: return 0.8f;
        }
    }

    IEnumerator SlowRandomBorderTile()
    {
        while (true)
        {

            TileController[] borderTiles = GetBorderTiles();
            if (borderTiles.Length == 0) continue;

            TileController selectedTile = borderTiles[Random.Range(0, borderTiles.Length)];

            float slowAmount = GetSlowEffectAmount();
            selectedTile.SetTileState(1, slowAmount, slowDuration);

            SpriteRenderer sr = selectedTile.GetComponent<SpriteRenderer>();
            Sprite originalSprite = sr != null ? sr.sprite : null;
            if (sr != null && slowTileSprite != null) sr.sprite = slowTileSprite;

            selectedTile.ApplyEffect(slowAmount, slowDuration);
            selectedTile.StartCoroutine(selectedTile.ApplyEffectForDuration());

            StartCoroutine(PlayFreezeAnimation());
            
            yield return new WaitForSeconds(slowDuration);
            

            if (sr != null && originalSprite != null) sr.sprite = originalSprite;
            selectedTile.SetTileState(0);
            yield return new WaitForSeconds(2f); // 每 3 秒触发一次
            


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
        for (int i = 0; i < board.rows; i++)
            for (int j = 0; j < board.columns; j++)
                if (i == 0 || i == board.rows - 1 || j == 0 || j == board.columns - 1)
                {
                    TileController tile = board.tiles[i, j];
                    if (tile != null && tile != board.monsterSpawnTile && tile != board.monsterDestTile && tile.GetTileState() == 0)
                        list.Add(tile);
                }
        return list.ToArray();
    }
}
