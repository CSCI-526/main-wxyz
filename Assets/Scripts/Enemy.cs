using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
    protected float originalWidthHealthBar = 0.35f;

    protected Coroutine frozenEffectCoroutine;
    protected Coroutine burnEffectCoroutine;
    // protected bool frozenState = false;
    // protected bool burnState = false;
    
    protected int wayfindingIndex = 0;
    protected int coin = 5;

    protected SpriteRenderer spriteRenderer;

    // Two manager variables.
    private GameManager gameManager;
    private TutGameManager tutGameManager;

    public Image healthBar;

    public Sprite[] normalSprites;
    public Sprite[] frozenSprites;
    public Sprite[] burnSprites;
    private float animTimer = 0f;
    private int currentFrame = 0;
    private float originalClipRatio = 0.1f;
    private float clipRatio = 0.1f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        originalColor = spriteRenderer.color;
        currentHealth = originalHealth;
        currentSpeed = originalSpeed;
        originalWidthHealthBar = healthBar.GetComponent<RectTransform>().sizeDelta.x;
        originalClipRatio = 1f / normalSprites.Length;
        clipRatio = originalClipRatio;

        Transform startPoint = transform;
        for (int i = 0; i < waypoints.Length; i++)
        {
            distance += Vector2.Distance(startPoint.position, waypoints[i].position);
            startPoint = waypoints[i];
        }
        
        if (uiManager == null)
        {
            uiManager = Object.FindFirstObjectByType<UIManager>();
        }
        
        gameManager = Object.FindFirstObjectByType<GameManager>();
        tutGameManager = Object.FindFirstObjectByType<TutGameManager>();
        
        if (enemyData != null)
        {
            UpdateAppearance();
        }
    }

    void Update()
    {
        EnemyBehavior();
    }

    /*** Enemy initialization ***/
    public void InitiateEnemy(Transform[] waypointList, float health, float speed, int c)
    {
        waypoints = waypointList;
        originalHealth = health;
        originalSpeed = speed;
        coin = c;
    }

    public void UpdateAppearance()
    {
        if (spriteRenderer != null && enemyData != null)
        {
            spriteRenderer.sprite = enemyData.enemySprite;
        }
    }
    /*** End enemy initialization ***/

    /*** Enemy wayfinding ***/
    protected void EnemyBehavior()
    {
        // enemy health bar
        RectTransform rt = healthBar.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(originalWidthHealthBar * (currentHealth / originalHealth), rt.sizeDelta.y);
        
        // enemy abnormal state animation switch
        UpdateAnimation();

        // enemy way finding
        if (wayfindingIndex < waypoints.Length)
        {
            Transform targetPoint = waypoints[wayfindingIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, currentSpeed * Time.deltaTime);
            distance -= currentSpeed * Time.deltaTime;
            // Debug.Log($"{name} remains {distance} distance to destination");
            if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
            {
                wayfindingIndex++;
                if (wayfindingIndex == 1)   spriteRenderer.flipX = true;  // 水平翻转
                if (wayfindingIndex == 3)   spriteRenderer.flipX = false; 
            }
        }
        else if (wayfindingIndex == waypoints.Length)
        {
            ReachDest();
        }
    }
    /*** End enemy wayfinding ***/

    /*** Enemy health management ***/
    public void EnemyTakeDamage(float damage, string type = "gun")
    {
        // Apply tower damage based on type.
        if (gameManager != null)
        {
            switch (type)
            {
                case "burning":
                    gameManager.AddDamageFromBurningTower(damage);
                    break;
                case "frozen":
                    gameManager.AddDamageFromSlowTower(damage);
                    break;
                case "energy":
                    gameManager.AddDamageFromEnergyTower(damage);
                    break;
                default:
                    gameManager.AddDamageFromCanonTower(damage);
                    break;
            }
        }

        currentHealth -= damage;
        Debug.Log($"{name} took {damage} damage ({originalHealth} / {currentHealth}) from {type}");
        if (currentHealth <= 0)
            Defeated();
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
            // Reward coins and add score.
            if (gameManager != null)
            {
                gameManager.AddCoin(coin);
                gameManager.AddScore(originalHealth);
                Debug.Log("Player coin added:" + gameManager.playerGold);
            }
            else if (tutGameManager != null)
            {
                tutGameManager.AddCoin(coin);
                tutGameManager.AddScore(originalHealth);
                Debug.Log("Player coin added:" + tutGameManager.playerGold);
            }
            
            if (uiManager != null)
                uiManager.UpdateGoldUI();
            Destroy(gameObject);
        }
    }

    protected void ReachDest()
    {
        DamagePlayerHealth(1);
        Debug.Log(gameObject.name + index + " make 1 damage");
        Destroy(gameObject);
        if (uiManager != null)
            uiManager.UpdateHealthUI();
    }

    protected void DamagePlayerHealth(int damage)
    {
        if (gameManager != null)
            gameManager.ReduceHealth(damage);
        else if (tutGameManager != null)
            tutGameManager.ReduceHealth(damage);
    }
    /*** End enemy health management ***/

    /*** Enemy abnormal state effects ***/
    public void EnemySlowEffect(float frozenRatio, float duration)
    {
        if (burnEffectCoroutine == null)
        {
            if (frozenEffectCoroutine != null) StopCoroutine(frozenEffectCoroutine);
            frozenEffectCoroutine = StartCoroutine(SlowEffectCoroutine(frozenRatio, duration));
        }
    }

    protected IEnumerator SlowEffectCoroutine(float frozenRatio, float duration)
    {
        // frozenEffectRender();
        currentSpeed *= frozenRatio;
        yield return new WaitForSeconds(0.1f);

        while (duration > 0)
        {
            // frozenEffectRender();
            EnemyTakeDamage(20f, "frozen");
            yield return new WaitForSeconds(1f);
            duration -= 1f;
        }

        // originalRender();
        currentSpeed = originalSpeed;
        frozenEffectCoroutine = null;
    }

    public void EnemyBurnEffect(float damagePerSecond, float duration)
    {
        if (burnEffectCoroutine != null)
            StopCoroutine(burnEffectCoroutine);
        burnEffectCoroutine = StartCoroutine(BurnCoroutine(damagePerSecond, duration));
    }

    protected IEnumerator BurnCoroutine(float damagePerSecond, float duration)
    {
        if (frozenEffectCoroutine != null)
        {
            StopCoroutine(frozenEffectCoroutine);
            frozenEffectCoroutine = null;
            currentSpeed = originalSpeed;
            // originalRender();
        }

        burnEffectRender();
        yield return new WaitForSeconds(0.1f);

        while (duration > 0)
        {
            EnemyTakeDamage(damagePerSecond, "burning");
            // int freq = 2;
            // float interval = 1f / freq;
            // for (int i = 0; i < freq; i++)
            // {
            //     // burnEffectRender();
            //     yield return new WaitForSeconds(interval / 2);
            //     // originalRender();
            //     yield return new WaitForSeconds(interval / 2);
            //     duration -= interval;
            // }
            // burnEffectRender();
            yield return new WaitForSeconds(1f);
            duration -= 1f;
            // originalRender();
            // yield return new WaitForSeconds(0.5f);
            // duration -= 0.5f;
        }

        // originalRender();
        burnEffectCoroutine = null;
    }

    protected void originalRender()
    {
        // if (spriteRenderer != null)
        //     spriteRenderer.color = originalColor;
    }

    protected void frozenEffectRender()
    {
        // if (spriteRenderer != null)
        //     spriteRenderer.color = Color.blue;
    }

    protected void burnEffectRender()
    {
        // if (spriteRenderer != null)
        //     spriteRenderer.color = Color.red;
    }
    /*** End enemy abnormal state effects ***/

    void UpdateAnimation()
    {
        animTimer += Time.deltaTime;
        if (animTimer >= clipRatio)
        {
            animTimer = 0f;
            currentFrame = (currentFrame + 1) % normalSprites.Length;
            if (burnEffectCoroutine != null)
            {
                spriteRenderer.sprite = burnSprites[currentFrame];
                clipRatio = originalClipRatio;
            }
            else if (frozenEffectCoroutine != null)
            {
                spriteRenderer.sprite = frozenSprites[currentFrame];
                clipRatio = originalClipRatio * (originalSpeed / currentSpeed);
            }
            else
            {
                spriteRenderer.sprite = normalSprites[currentFrame];
                clipRatio = originalClipRatio;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Here you can check if the enemy has entered the final tile.
    }
}
