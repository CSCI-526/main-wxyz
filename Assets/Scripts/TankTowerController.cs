using UnityEngine;
using System.Collections;


public class TankTowerController : TowerController
{
    public GameObject projectilePrefab;
    public float fireRate;
    private float lastFireTime;
    private Transform turretTransform;
    public GameObject turretPrefab;
    private Enemy currentTarget;

    void Start()
    {
        base.Start();
        CreateTurret();
    }

    void Update()
    {
        UpdateTarget();
        UpdateTurretRotation();
        FireIfReady();
    }

    void UpdateTarget()
    {
        currentTarget = FindTargetEnemy();
    }

    void UpdateTurretRotation()
    {
        if (currentTarget == null || turretTransform == null) return;

        Vector2 dir = currentTarget.transform.position - turretTransform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);

        // 平滑插值旋转
        turretTransform.rotation = Quaternion.Lerp(
            turretTransform.rotation,
            targetRotation,
            Time.deltaTime * 15f // 可调快慢
        );
    }

    void FireIfReady()
    {
        if (Time.time - lastFireTime >= fireRate && currentTarget != null)
        {
            ShootProjectile(currentTarget);
            lastFireTime = Time.time;
        }
    }


    void FireAtEnemy()
    {
        if (Time.time - lastFireTime >= fireRate)
        {
            Enemy targetEnemy = FindTargetEnemy();
            if (targetEnemy != null)
            {
                currentTarget = targetEnemy; // 保存目标
                ShootProjectile(targetEnemy);
                lastFireTime = Time.time;
            }
        }
    }

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

    void CreateTurret()
    {
        if (turretPrefab != null)
        {
            GameObject turret = Instantiate(turretPrefab, transform);
            turret.name = "Turret";
            turret.transform.localPosition = Vector3.zero;
            turretTransform = turret.transform;
        }
        else
        {
            Debug.LogWarning("Turret Prefab 未设置！");
        }
    }
}