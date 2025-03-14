using UnityEngine;

public class TowerBaseController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetBaseSprite(Sprite newSprite)
    {
        if(spriteRenderer != null)
        {
            spriteRenderer.sprite = newSprite;
        }
    }
}
