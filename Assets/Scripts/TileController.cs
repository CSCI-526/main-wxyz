using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public Vector2Int gridPosition;
    public TowerController towerOnTile;
    private int tileState = 0; // 0: 无状态, 1: 冰冻, 2: 燃烧
    private List<Enemy> enemiesOnTile = new List<Enemy>();
    private int burnDamage = 0;
    private float burnDuration = 0f;

    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider2D>(); // 自动添加 BoxCollider2D
        }
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

    public int GetTileState()
    {
        return tileState;
    }

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
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemiesOnTile.Remove(enemy);
            }
        }
    }

    public List<Enemy> GetEnemiesOnTile()
    {
        return enemiesOnTile;
    }

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

