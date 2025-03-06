using UnityEngine;

public class TankTowerController : TowerController
{
    public GameObject projectilePrefab;
    public float fireRate;
    private float lastFireTime;

    void Update()
    {
        FireAtEnemy();
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
        int lowestIndex = int.MaxValue; // 设置一个极大的初始值

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null && enemy.index < lowestIndex) // 选择 enemyIndex 最小的敌人
                {
                    lowestIndex = enemy.index;
                    targetEnemy = enemy;
                }
            }
        }
        return targetEnemy;
    }


    void ShootProjectile(Enemy target)
    {
        GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        projectile.target = target;
        projectile.damage = attackDamage;
        projectile.InitializeProjectile();
    }
}
