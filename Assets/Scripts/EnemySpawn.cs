using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class EnemySpawn : MonoBehaviour
{
    [Header("Configuration Per Wave")]
    public GameObject enemyPrefab;                      // 敌人类型

    private int enemyCount = 5;                         // 这一波的敌人数量
    private float timeBetweenEnemies = 3f;              // 同一波内怪物生成的间隔
    private float timeAfterWave = 5f;                   // 与下一波的间隔时间

    private Transform spawnPoint;                       // 敌人出生点
    private Transform[] path = new Transform[4];        // 敌人路径点

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void EnemySpawnConfigInit()
    {
        BoardManager gridManager = Object.FindFirstObjectByType<BoardManager>();
        if (gridManager != null){
            if(gridManager.monsterSpawnTile != null)
            {
                SpriteRenderer redSprite = gridManager.monsterSpawnTile.GetComponent<SpriteRenderer>();
                if (redSprite != null)
                {
                    spawnPoint = redSprite.transform;
                }
            }
            if(gridManager.monsterTurnTile1 != null)
            {
                SpriteRenderer turnSprite1 = gridManager.monsterTurnTile1.GetComponent<SpriteRenderer>();
                if (turnSprite1 != null)
                {
                    path[0] = turnSprite1.transform;
                }
            }
            if(gridManager.monsterTurnTile2 != null)
            {
                SpriteRenderer turnSprite2 = gridManager.monsterTurnTile2.GetComponent<SpriteRenderer>();
                if (turnSprite2 != null)
                {
                    path[1] = turnSprite2.transform;
                }
            }
            if(gridManager.monsterTurnTile3 != null)
            {
                SpriteRenderer turnSprite3 = gridManager.monsterTurnTile3.GetComponent<SpriteRenderer>();
                if (turnSprite3 != null)
                {
                    path[2] = turnSprite3.transform;
                }
            }
            if(gridManager.monsterDestTile != null)
            {
                SpriteRenderer monsterDestSprite = gridManager.monsterDestTile.GetComponent<SpriteRenderer>();
                if (monsterDestSprite != null)
                {
                    path[3] = monsterDestSprite.transform;
                }
            }
        } 
    }

    public IEnumerator SpawnWaves()
    {
        float currentMaxHealth = 100f;
        int enemyIndex = 1;
        while(true)
        {
            for(int i = 0; i < enemyCount; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
                enemy.GetComponent<Enemy>().SetMaxHealth(currentMaxHealth);
                enemy.GetComponent<Enemy>().waypoints = path;
                enemy.GetComponent<Enemy>().index = enemyIndex;
                enemyIndex ++;
                yield return new WaitForSeconds(timeBetweenEnemies);
            }
            yield return new WaitForSeconds(timeAfterWave);
            enemyCount ++;
            if (timeBetweenEnemies > 0.3f)
            {
                timeBetweenEnemies -= 0.2f;
            }
            if (timeAfterWave > 3f)
            {
                timeAfterWave --;
            }
            currentMaxHealth += 10;
        }  
    }
}