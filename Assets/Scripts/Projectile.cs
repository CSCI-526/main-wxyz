using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Enemy target;
    public float speed;
    public float damage;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void InitializeProjectile()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 direction = (target.transform.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); 
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.EnemyTakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
