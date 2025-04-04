﻿using UnityEngine;
using System.Linq;

public class EnergyTowerController : TowerController
{
    [Header("Energy Tower Settings")]
    public LineRenderer energyBeam;
    public Gradient beamColorGradient;

    private float currentDamage;
    private Enemy currentTarget;
    private bool isAttacking = false;
    private float checkInterval = 0.2f;
    private float lastCheckTime;

    public override void Start()
    {
        base.Start();
        InitializeTowerStats();
        energyBeam.enabled = false;
        energyBeam.useWorldSpace = true;
    }

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
                if (enemy != null && enemy.IsAlive && enemy.distance < minDistance) // **选择 distance最小的敌人**
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
            // Debug.Log($"锁定目标: {currentTarget.name} | Index: {currentTarget.index}");
            currentDamage = GetMinDamage(rankValue);
        }
        else
        {
            isAttacking = false;
            DisableBeam();


            // Debug.Log("未找到有效目标");
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

        currentTarget.EnemyTakeDamage(currentDamage * Time.deltaTime, "energy");

        // **确保击杀后再检测新敌人**
        if (!currentTarget.IsAlive)
        {
            isAttacking = false;
            currentTarget = null;
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
        }
    }

    private void DisableBeam()
    {
        if (energyBeam) energyBeam.enabled = false;
    }

    // **确保 `EnergyTower` 不会因 `ReplaceTowerBase()` 丢失基础组件**


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

    /*private void OnDestroy()
    {

    }
    */

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}