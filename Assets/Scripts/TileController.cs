using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public Vector2Int gridPosition;
    public TowerController towerOnTile;
    private int tileState = 0; // 0: 无状态, 1: 冰冻, 2: 燃烧
<<<<<<< HEAD
    private List<Enemy> enemiesOnTile = new List<Enemy>(); // 当前 Tile 上的敌人

    void Awake()
    {
        // 确保 Tile 拥有 Collider
=======
    private List<Enemy> enemiesOnTile = new List<Enemy>();
    private int burnDamage = 0;
    private float burnDuration = 0f;

    void Awake()
    {
>>>>>>> origin/ZixuanXu
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider2D>(); // 自动添加 BoxCollider2D
        }
<<<<<<< HEAD
        col.isTrigger = true; // 设为触发器
    }

    // 设置 Tile 状态
    public void SetTileState(int state)
    {
        tileState = state;
    }

    // 获取当前 Tile 状态
=======
        col.isTrigger = true;
    }

    public void SetTileState(int state, int damage = 0, float duration = 0f)
    {
        tileState = state;
        if (state == 2) // 燃烧状态
        {
            burnDamage = damage;
            burnDuration = duration;
        }
        else
        {
            burnDamage = 0;
            burnDuration = 0f;
        }
    }

>>>>>>> origin/ZixuanXu
    public int GetTileState()
    {
        return tileState;
    }

<<<<<<< HEAD
    // 当敌人进入 Tile 时，加入列表（基于 Tag 检测）
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // 只添加 Tag 为 "enemy" 的对象
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if(tileState == 1){
                enemy.EnemySlowEffect(0.5f, 2f);
            } else if(tileState == 2) {
                
            }
            
            if (enemy != null && !enemiesOnTile.Contains(enemy))
            {
                enemiesOnTile.Add(enemy);
=======
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && !enemiesOnTile.Contains(enemy))
            {
                enemiesOnTile.Add(enemy);
                
                // 如果Tile是燃烧状态，立即对进入的敌人施加燃烧效果
                if (tileState == 2)
                {
                    enemy.EnemyBurnEffect(burnDamage, burnDuration);
                }
>>>>>>> origin/ZixuanXu
            }
        }
    }

<<<<<<< HEAD
    // 当敌人离开 Tile 时，从列表移除
    /*void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // 只移除 Tag 为 "enemy" 的对象
=======
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
>>>>>>> origin/ZixuanXu
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemiesOnTile.Remove(enemy);
            }
        }
<<<<<<< HEAD
    }*/

    // 获取当前 Tile 上的所有敌人
=======
    }

>>>>>>> origin/ZixuanXu
    public List<Enemy> GetEnemiesOnTile()
    {
        return enemiesOnTile;
    }
<<<<<<< HEAD
}
=======

    // 施加燃烧效果给当前 Tile 上的所有敌人
    public void ApplyBurnEffect(int damage, float duration)
    {
        foreach (Enemy enemy in new List<Enemy>(enemiesOnTile))
        {
            if (enemy != null)
            {
                enemy.EnemyBurnEffect(damage, duration);
            }
        }
    }
}

>>>>>>> origin/ZixuanXu
