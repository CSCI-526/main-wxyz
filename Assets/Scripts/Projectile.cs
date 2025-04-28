using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Enemy target;
    public float speed;
    public float damage;
    private Rigidbody2D rb;
    private float lifetime = 0.4f; // 子弹最大存在时间
    private float lifeTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lifeTimer = lifetime; // 计时器初始化
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
            Destroy(gameObject); // 目标消失时销毁子弹
            return;
        }

        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0)
        {
            Destroy(gameObject); // 超时销毁子弹
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
            Destroy(gameObject); // 命中敌人后销毁子弹
        }
    }
}