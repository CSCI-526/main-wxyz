using System.Collections;
using UnityEngine;

public class BurningTowerController : TowerController
{
    private BoardManager board;
    public float burnDuration = 3f;
    public Sprite[] burnFrames;  // 四帧动画
    private SpriteRenderer burningTowerRenderer;

    void Start()
    {
        base.Start();
        burningTowerRenderer = GetComponent<SpriteRenderer>();
        board = FindObjectOfType<BoardManager>();
        if (board == null)
        {
            Debug.LogError("BoardManager not found in the scene!");
            return;
        }

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
            selectedTile.SetTileState(2, burnDamage, burnDuration);

            // 变色显示燃烧状态
            SpriteRenderer sr = selectedTile.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = Color.red;  // 用红色表示燃烧
            }

            selectedTile.ApplyEffect(burnDamage, burnDuration);
            selectedTile.StartCoroutine(selectedTile.ApplyEffectForDuration());

            // 播放燃烧动画
            StartCoroutine(PlayBurnAnimation());

            yield return new WaitForSeconds(5f); // 每 5 秒触发一次
        }
    }

    IEnumerator PlayBurnAnimation()
    {
        if (burnFrames == null || burnFrames.Length < 3 || burningTowerRenderer == null)
            yield break;

        // Step 1: 点燃瞬间（帧1）
        burningTowerRenderer.sprite = burnFrames[0];
        yield return new WaitForSeconds(0.01f);

        // Step 2: 持续燃烧状态（帧2）
        burningTowerRenderer.sprite = burnFrames[1];
        float holdDuration = burnDuration - 0.01f;
        yield return new WaitForSeconds(holdDuration);


        // Step 3: 回到默认塔（帧0）
        burningTowerRenderer.sprite = burnFrames[0];
    }

    TileController[] GetBorderTiles()
    {
        if (board == null || board.tiles == null) return new TileController[0];

        System.Collections.Generic.List<TileController> borderTiles = new System.Collections.Generic.List<TileController>();

        for (int i = 0; i < board.rows; i++)
        {
            for (int j = 0; j < board.columns; j++)
            {
                if (i == 0 || i == board.rows - 1 || j == 0 || j == board.columns - 1)
                {
                    TileController tile = board.tiles[i, j];

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
