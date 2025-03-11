using UnityEngine;

public class TowerController : MonoBehaviour
{
    public int rankValue = 1;

    public string towerName;
    public Sprite towerSprite;
    public float attackRange;
    public float attackDamage;
    
    public GameObject towerBasePrefab;
    public GameObject[] basePrefabs;
   


    protected SpriteRenderer spriteRenderer;
    private TowerBaseController baseController;
    public Vector2Int gridPosition;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void Start()
    {
        UpdateAppearance();

        if (transform.Find("TowerBaseHolder") == null)
        {
            CreateTowerBase();
        }
        else
        {
            Transform baseHolder = transform.Find("TowerBaseHolder");
            if (baseHolder != null)
            {
                baseController = baseHolder.GetComponent<TowerBaseController>();
            }
        }
    }

    protected virtual void CreateTowerBase()
    {
        GameObject baseObj = null;
        if (basePrefabs != null && basePrefabs.Length > 0)
        {
            int rankIndex = Mathf.Clamp(rankValue - 1, 0, basePrefabs.Length - 1);
            baseObj = Instantiate(basePrefabs[rankIndex], transform);
        }
        else if (towerBasePrefab != null)
        {
            baseObj = Instantiate(towerBasePrefab, transform);
        }

        if (baseObj != null)
        {
            baseObj.name = "TowerBaseHolder";
            baseObj.transform.localPosition = Vector3.zero;
            baseController = baseObj.GetComponent<TowerBaseController>();
        }
    }

    public virtual void UpdateAppearance()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = towerSprite;
        }

        if (baseController != null && basePrefabs != null && basePrefabs.Length > 0)
        {
            int rankIndex = Mathf.Clamp(rankValue - 1, 0, basePrefabs.Length - 1);
            GameObject basePrefab = basePrefabs[rankIndex];
            Sprite baseSprite = basePrefab.GetComponent<SpriteRenderer>().sprite;
            if (baseSprite != null)
            {
                baseController.SetBaseSprite(baseSprite);
            }
        }
    }

    public virtual void UpgradeTower()
    {
        if (rankValue < 4)
        {
            rankValue++;
            attackRange *= 1.2f;
            attackDamage *= 1.2f;
            ReplaceTowerBase();
        }
        UpdateAppearance();
    }

    protected virtual void ReplaceTowerBase()
    {
        Transform baseHolder = transform.Find("TowerBaseHolder");
        if (baseHolder != null)
        {
            Destroy(baseHolder.gameObject);
        }
        CreateTowerBase();
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
