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
        TowerController tower = GetComponent<TowerController>();
        if (tower != null)
        {
            switch (tower.rankValue)
            {
                case 1: return 20f;
                case 2: return 25f;
                case 3: return 35f;
                case 4: return 50f;
                default: return 20f;
            }
        }
        return 20f;
    }

    IEnumerator BurnRandomBorderTile()
    {
        while (true)
        {
            TileController[] borderTiles = GetBorderTiles();
            if (borderTiles.Length == 0)
            {
                yield return null;
                continue;
            }

            TileController selectedTile = borderTiles[Random.Range(0, borderTiles.Length)];
            float burnDamage = GetBurnDamage();
            SpriteRenderer sr = selectedTile.GetComponent<SpriteRenderer>();
            Sprite originalSprite = sr != null ? sr.sprite : null;

            // 灼烧伤害前端部分(播放燃烧动画，将地块替换为lava)
            StartCoroutine(PlayBurnAnimation());
            if (sr != null && burningTileSprite != null) sr.sprite = burningTileSprite;

            // 灼烧伤害逻辑部分
            selectedTile.SetTileState(2, burnDamage, burnDuration);
            selectedTile.ApplyEffect(burnDamage, burnDuration);
            selectedTile.StartCoroutine(selectedTile.ApplyEffectForDuration());

            yield return new WaitForSeconds(burnDuration);

            // 恢复正常
            if (sr != null && originalSprite != null) sr.sprite = originalSprite;
            selectedTile.SetTileState(0);

            // 防御塔冷却
            yield return new WaitForSeconds(2f);
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
