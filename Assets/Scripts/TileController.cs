using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public Vector2Int gridPosition;
    public TowerController towerOnTile;
    private int tileState = 0; // 0: 无状态, 1: 冰冻, 2: 燃烧
    private List<Enemy> enemiesOnTile = new List<Enemy>(); // 当前 Tile 上的敌人

    void Awake()
    {
        // 确保 Tile 拥有 Collider
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider2D>(); // 自动添加 BoxCollider2D
        }
        col.isTrigger = true; // 设为触发器
    }

    // 设置 Tile 状态
    public void SetTileState(int state)
    {
        tileState = state;
    }

    // 获取当前 Tile 状态
    public int GetTileState()
    {
        return tileState;
    }

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
            }
        }
    }

    // 当敌人离开 Tile 时，从列表移除
    /*void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // 只移除 Tag 为 "enemy" 的对象
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemiesOnTile.Remove(enemy);
            }
        }
    }*/

    // 获取当前 Tile 上的所有敌人
    public List<Enemy> GetEnemiesOnTile()
    {
        return enemiesOnTile;
    }
}