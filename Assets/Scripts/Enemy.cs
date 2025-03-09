using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public EnemyData enemyData;
    public UIManager uiManager;
    public Transform[] waypoints;
    public int index;
    private float moveSpeed = 0.5f;
    private float currentHealth = 100f;
    private int currentIndex = 0;
    private int coin = 5;

    private SpriteRenderer spriteRenderer;
    // private float burnDuration = 0f;
    // private float burnDamage = 0f;
    // private float slowRatio = 1f;
    private Coroutine slowEffectCoroutine;
    private Coroutine burnEffectCoroutine;
    private float originalSpeed;


    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        originalSpeed = moveSpeed;
        if (uiManager == null)
        {
            uiManager = Object.FindFirstObjectByType<UIManager>();
        }
        // UIManager uiManager = Object.FindFirstObjectByType<UIManager>();
        if (enemyData != null)
        {
            // UpdateAppearance();
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
        EnemyBehavior();
    }

    /*** enemy wayfinding ***/
    private void EnemyBehavior()
    {
        if(currentIndex < waypoints.Length){
            Transform targetPoint = waypoints[currentIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
            if(Vector3.Distance(transform.position, targetPoint.position) < 0.01f){
                currentIndex++;
                // enemy function test
                // EnemyTakeDamage(40);
                // EnemyBurnEffect(20f, 2f);
                // EnemySlowEffect(0.5f, 2f);
            }
        }
        else if (currentIndex == waypoints.Length){
            ReachDest();
        }
    }

    /*** enemy wayfinding end ***/



    /*** enemy health management:
    1. take damage ( public void EnemyTakeDamage(float damage) )
    2. set enemy HP ( public void SetMaxHealth(float maxHealth) )
    3. interact with GameManager.playerHealth ( private void ReachDest() )
    ***/
    public void EnemyTakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) Defeated();
    }

    public void SetMaxHealth(float maxHealth)
    {
        currentHealth = maxHealth;
    }

    private void Defeated()
    {
        Debug.Log(gameObject.name + index + " killed");
        GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddCoin(coin);
            Debug.Log("Player coin added:" + gameManager.playerGold);            
        }
        if (uiManager != null)  uiManager.UpdateGoldUI();
        Destroy(gameObject);
    }

    private void ReachDest()
    {
        DamagePlayerHealth(1);
        Debug.Log(gameObject.name + index + " make 1 damage");
        Destroy(gameObject);
        if (uiManager != null)  uiManager.UpdateHealthUI();
    }

    private void DamagePlayerHealth(int damage)
    {
        GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
        if (gameManager != null)    gameManager.ReduceHealth(damage);
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
            if (slowEffectCoroutine != null)    StopCoroutine(slowEffectCoroutine);
            slowEffectCoroutine = StartCoroutine(SlowEffectCoroutine(slowRatio, duration));
        }
    }

    private IEnumerator SlowEffectCoroutine(float slowRatio, float duration)
    {
        slowEffectRender();
        moveSpeed *= slowRatio;

        yield return new WaitForSeconds(duration);

        originalRender();
        moveSpeed = originalSpeed;
        slowEffectCoroutine = null;
    }

    public void EnemyBurnEffect(float damagePerSecond, float duration)
    {
        if (burnEffectCoroutine != null)    StopCoroutine(burnEffectCoroutine);
        burnEffectCoroutine = StartCoroutine(BurnCoroutine(damagePerSecond, duration));
    }

    private IEnumerator BurnCoroutine(float damagePerSecond, float duration)
    {
        if (slowEffectCoroutine != null)
        {
            StopCoroutine(slowEffectCoroutine);
            slowEffectCoroutine = null;
            moveSpeed = originalSpeed;
            originalRender();
        }

        burnEffectRender();
        yield return new WaitForSeconds(0.1f);

        while (duration > 0)
        {
            EnemyTakeDamage(damagePerSecond);
            yield return new WaitForSeconds(1f);
            duration -= 1f;
        }

        originalRender();
        burnEffectCoroutine = null;
    }

    private void originalRender()
    {
        if (spriteRenderer != null) spriteRenderer.color = Color.black;
    }

    public void slowEffectRender()
    {
        if (spriteRenderer != null) spriteRenderer.color = Color.blue;
    }

    private void burnEffectRender()
    {
        if (spriteRenderer != null) spriteRenderer.color = Color.red;
    }

    /*** enemy abnormal state end ***/

    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Here check if entering the final tile
    }
}