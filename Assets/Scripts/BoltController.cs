using System.Collections;
using UnityEngine;

public class BoltController : MonoBehaviour
{
    private Transform target;
    private int damage;
    public float speed = 30f;
    public bool usePrediction = true;//bullet prediction
    public void Initialize(Transform _target, int _damage)
    {
        target = _target;
        damage = _damage;
        StartCoroutine(HitTarget());
    }
    IEnumerator HitTarget()
    {
        while (target != null)
        {
            Vector3 targetPos = usePrediction ? PredictPosition() : target.position;
            Vector3 direction = (targetPos - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            if (Vector3.Distance(transform.position, targetPos) < 0.5f)
                break;
            yield return null;
        }
        if (target != null)
        {
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.EnemyTakeDamage(damage);   
            }
        }

        Destroy(gameObject);
    }

    Vector3 PredictPosition()
    {
        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
        if (rb == null)
            return target.position;
        float distance = Vector3.Distance(transform.position, target.position);
        float timeToReach = distance / speed;
        return target.position + (Vector3)(rb.linearVelocity * timeToReach);
    }
}
