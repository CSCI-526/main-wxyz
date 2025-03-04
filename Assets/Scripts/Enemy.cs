using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData enemyData;
    private float currentHealth;

    private SpriteRenderer spriteRenderer;


    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {   
        if(enemyData != null)
        {
            //UpdateAppearance();
        }
        currentHealth = enemyData.maxHealth;
        
    }
    public void UpdateAppearance()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = enemyData.enemySprite;
        }

    }

    void Update()
    {
        // Enemy behavior logic using enemyData.moveSpeed etc
        EnemyBehavior();
    }

    void EnemyBehavior()
    {
        // Enemy moving logic
    }
    void EnemyTakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Here check if entering the final tile
    }


}
