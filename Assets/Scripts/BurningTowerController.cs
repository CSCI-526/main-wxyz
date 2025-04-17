using System.Collections;
using UnityEngine;

public class BurningTowerController : TowerController
{
    public float burnDuration = 3f;
    public Sprite[] burnFrames;  // 四帧动画
    private SpriteRenderer burningTowerRenderer;
    public Sprite burningTileSprite;

    private BoardManager board;

    void Start()
    {
        base.Start();
        burningTowerRenderer = GetComponent<SpriteRenderer>();
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
            TileController[] borderTiles = GetBorderTiles();
            if (borderTiles.Length == 0) continue;

            TileController selectedTile = borderTiles[Random.Range(0, borderTiles.Length)];
            float burnDamage = GetBurnDamage();

            // 变色显示燃烧状态
            SpriteRenderer sr = selectedTile.GetComponent<SpriteRenderer>();

            Sprite originalSprite = sr != null ? sr.sprite : null;
            if (sr != null && burningTileSprite != null) sr.sprite = burningTileSprite;

            selectedTile.SetTileState(2, burnDamage, burnDuration);
            selectedTile.ApplyEffect(burnDamage, burnDuration);
            selectedTile.StartCoroutine(selectedTile.ApplyEffectForDuration());

            StartCoroutine(PlayBurnAnimation());
            yield return new WaitForSeconds(burnDuration);
            

            if (sr != null && originalSprite != null) sr.sprite = originalSprite;
            selectedTile.SetTileState(0);
            yield return new WaitForSeconds(2f); // 每 5 秒触发一次



        }
    }

    IEnumerator PlayBurnAnimation()
    {
        if (burnFrames == null || burnFrames.Length < 3 || burningTowerRenderer == null)
            yield break;

        // 帧1
        burningTowerRenderer.sprite = burnFrames[0];
        yield return new WaitForSeconds(0.1f);
        // 帧2
        burningTowerRenderer.sprite = burnFrames[1];
        yield return new WaitForSeconds(0.1f);
        // 帧3
        burningTowerRenderer.sprite = burnFrames[2];
        yield return new WaitForSeconds(0.1f);
        // 帧4(保持燃烧)
        burningTowerRenderer.sprite = burnFrames[3];
        float holdDuration = burnDuration - 0.3f;
        yield return new WaitForSeconds(holdDuration);
        // 回到帧0
        burningTowerRenderer.sprite = burnFrames[0];
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
