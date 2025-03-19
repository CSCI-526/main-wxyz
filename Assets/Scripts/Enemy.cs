using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public EnemyData enemyData;
    public UIManager uiManager;
    public Transform[] waypoints;
    public int index;
    public float distance = 0f;
    public bool IsAlive { get; protected set; } = true;

    protected float currentSpeed = 1f;
    protected float originalSpeed = 1f;
    protected float currentHealth = 100f;
    protected float originalHealth = 100f;
    protected Color originalColor;

    protected Coroutine slowEffectCoroutine;
    protected Coroutine burnEffectCoroutine;
    
    protected int wayfindingIndex = 0;
    protected int coin = 5;

    protected SpriteRenderer spriteRenderer;
    
    
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        originalColor = spriteRenderer.color;
        currentHealth = originalHealth;
        currentSpeed = originalSpeed;
        Transform startPoint = transform;
        for (int i=0; i < waypoints.Length; i ++)
        {
            distance += Vector2.Distance(startPoint.position, waypoints[i].position);
            startPoint = waypoints[i];
        }
        if (uiManager == null)
        {
            uiManager = Object.FindFirstObjectByType<UIManager>();
        }
        if (enemyData != null)
        {
            // UpdateAppearance();
        }
        // currentHealth = enemyData.maxHealth;
    }

    public void InitiateEnemy(Transform[] waypointList, float health, float speed, int c)
    {
        waypoints = waypointList;
        originalHealth = health;
        originalSpeed = speed;
        coin = c;
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
        EnemyBehavior();
    }

    /*** enemy wayfinding ***/
    protected void EnemyBehavior()
    {
        if (wayfindingIndex < waypoints.Length)
        {
            Transform targetPoint = waypoints[wayfindingIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, currentSpeed * Time.deltaTime);
            distance -= currentSpeed * Time.deltaTime;
            Debug.Log($"{name} remains {distance} distance to destination");
            if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
            {
                wayfindingIndex ++;
                // enemy function test
                // EnemyTakeDamage(40);
                // EnemyBurnEffect(20f, 2f);
                // EnemySlowEffect(0.5f, 2f);
            }
        }
        else if (wayfindingIndex == waypoints.Length)
        {
            ReachDest();
        }
    }

    /*** enemy wayfinding end ***/



    /*** enemy health management:
    1. take damage ( public void EnemyTakeDamage(float damage) )
    2. set enemy HP ( public void SetMaxHealth(float maxHealth) )
    3. interact with GameManager.playerHealth ( protected void ReachDest() )
    ***/
    public void EnemyTakeDamage(float damage, string type = "gun")
    {
        switch (type)
        {
            case "burning":
                GameManager.Instance.AddDamageFromBurningTower(damage);
                break;
            case "slow":
                GameManager.Instance.AddDamageFromSlowTower(damage);
                break;
            case "energy":
                GameManager.Instance.AddDamageFromEnergyTower(damage);
                break;
            default:
                GameManager.Instance.AddDamageFromTankTower(damage);
                break;
        }
        currentHealth -= damage;
        Debug.Log($"{name} took {damage} damage({originalHealth} / {currentHealth}) from {type}");
        if (currentHealth <= 0) Defeated();
    }

    public void SetMaxHealth(float maxHealth)
    {
        currentHealth = maxHealth;
        originalHealth = maxHealth;
    }

    public void SetcurrentSpeed(float speed)
    {
        currentSpeed = speed;
        originalSpeed = speed;
    }

    protected void Defeated()
    {
        if (IsAlive)
        {
            IsAlive = false;
            Debug.Log(gameObject.name + index + " killed, HP:" + originalHealth);
            GameManager.Instance.AddCoin(coin);
            GameManager.Instance.AddScore(originalHealth);
            Debug.Log("Player coin added:" + GameManager.Instance.playerGold);       
            if (uiManager != null)  uiManager.UpdateGoldUI();
            Destroy(gameObject);
        }
        // Debug.Log(gameObject.name + index + " killed, HP:" + originalHealth);
        // GameManager.Instance.AddCoin(coin);
        // GameManager.Instance.AddScore(originalHealth);
        // Debug.Log("Player coin added:" + GameManager.Instance.playerGold);       
        // if (uiManager != null)  uiManager.UpdateGoldUI();
        // Destroy(gameObject);
    }

    protected void ReachDest()
    {
        DamagePlayerHealth(1);
        Debug.Log(gameObject.name + index + " make 1 damage");
        Destroy(gameObject);
        if (uiManager != null) uiManager.UpdateHealthUI();
    }

    protected void DamagePlayerHealth(int damage)
    {
        GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
        if (gameManager != null) gameManager.ReduceHealth(damage);
    }

    /*** enemy health management end ***/



    /*** enemy abnormal state:
    1. slow effect ( public void EnemySlowEffect(float slowRatio, float duration) )
    2. burn effect ( public void EnemyBurnEffect(float damagePerSecond, float duration) )
    ***/
    public void EnemySlowEffect(float slowRatio, float duration)
    {
        if (burnEffectCoroutine == null)
        {
            if (slowEffectCoroutine != null) StopCoroutine(slowEffectCoroutine);
            slowEffectCoroutine = StartCoroutine(SlowEffectCoroutine(slowRatio, duration));
        }
    }

    protected IEnumerator SlowEffectCoroutine(float slowRatio, float duration)
    {
        slowEffectRender();
        currentSpeed *= slowRatio;
        yield return new WaitForSeconds(0.1f);

        while (duration > 0)
        {
            // EnemyTakeColdDamage(20f);
            EnemyTakeDamage(20f, "slow");
            yield return new WaitForSeconds(1f);
            duration -= 1f;
        }

        // yield return new WaitForSeconds(duration);

        originalRender();
        currentSpeed = originalSpeed;
        slowEffectCoroutine = null;
    }

    public void EnemyBurnEffect(float damagePerSecond, float duration)
    {
        if (burnEffectCoroutine != null) StopCoroutine(burnEffectCoroutine);
        burnEffectCoroutine = StartCoroutine(BurnCoroutine(damagePerSecond, duration));
    }

    protected IEnumerator BurnCoroutine(float damagePerSecond, float duration)
    {
        if (slowEffectCoroutine != null)
        {
            StopCoroutine(slowEffectCoroutine);
            slowEffectCoroutine = null;
            currentSpeed = originalSpeed;
            originalRender();
        }

        burnEffectRender();
        yield return new WaitForSeconds(0.1f);

        while (duration > 0)
        {
            // EnemyTakeBurnDamage(damagePerSecond);
            EnemyTakeDamage(damagePerSecond, "burning");
            yield return new WaitForSeconds(1f);
            duration -= 1f;
        }

        originalRender();
        burnEffectCoroutine = null;
    }

    protected void originalRender()
    {
        if (spriteRenderer != null) spriteRenderer.color = originalColor;
    }

    protected void slowEffectRender()
    {
        if (spriteRenderer != null) spriteRenderer.color = Color.blue;
    }

    protected void burnEffectRender()
    {
        if (spriteRenderer != null) spriteRenderer.color = Color.red;
    }

    /*** enemy abnormal state end ***/



    void OnTriggerEnter2D(Collider2D other)
    {
        // Here check if entering the final tile
    }
}