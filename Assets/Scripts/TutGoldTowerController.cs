using UnityEngine;
using System.Collections;

public class TutGoldTowerController : TowerController
{
    public int goldPerCycle = 10; // 每次生产的金币数量
    public float generateInterval = 5f; // 生产金币的间隔时间
    public Sprite[] goldFrames;
    private SpriteRenderer goldRenderer;
    private float lastGenerateTime;
    private TutGameManager tutgameManager;

    void Start()
    {
        base.Start();
        goldRenderer = GetComponent<SpriteRenderer>();
        tutgameManager = FindObjectOfType<TutGameManager>();
        lastGenerateTime = Time.time;

        StartCoroutine(GoldAnimationLoop());
    }

    void Update()
    {
        // Debug.Log("GoldTower Update Running...");
        GenerateGold();
    }

    void GenerateGold()
    {
        if (Time.time - lastGenerateTime >= generateInterval)
        {
            //Debug.Log("GoldTower Generating Gold...");
            if (tutgameManager != null)
            {
                tutgameManager.AddCoin(goldPerCycle);
                //Debug.Log("Gold Tower added " + goldPerCycle + " coins.");
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
            if (goldFrames == null || goldFrames.Length < 5 || goldRenderer == null)
                yield break;

            goldRenderer.sprite = goldFrames[0];
            yield return new WaitForSeconds(0.6f);
            goldRenderer.sprite = goldFrames[1];
            yield return new WaitForSeconds(0.6f);
            goldRenderer.sprite = goldFrames[2];
            yield return new WaitForSeconds(0.6f);
            goldRenderer.sprite = goldFrames[3];
            yield return new WaitForSeconds(0.6f);
            goldRenderer.sprite = goldFrames[4];
            yield return new WaitForSeconds(3f);

            goldRenderer.sprite = goldFrames[0];
        }
    }

    void OnDestroy()
    {
        if (tutgameManager != null)
        {
            int bonusGold = 60 * (int)Mathf.Pow(2, rankValue - 1);
            tutgameManager.AddCoin(bonusGold);
        }
    }
}