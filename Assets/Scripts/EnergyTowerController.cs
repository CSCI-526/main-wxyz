using UnityEngine;

public class EnergyTowerController : TowerController
{
    [Header("Energy Tower Settings")]
    public float minDamage = 5f;
    public float maxDamage = 50f;
    public float damageGrowthRate = 2f;
    public LineRenderer energyBeam;
    public Gradient beamColorGradient;

    private float currentDamage;
    private Enemy currentTarget;
    private float checkInterval = 0.2f; // 检测间隔优化 <button class="citation-flag" data-index="5">
    private float lastCheckTime;

    public new void Start()
    {
        base.Start();
        currentDamage = minDamage;
        energyBeam.enabled = false; // 初始禁用射线 <button class="citation-flag" data-index="2">
        energyBeam.useWorldSpace = true; // 世界坐标系 <button class="citation-flag" data-index="2">
    }

    void Update()
    {
        if (Time.time - lastCheckTime >= checkInterval)
        {
            AcquireNewTarget();
            lastCheckTime = Time.time;
        }

        if (currentTarget != null && currentTarget.IsAlive)
        {
            UpdateBeamVisual();
            ApplyContinuousDamage();
        }
        else
        {
            DisableBeam();
            currentDamage = minDamage; // 重置伤害值 <button class="citation-flag" data-index="10">
        }
    }

    private void AcquireNewTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
        float closestDistance = Mathf.Infinity;
        currentTarget = null;

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy")) // 与TankTower一致的标签检测 <button class="citation-flag" data-index="1">
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null && enemy.IsAlive)
                {
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        currentTarget = enemy;
                    }
                }
            }
        }

        if (currentTarget != null)
        {
            Debug.Log($"锁定目标: {currentTarget.name} | 距离: {closestDistance:F2}"); // 调试信息 <button class="citation-flag" data-index="5">
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

            float t = (currentDamage - minDamage) / (maxDamage - minDamage);
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
            currentDamage + damageGrowthRate * Time.deltaTime,
            minDamage,
            maxDamage
        );

        currentTarget.EnemyTakeDamage(currentDamage * Time.deltaTime);
        Debug.Log($"造成伤害: {currentDamage * Time.deltaTime} → 剩余生命值: {currentTarget.currentHealth}"); // 伤害验证 <button class="citation-flag" data-index="5">
    }

    private void DisableBeam()
    {
        if (energyBeam) energyBeam.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}