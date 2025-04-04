using UnityEngine;
<<<<<<< Updated upstream

public class TutEnergyTowerController : TowerController
=======
using System.Linq;

public class TutEnergyTowerController : EnergyTowerController
>>>>>>> Stashed changes
{
    [Header("Energy Tower Settings")]
    public LineRenderer energyBeam;
    public Gradient beamColorGradient;

    private float currentDamage;
    private Enemy currentTarget;
    private bool isAttacking = false;
    private float checkInterval = 0.2f;
    private float lastCheckTime;

<<<<<<< Updated upstream
    private void Start()
    {
        base.Start();
        towerName = "TutEnergyTower";

=======
    public override void Start()
    {
        base.Start();
>>>>>>> Stashed changes
        InitializeTowerStats();
        energyBeam.enabled = false;
        energyBeam.useWorldSpace = true;
    }

<<<<<<< Updated upstream
    private void Update()
    {
        if (!isAttacking && Time.time - lastCheckTime >= checkInterval)
        {
            AcquireLowestDistanceEnemy();
=======
    private void InitializeTowerStats()
    {
        rankValue = Mathf.Clamp(rankValue, 1, 4); // 保险措施
        currentDamage = GetMinDamage(rankValue);
        attackDamage = GetMinDamage(rankValue);
    }

    void Update()
    {
        if (!isAttacking && Time.time - lastCheckTime >= checkInterval)
        {
            AcquireLowestDistanceEnemy(); // **使用基于距离的寻敌逻辑**
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
    private void InitializeTowerStats()
    {
        rankValue = Mathf.Clamp(rankValue, 1, 4);
        currentDamage = GetMinDamage(rankValue);
        attackDamage = GetMinDamage(rankValue);
    }
=======
>>>>>>> Stashed changes

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
<<<<<<< Updated upstream
                if (enemy != null && enemy.IsAlive && enemy.distance < minDistance)
=======
                if (enemy != null && enemy.IsAlive && enemy.distance < minDistance) // **选择 distance最小的敌人**
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
=======
            // Debug.Log($"锁定目标: {currentTarget.name} | Index: {currentTarget.index}");
>>>>>>> Stashed changes
            currentDamage = GetMinDamage(rankValue);
        }
        else
        {
            isAttacking = false;
            DisableBeam();
<<<<<<< Updated upstream
=======


            // Debug.Log("未找到有效目标");
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
        float deltaDamage = currentDamage * Time.deltaTime;
        currentTarget.EnemyTakeDamage(deltaDamage, "energy");

        // 更新统计
        if (TutGameManager.Instance != null)
            TutGameManager.Instance.AddDamageFromEnergyTower(deltaDamage);

=======
        currentTarget.EnemyTakeDamage(currentDamage * Time.deltaTime, "energy");

        // **确保击杀后再检测新敌人**
>>>>>>> Stashed changes
        if (!currentTarget.IsAlive)
        {
            isAttacking = false;
            currentTarget = null;
<<<<<<< Updated upstream
            AcquireLowestDistanceEnemy();
=======
            AcquireLowestDistanceEnemy(); // 重新获取最前面的敌人
        }
    }

    public override void UpgradeTower()
    {
        if (rankValue < 4)
        {
            rankValue++;
            attackRange *= 1.2f;

            // ** 直接更新最小值、最大值和增长速率**
            InitializeTowerStats();

            ReplaceTowerBase(); // **确保基础对象不丢失**
>>>>>>> Stashed changes
        }
    }

    private void DisableBeam()
    {
        if (energyBeam) energyBeam.enabled = false;
    }

<<<<<<< Updated upstream
=======
    // **确保 `EnergyTower` 不会因 `ReplaceTowerBase()` 丢失基础组件**


>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
=======
    /*private void OnDestroy()
    {

    }
    */

>>>>>>> Stashed changes
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
<<<<<<< Updated upstream
}
=======
}
>>>>>>> Stashed changes
