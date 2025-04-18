using UnityEngine;
using System.Collections;

public class GoldTowerController : TowerController
{
    public int goldPerCycle = 5;
    public float generateInterval = 5f;
    public Sprite[] goldFrames;
    private SpriteRenderer goldRenderer;
    private float lastGenerateTime;
    private GameManager gameManager;

    void Start()
    {
        base.Start();
        goldRenderer = GetComponent<SpriteRenderer>();
        gameManager = FindObjectOfType<GameManager>();
        lastGenerateTime = Time.time;

        StartCoroutine(GoldAnimationLoop());
    }

    void Update()
    {
        GenerateGold();
    }

    void GenerateGold()
    {
        if (Time.time - lastGenerateTime >= generateInterval)
        {
            if (gameManager != null)
            {
                gameManager.AddCoin(goldPerCycle);
            }
            else
            {
                Debug.LogError("GoldTowerController: gameManager is NULL!");
            }

            lastGenerateTime = Time.time;
        }
    }

    IEnumerator GoldAnimationLoop()
    {
        while (true)
        {
            if (goldFrames == null || goldFrames.Length < 4 || goldRenderer == null)
                yield break;

            goldRenderer.sprite = goldFrames[0];
            yield return new WaitForSeconds(0.6f);
            goldRenderer.sprite = goldFrames[1];
            yield return new WaitForSeconds(0.6f);
            goldRenderer.sprite = goldFrames[2];
            yield return new WaitForSeconds(0.6f);
            goldRenderer.sprite = goldFrames[3];
            yield return new WaitForSeconds(3f);

            goldRenderer.sprite = goldFrames[0];
        }
    }

    void OnDestroy()
    {
        if (gameManager != null)
        {
            int bonusGold = 60 * (int)Mathf.Pow(2, rankValue - 1);
            gameManager.AddCoin(bonusGold);
        }
    }
}

