using System.Collections;
using UnityEngine;

public class BurningTowerController : TowerController
{
    public float burnDuration = 3f;
    public Sprite burningTileSprite;

    private BoardManager board;

    void Start()
    {
        base.Start();
        board = FindObjectOfType<BoardManager>();
        if (board == null) { Debug.LogError("BoardManager not found"); return; }
        StartCoroutine(BurnRandomBorderTile());
    }

    public float GetBurnDamage()
    {
        switch (rankValue)
        {
            case 1: return 20;
            case 2: return 25;
            case 3: return 35;
            case 4: return 50;
            default: return 20;
        }
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

            SpriteRenderer sr = selectedTile.GetComponent<SpriteRenderer>();
            Sprite originalSprite = sr != null ? sr.sprite : null;
            if (sr != null && burningTileSprite != null) sr.sprite = burningTileSprite;

            selectedTile.SetTileState(2, burnDamage, burnDuration);
            selectedTile.ApplyEffect(burnDamage, burnDuration);
            selectedTile.StartCoroutine(selectedTile.ApplyEffectForDuration());

            yield return new WaitForSeconds(burnDuration);

            if (sr != null && originalSprite != null) sr.sprite = originalSprite;
            selectedTile.SetTileState(0);
        }
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
