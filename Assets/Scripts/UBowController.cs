using System.Collections;
using UnityEngine;

public class UBowTower : MonoBehaviour
{
    [Header("kernel Parameter")]
    public float attackRange = 18f;
    public float fireRate = 0.5f;
    public int damagePerShot = 25;
    public float rotationSpeed = 10f;

    [Header("Component")]
    public Transform firePoint;      
    public GameObject boltPrefab; //Bolt prefab
    public LayerMask enemyLayer;

    private Transform currentTarget;
    private float nextFireTime;

    void Start()
    {
        StartCoroutine(TargetingRoutine());
    }

    void Update()
    {
        HandleAiming();
        HandleShooting();
    }

    void HandleAiming()
    {
        if (currentTarget != null)
        {
            Vector3 dir = currentTarget.position - transform.position;
            dir.y = 0; // keep rotation horizontal
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
        }
    }

    void HandleShooting()
    {
        if (Time.time >= nextFireTime && currentTarget != null)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        GameObject proj = Instantiate(boltPrefab,
            firePoint.position,
            firePoint.rotation);

        BoltController pc = proj.GetComponent<BoltController>();
        pc.Initialize(currentTarget, damagePerShot);
    }

    IEnumerator TargetingRoutine()
    {
        while (true)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
            currentTarget = hits.Length > 0 ? GetClosestEnemy(hits) : null;
            yield return new WaitForSeconds(0.1f);
        }
    }

    Transform GetClosestEnemy(Collider[] hits)
    {
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = hit.transform;
            }
        }
        return closest;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}