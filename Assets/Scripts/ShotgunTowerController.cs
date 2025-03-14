using UnityEngine;

public class ShotgunTowerController : TowerController
{
    public GameObject projectilePrefab;
    public float fireRate;
    public int pelletCount = 5; //Number of bullets fired in one shot
    public float spreadAngle = 30f; //Cone spread angle
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
                ShootShotgun(targetEnemy);
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

    void ShootShotgun(Enemy target)
    {
        for (int i = 0; i < pelletCount; i++)
        {
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            ShotgunProjectile projectile = projectileObj.GetComponent<ShotgunProjectile>();
            if (projectile != null)
            {
                float randomAngle = Random.Range(-spreadAngle / 2, spreadAngle / 2); //Random angle
                projectile.SetDirection(target.transform.position, randomAngle);
                projectile.damage = attackDamage / pelletCount; //Damage distribution
                projectile.speed = 10f;
            }
        }
    }
}
