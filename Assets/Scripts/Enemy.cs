using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData enemyData;
    public Transform[] waypoints;
    private float moveSpeed = 3f;
    private float currentHealth = 100f;
    private int currentIndex = 0;

    private SpriteRenderer spriteRenderer;


    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (enemyData != null)
        {
            //UpdateAppearance();
        }
        // currentHealth = enemyData.maxHealth;
        
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
        if(currentIndex < waypoints.Length){
            Transform targetPoint = waypoints[currentIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
            if(Vector3.Distance(transform.position, targetPoint.position) < 0.01f){
                currentIndex++;
                // EnemyTakeDamage(40);
            }
        }
        else if (currentIndex == waypoints.Length){
            ReachDest();
        }
    }
    public void EnemyTakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetMaxHealth(float maxHealth)
    {
        currentHealth = maxHealth;
    }

    private void ReachDest()
    {
        DamagePlayerHealth(1);
        Debug.Log(gameObject.name + " make 1 damage");
        Destroy(gameObject);
    }

    private void DamagePlayerHealth(int damage)
    {
        GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.playerHealth -= damage;
            Debug.Log("Player health: " + gameManager.playerHealth);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Here check if entering the final tile
    }
}