using UnityEngine;
using System.Collections;

public class TutFrozenTowerController : TowerController
{
    public float slowDuration = 3f;
    public Sprite frozenTileSprite;

    private BoardManager board;

    void Start()
    {
        base.Start();
        board = FindObjectOfType<BoardManager>();
        if (board == null) { Debug.LogError("BoardManager not found"); return; }
        StartCoroutine(SlowRandomBorderTile());
    }

    float GetSlowEffectAmount()
    {
        switch (rankValue)
        {
            case 1: return 0.6f;
            case 2: return 0.5f;
            case 3: return 0.4f;
            case 4: return 0.2f;
            default: return 0.8f;
        }
    }

    IEnumerator SlowRandomBorderTile()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            TileController[] borderTiles = GetBorderTiles();
            if (borderTiles.Length == 0) continue;

            TileController selectedTile = borderTiles[Random.Range(0, borderTiles.Length)];
            float slowAmount = GetSlowEffectAmount();

            SpriteRenderer sr = selectedTile.GetComponent<SpriteRenderer>();
            Sprite originalSprite = sr ? sr.sprite : null;
            if (sr && frozenTileSprite) sr.sprite = frozenTileSprite;

            selectedTile.SetTileState(1, slowAmount, slowDuration);
            selectedTile.ApplyEffect(slowAmount, slowDuration);
            selectedTile.StartCoroutine(selectedTile.ApplyEffectForDuration());

            yield return new WaitForSeconds(slowDuration);

            if (sr && originalSprite) sr.sprite = originalSprite;
            selectedTile.SetTileState(0);
        }
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
