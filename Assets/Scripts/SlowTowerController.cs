using System.Collections;
using UnityEngine;

public class SlowTowerController : TowerController
{
    
    public float frozenDuration = 4f;
    public Sprite[] freezeFrames;
    public Sprite frozenTileSprite;

    private SpriteRenderer frozenTowerRenderer;
    private BoardManager board;

    void Start()
    {
        base.Start();
        frozenTowerRenderer = GetComponent<SpriteRenderer>();
        board = FindObjectOfType<BoardManager>();
        if (board == null) { Debug.LogError("BoardManager not found"); return; }
        StartCoroutine(SlowRandomBorderTile());
    }

    float GetSlowEffectAmount()
    {
        switch (rankValue)
        {
            case 1: return 0.8f;
            case 2: return 0.7f;
            case 3: return 0.5f;
            case 4: return 0.3f;
            default: return 0.8f;
        }
    }

    IEnumerator SlowRandomBorderTile()
    {
        while (true)
        {
            TileController[] borderTiles = GetBorderTiles();
            if (borderTiles.Length == 0)    continue;

            TileController selectedTile = borderTiles[Random.Range(0, borderTiles.Length)];
            SpriteRenderer sr = selectedTile.GetComponent<SpriteRenderer>();
            Sprite originalSprite = sr != null ? sr.sprite : null;
            float frozenAmount = GetSlowEffectAmount();

            // 冰冻前端部分(播放冰冻动画，将地块替换为ice)
            StartCoroutine(PlayFreezeAnimation());
            if (sr != null && frozenTileSprite != null) sr.sprite = frozenTileSprite;
            
            // 冰冻逻辑部分
            selectedTile.SetTileState(1, frozenAmount, frozenDuration);
            selectedTile.ApplyEffect(frozenAmount, frozenDuration);
            selectedTile.StartCoroutine(selectedTile.ApplyEffectForDuration());

            yield return new WaitForSeconds(frozenDuration);

            // 恢复正常
            if (sr != null && originalSprite != null) sr.sprite = originalSprite;
            selectedTile.SetTileState(0);
            
            // 防御塔冷却
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator PlayFreezeAnimation()
    {
        if (freezeFrames == null || freezeFrames.Length < 4 || frozenTowerRenderer == null)
            yield break;

        frozenTowerRenderer.sprite = freezeFrames[0];
        yield return new WaitForSeconds(0.1f);

        frozenTowerRenderer.sprite = freezeFrames[1];
        yield return new WaitForSeconds(0.1f);

        frozenTowerRenderer.sprite = freezeFrames[2];
        yield return new WaitForSeconds(0.1f);

        frozenTowerRenderer.sprite = freezeFrames[3];
        float holdDuration = frozenDuration - 0.3f;
        yield return new WaitForSeconds(holdDuration);

        frozenTowerRenderer.sprite = freezeFrames[0];
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
