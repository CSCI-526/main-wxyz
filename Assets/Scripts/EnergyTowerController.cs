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
        rankValue = Mathf.Clamp(rankValue, 1, 4); //保险措施
        currentDamage = GetMinDamage(rankValue);
        attackDamage = GetMinDamage(rankValue);
    }
    void Update()
    {
        if (!isAttacking && Time.time - lastCheckTime >= checkInterval)
        {
            AcquireLowestIndexEnemy(); //用新的寻敌逻辑
            lastCheckTime = Time.time;
        }
        if (currentTarget != null && currentTarget.IsAlive)
        {
            UpdateBeamVisual();
            ApplyContinuousDamage();
        }
        else if (isAttacking)
        {
            isAttacking = false;
            DisableBeam();
            currentDamage = GetMinDamage(rankValue);
        }
    }
    //采用index选择最前面的敌人
    private void AcquireLowestIndexEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
        Enemy targetEnemy = null;
        int lowestIndex = int.MaxValue;

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null && enemy.index < lowestIndex) //选择index最小的敌人
                {
                    lowestIndex = enemy.index;
                    targetEnemy = enemy;
                }
            }
        }
        if (targetEnemy != null)
        {
            isAttacking = true;
            currentTarget = targetEnemy;
            Debug.Log($"锁定目标: {currentTarget.name} | Index: {currentTarget.index}");
        }
        else
        {
            Debug.Log("未找到有效目标");
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
        currentTarget.EnemyTakeDamage(currentDamage * Time.deltaTime);
        //确保击杀后再检测新敌人
        if (!currentTarget.IsAlive)
        {
            isAttacking = false;
            currentTarget = null;
            AcquireLowestIndexEnemy(); //重新获取最前面的敌人
        }
    }

    public override void UpgradeTower()
    {
        if (rankValue < 4)
        {
            rankValue++;
            attackRange *= 1.2f;
            //直接更新最小值最大值和增长速率
            InitializeTowerStats();

            ReplaceTowerBase(); //确保基础对象不丢失
        }
    }

    private void DisableBeam()
    {
        if (energyBeam) energyBeam.enabled = false;
    }

    //确保EnergyTower不会因ReplaceTowerBase()丢失基础组件
    private float GetMinDamage(int level)
    {
        switch (level)
        {
            case 1: return 10f;
            case 2: return 20f;
            case 3: return 30f;
            case 4: return 40f;
            default: return 10f;
        }
    }
    private float GetMaxDamage(int level)
    {
        switch (level)
        {
            case 1: return 50f;
            case 2: return 60f;
            case 3: return 70f;
            case 4: return 80f;
            default: return 50f;
        }
    }
    private float GetDamageGrowthRate(int level)
    {
        switch (level)
        {
            case 1: return 10f;
            case 2: return 12f;
            case 3: return 15f;
            case 4: return 18f;
            default: return 10f;
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
