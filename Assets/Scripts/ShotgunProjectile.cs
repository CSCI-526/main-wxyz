using UnityEngine;

public class ShotgunProjectile : MonoBehaviour
{
    public Enemy target;
    public float speed;
    public float damage;
    private Vector2 moveDirection;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetDirection(Vector2 targetPosition, float angleOffset)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        direction = Quaternion.Euler(0, 0, angleOffset) * direction; // Rotate direction
        rb.linearVelocity = direction * speed;
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
