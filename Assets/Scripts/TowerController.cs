using UnityEngine;

public class TowerController : MonoBehaviour
{
    public TowerData towerData;           
    public GameObject towerBasePrefab;    
    public GameObject[] basePrefabs;
    
    private SpriteRenderer spriteRenderer;
    private TowerBaseController baseController;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if(towerData != null)
        {
            UpdateAppearance();
        }

        if(transform.Find("TowerBaseHolder") == null && towerBasePrefab != null)
        {
            GameObject baseObj = Instantiate(towerBasePrefab, transform);
            baseObj.name = "TowerBaseHolder";
            baseObj.transform.localPosition = new Vector3(0, -0.5f, 0); 

            baseController = baseObj.GetComponent<TowerBaseController>();
        }
        else
        {
            Transform baseHolder = transform.Find("TowerBaseHolder");
            if(baseHolder != null)
            {
                baseController = baseHolder.GetComponent<TowerBaseController>();
            }
        }
    }

    public void UpdateAppearance()
    {
        // Update the tower's main sprite and color.
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = towerData.towerSprite;
            spriteRenderer.color = towerData.towerColor;
        }

        if (baseController != null && basePrefabs != null)
        {
            int rankIndex = Mathf.Clamp(towerData.rankValue - 1, 0, basePrefabs.Length - 1);
            GameObject basePrefab = basePrefabs[rankIndex];
            Sprite baseSprite = basePrefab.GetComponent<SpriteRenderer>().sprite; 

            if (baseSprite != null)
            {
                baseController.SetBaseSprite(baseSprite);
            }
        }
    }


    public void UpgradeTower(TowerData newData)
    {
        towerData = newData;
        UpdateAppearance();
    }
}
