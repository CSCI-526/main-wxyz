using UnityEngine;
using System.Collections;


public class TankTowerController : TowerController
{
    public GameObject projectilePrefab;
    public float fireRate;
    private float lastFireTime;

    void Update()
    {
        FireAtEnemy();
    }
    void Start()
    {
        base.Start();
    }

    void FireAtEnemy()
    {
        if (Time.time - lastFireTime >= fireRate)
        {
            Enemy targetEnemy = FindTargetEnemy();
            if (targetEnemy != null)
            {
                ShootProjectile(targetEnemy);
                lastFireTime = Time.time;
            }
        }
    }

    Enemy FindTargetEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
        Enemy targetEnemy = null;
        float lowestDistance = int.MaxValue; // 设置一个极大的初始值

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null && enemy.distance < lowestDistance) // 选择 enemyIndex 最小的敌人
                {
                    lowestDistance = enemy.distance;
                    targetEnemy = enemy;
                }
            }
        }
        return targetEnemy;
    }


    /*void ShootProjectile(Enemy target)
    {
        for (int i = 0; i < rankValue; i++) {
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.target = target;
            projectile.damage = 50f;
            //Debug.Log("Tower Rank: " + rankValue + " | Damage: " + projectile.damage);
            projectile.InitializeProjectile();
        }
    }*/

    void ShootProjectile(Enemy target)
    {
        StartCoroutine(FireProjectilesWithDelay(target));
    }

    IEnumerator FireProjectilesWithDelay(Enemy target)
    {
        float fireInterval = 0.1f; // 每颗子弹的间隔时间，可调整

        for (int i = 0; i < rankValue; i++)
        {
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.target = target;
            projectile.damage = 40f;
            projectile.InitializeProjectile();

            yield return new WaitForSeconds(fireInterval); // 等待一定时间再发射下一颗子弹
        }
    }

     /*float CalculateDamage()
     {
         switch (rankValue)  // 直接根据 rankValue 返回固定伤害
         {
             case 1: return 20f;  
             case 2: return 40f;  
             case 3: return 60f;  
             case 4: return 80f;  
             default: return 10f;  
         }
    }*/
}
