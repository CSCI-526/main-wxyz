using UnityEngine;

public class TutEnergyTowerController : TowerController
{
    [Header("Energy Tower Settings")]
    public LineRenderer energyBeam;
    public Gradient beamColorGradient;

    private float currentDamage;
    private Enemy currentTarget;
    private bool isAttacking = false;
    private float checkInterval = 0.2f;
    private float lastCheckTime;

    private void Start()
    {
        base.Start();
        towerName = "TutEnergyTower";

        InitializeTowerStats();
        energyBeam.enabled = false;
        energyBeam.useWorldSpace = true;
    }

    private void Update()
    {
        if (!isAttacking && Time.time - lastCheckTime >= checkInterval)
        {
            AcquireLowestDistanceEnemy();
            lastCheckTime = Time.time;
        }

        if (currentTarget != null && currentTarget.IsAlive)
        {
            UpdateBeamVisual();
            ApplyContinuousDamage();
        }
        else
        {
            isAttacking = false;
            DisableBeam();
            currentDamage = GetMinDamage(rankValue);
        }
    }

    private void InitializeTowerStats()
    {
        rankValue = Mathf.Clamp(rankValue, 1, 4);
        currentDamage = GetMinDamage(rankValue);
        attackDamage = GetMinDamage(rankValue);
    }

    private void AcquireLowestDistanceEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
        Enemy targetEnemy = null;
        float minDistance = float.MaxValue;

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null && enemy.IsAlive && enemy.distance < minDistance)
                {
                    minDistance = enemy.distance;
                    targetEnemy = enemy;
                }
            }
        }

        if (targetEnemy != null)
        {
            isAttacking = true;
            currentTarget = targetEnemy;
            currentDamage = GetMinDamage(rankValue);
        }
        else
        {
            isAttacking = false;
            DisableBeam();
        }
    }

    private void UpdateBeamVisual()
    {
        if (!energyBeam) return;

        if (currentTarget != null && currentTarget.IsAlive)
        {
            energyBeam.enabled = true;
            energyBeam.SetPosition(0, transform.position);
            energyBeam.SetPosition(1, currentTarget.transform.position);

            float t = (currentDamage - GetMinDamage(rankValue)) /
                      (GetMaxDamage(rankValue) - GetMinDamage(rankValue));
            energyBeam.startColor = beamColorGradient.Evaluate(t);
            energyBeam.endColor = beamColorGradient.Evaluate(t);
        }
        else
        {
            energyBeam.enabled = false;
        }
    }

    private void ApplyContinuousDamage()
    {
        if (currentTarget == null || !currentTarget.IsAlive) return;

        currentDamage = Mathf.Clamp(
            currentDamage + GetDamageGrowthRate(rankValue) * Time.deltaTime,
            GetMinDamage(rankValue),
            GetMaxDamage(rankValue)
        );

        float deltaDamage = currentDamage * Time.deltaTime;
        currentTarget.EnemyTakeDamage(deltaDamage, "energy");

        // 更新统计
        if (TutGameManager.Instance != null)
            TutGameManager.Instance.AddDamageFromEnergyTower(deltaDamage);

        if (!currentTarget.IsAlive)
        {
            isAttacking = false;
            currentTarget = null;
            AcquireLowestDistanceEnemy();
        }
    }

    private void DisableBeam()
    {
        if (energyBeam) energyBeam.enabled = false;
    }

    private float GetMinDamage(int level)
    {
        switch (level)
        {
            case 1: return 30f;
            case 2: return 50f;
            case 3: return 70f;
            case 4: return 90f;
            default: return 30f;
        }
    }

    private float GetMaxDamage(int level)
    {
        switch (level)
        {
            case 1: return 80f;
            case 2: return 120f;
            case 3: return 160f;
            case 4: return 200f;
            default: return 80f;
        }
    }

    private float GetDamageGrowthRate(int level)
    {
        switch (level)
        {
            case 1: return 50f;
            case 2: return 60f;
            case 3: return 80f;
            case 4: return 120f;
            default: return 40f;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
