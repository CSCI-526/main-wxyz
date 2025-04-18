using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public Vector2Int gridPosition;
    public TowerController towerOnTile;
    private int tileState = 0; 
    private List<Enemy> enemiesOnTile = new List<Enemy>();
    private float Damage = 0;
    private float Duration = 0f;
    
    public GameObject hover;
    public Sprite originalSprite;

    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider2D>(); // 自动添加 BoxCollider2D
        }
        col.isTrigger = true;
    }

    public void SetTileState(int state, float damage = 0, float duration = 0f)
    {
        tileState = state;
        Damage = damage;
        Duration = duration;
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
                    enemy.EnemyBurnEffect(Damage, Duration);
                } 
                else if (tileState == 1) 
                {
                    enemy.EnemySlowEffect(Damage, Duration);
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

    // 施加燃烧或冰冻效果给当前 Tile 上的所有敌人
    public void ApplyEffect(float damage, float duration)
    {
        foreach (Enemy enemy in new List<Enemy>(enemiesOnTile))
        {
            if (enemy != null)
            {   
                if(tileState == 2) 
                {
                    enemy.EnemyBurnEffect(damage, duration);
                } 
                else if(tileState == 1) 
                {
                    enemy.EnemySlowEffect(damage, duration);
                }
            }
        }
    }

    public IEnumerator ApplyEffectForDuration()
    {
        yield return new WaitForSeconds(Duration);
        // 恢复颜色和状态
        ResetTileState();
    }

    // 恢复 Tile 的状态
    public void ResetTileState()
    {
        SetTileState(0); 
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = originalSprite;
        }
    }

    public void LightOnHover()
    {
        if (hover != null)
        {
            hover.SetActive(true);
        }
    }

    public void DisableHover()
    {
        if (hover != null)
        {
            hover.SetActive(false);
        }
    }
}
