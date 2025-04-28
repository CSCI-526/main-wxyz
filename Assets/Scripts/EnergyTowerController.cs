using UnityEngine;
using System.Linq;
using System.Collections;


public class EnergyTowerController : TowerController
{
    [Header("Energy Tower Settings")]
    public LineRenderer energyBeam;
    public Gradient beamColorGradient;
    public Sprite[] energyFrames;
    private SpriteRenderer towerRenderer;

    private float currentDamage;
    private Enemy currentTarget;
    private bool isAttacking = false;
    private float checkInterval = 0.2f;
    private float lastCheckTime;

    public Transform firePoint;
    
    public override void Start()
    {
        base.Start();
        InitializeTowerStats();
        energyBeam.enabled = false;
        energyBeam.useWorldSpace = true;

        towerRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(PlayEnergyAnimationLoop());

        //自动创建Firepoint
        CreateFirePoint();
    }

    private void CreateFirePoint()
    {
        GameObject firePointObj = new GameObject("FirePoint");
        firePointObj.transform.SetParent(this.transform);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Vector3 centerOffset = sr.bounds.center - transform.position;

           //加一点偏移
            centerOffset += new Vector3(0f, 0f, 0f); // 
            firePointObj.transform.localPosition = centerOffset;
        }
        else
        {
            firePointObj.transform.localPosition = Vector3.zero;
        }

        firePoint = firePointObj.transform;
    }





    private void InitializeTowerStats()
    {
        rankValue = Mathf.Clamp(rankValue, 1, 4);
        currentDamage = GetMinDamage(rankValue);
        attackDamage = GetMinDamage(rankValue);
    }

    void Update()
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

    /* private void UpdateBeamVisual()
    {
        if (!energyBeam) return;

        if (currentTarget != null && currentTarget.IsAlive)
        {
            energyBeam.enabled = true;

            // 塔中心偏移，可以调
            Vector3 towerCenter = transform.position + new Vector3(0f, 0.1f, 0f);

            // 敌人中心偏移，可以调
            Vector3 enemyCenter = currentTarget.transform.position + new Vector3(0f, 0.5f, 0f);

            energyBeam.SetPosition(0, towerCenter);
            energyBeam.SetPosition(1, enemyCenter);

            float t = (currentDamage - GetMinDamage(rankValue)) / (GetMaxDamage(rankValue) - GetMinDamage(rankValue));
            energyBeam.startColor = beamColorGradient.Evaluate(t);
            energyBeam.endColor = beamColorGradient.Evaluate(t);
        }
        else
        {
            energyBeam.enabled = false;
        }
    } */

    private void UpdateBeamVisual()
    {
        if (!energyBeam || firePoint == null || currentTarget == null || currentTarget.hitPoint == null)
            return;

        energyBeam.enabled = true;
        energyBeam.SetPosition(0, firePoint.position);
        energyBeam.SetPosition(1, currentTarget.hitPoint.position);

        float t = (currentDamage - GetMinDamage(rankValue)) / (GetMaxDamage(rankValue) - GetMinDamage(rankValue));
        energyBeam.startColor = beamColorGradient.Evaluate(t);
        energyBeam.endColor = beamColorGradient.Evaluate(t);
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

    public override void UpgradeTower()
    {
        if (rankValue < 4)
        {
            rankValue++;
            attackRange *= 1.2f;
            InitializeTowerStats();
            ReplaceTowerBase();
        }
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
            case 1: return 120f;
            case 2: return 180f;
            case 3: return 240f;
            case 4: return 300f;
            default: return 120f;
        }
    }

    private float GetDamageGrowthRate(int level)
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

    IEnumerator PlayEnergyAnimationLoop()
    {
        while (true)
        {
            if (energyFrames == null || energyFrames.Length < 5 || towerRenderer == null)
                yield break;

            towerRenderer.sprite = energyFrames[0];
            yield return new WaitForSeconds(0.3f);
            towerRenderer.sprite = energyFrames[1];
            yield return new WaitForSeconds(0.3f);
            towerRenderer.sprite = energyFrames[2];
            yield return new WaitForSeconds(0.3f);
            towerRenderer.sprite = energyFrames[3];
            yield return new WaitForSeconds(0.3f);
            towerRenderer.sprite = energyFrames[4];
            yield return new WaitForSeconds(3f);


            towerRenderer.sprite = energyFrames[0];
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}