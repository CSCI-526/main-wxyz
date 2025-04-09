using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using System.Collections.Generic;

public class EnemySpawn : MonoBehaviour
{
    [Header("Custom Wave Configuration")]
    public bool enableCustomConfig = false;
    public bool enableSpawnFlag = false;
    public List<EnemyWave> enemyWaves;

    [Header("General Enemy Spawn Configuration")]
    public GameObject enemyPrefab1;                      // 敌人类型
    public GameObject enemyPrefab2;                      // 敌人类型
    public EnemyData enemyData;

    private int enemyCount = 5;                         // 这一波的敌人数量
    private int bigEnemyCount = 1;
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
        if (enableCustomConfig == true)
        {
            foreach (var wave in enemyWaves)
            {
                for (int i = 0; i < wave.count; i++)
                {
                    Debug.Log("Spawning enemy at: " + spawnPoint.position);
                    GameObject enemy = Instantiate(wave.enemyPrefab, spawnPoint.position, Quaternion.identity);
                    enemy.GetComponent<Enemy>().InitiateEnemy(path, wave.enemyHealth, wave.enemySpeed, wave.enemyCoin);
                    yield return new WaitForSeconds(wave.timeBetweenEnemies);
                }
<<<<<<< HEAD
                // yield return new WaitForSeconds(wave.timeAfterWave);
                if (enableSpawnFlag)
                {
                    TutGameManager.Instance.setSpawnFlag(false);
                    while (!TutGameManager.Instance.getSpawnFlag())
                    { 
=======

                // yield return new WaitForSeconds(wave.timeAfterWave);
                if (enableSpawnFlag)
                {
                    yield return new WaitForSeconds(0.1f);
                    TutGameManager.Instance.setSpawnFlag(false);
                    while (!TutGameManager.Instance.getSpawnFlag())
                    {
>>>>>>> f2e03a7c992f933402dfb771e2b6aebdfe3b3d24
                        yield return new WaitForSeconds(0.3f);
                    }
                }
                else
                {
                    yield return new WaitForSeconds(wave.timeAfterWave);
                }
<<<<<<< HEAD
                
                
=======
>>>>>>> f2e03a7c992f933402dfb771e2b6aebdfe3b3d24
            }
        }

        float currentMaxHealth = 100f;
        float currentMaxSpeed = 1f;
        int currentCoin = 5;
        int enemyIndex = 1;
        while(true)
        {
            for(int i = 0; i < enemyCount - bigEnemyCount; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab1, spawnPoint.position, Quaternion.identity);
                enemy.GetComponent<Enemy>().InitiateEnemy(path, currentMaxHealth, currentMaxSpeed, currentCoin);
                enemy.GetComponent<Enemy>().index = enemyIndex;
                enemyIndex ++;
                yield return new WaitForSeconds(timeBetweenEnemies);
            }
            for(int i = 0; i < bigEnemyCount; i++)
            {
                GameObject bigEnemy = Instantiate(enemyPrefab2, spawnPoint.position, Quaternion.identity);
                bigEnemy.GetComponent<Enemy>().InitiateEnemy(path, currentMaxHealth*1.8f, currentMaxSpeed*0.8f, currentCoin*2);
                bigEnemy.GetComponent<Enemy>().index = enemyIndex;
                enemyIndex ++;
                yield return new WaitForSeconds(timeBetweenEnemies+0.15f);
            }
            yield return new WaitForSeconds(timeAfterWave);
            enemyCount ++;
            if (enemyCount > 3*bigEnemyCount)
            {
                bigEnemyCount ++;
            }
            if (timeBetweenEnemies > 0.5f)
            {
                timeBetweenEnemies -= 0.5f;
            }
            else if (timeBetweenEnemies > 0.3f)
            {
                timeBetweenEnemies -= 0.1f;
                // timeAfterWave -= 0.7f;
            }
            else
            {
                currentMaxHealth += 15;
            }
            if (timeAfterWave > 3f)
            {
                timeAfterWave --;
            }
            currentMaxHealth += 10;
        }  
    }
}

[System.Serializable]
public class EnemyWave
{
    public GameObject enemyPrefab;
    public float enemyHealth;
    public float enemySpeed;
    public int enemyCoin;
    public float timeBetweenEnemies;
    public float timeAfterWave;
    public int count;
}