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
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                return collider.GetComponent<Enemy>();
            }
        }
        return null;
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