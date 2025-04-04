using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class DragArrow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static bool isDraggingArrow = false;
    public static float lastDragEndTime = 0f;

    private RectTransform rectTransform;
    private Vector2 initialAnchoredPosition;
    private Vector2 pointerOffset;

    public BoardManager boardManager;
    public GameManager gameManager;
    public UIManager uiManager;

    public Image parentCooldownImage;

    // Cooldown length in seconds.
    public float cooldownDuration = 60f;
    private bool onCooldown = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialAnchoredPosition = rectTransform.anchoredPosition;

        if (boardManager == null)
        {
            boardManager = FindObjectOfType<BoardManager>();
        }
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        if ((uiManager != null && uiManager.isPaused) || onCooldown)
        {
            return;
        }

        isDraggingArrow = true;


        if (boardManager != null)
        {
            boardManager.LightAllTileHovers();
        }

        // Calculate pointer offset.
        RectTransform parentRectTransform = rectTransform.parent as RectTransform;
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPointerPosition))
        {
            pointerOffset = rectTransform.anchoredPosition - localPointerPosition;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if ((uiManager != null && uiManager.isPaused) || onCooldown)
        {
            return;
        }

        RectTransform parentRectTransform = rectTransform.parent as RectTransform;
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPointerPosition))
        {
            rectTransform.anchoredPosition = localPointerPosition + pointerOffset;
        }

        Vector3 arrowScreenPos = RectTransformUtility.WorldToScreenPoint(
            eventData.pressEventCamera, 
            rectTransform.position
        );
        Vector3 arrowWorldPos = Camera.main.ScreenToWorldPoint(arrowScreenPos);
        arrowWorldPos.z = 0f;

        if (boardManager != null)
        {
            TileController tileUnderArrow = boardManager.GetTileUnderPosition(arrowWorldPos);
            boardManager.UpdateTileHoverStatesAllButOne(tileUnderArrow);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        if ((uiManager != null && uiManager.isPaused) || onCooldown)
        {
            return;
        }

        isDraggingArrow = false;
        lastDragEndTime = Time.time;

        if (boardManager != null)
        {
            boardManager.DisableAllTileHovers();

            Vector3 arrowScreenPos = RectTransformUtility.WorldToScreenPoint(
                eventData.pressEventCamera, 
                rectTransform.position
            );
            Vector3 arrowWorldPos = Camera.main.ScreenToWorldPoint(arrowScreenPos);
            arrowWorldPos.z = 0f;

            TileController tileAtDrop = boardManager.GetTileUnderPosition(arrowWorldPos);
            if (tileAtDrop != null)
            {
                TowerController towerOnTile = tileAtDrop.towerOnTile;
                if (towerOnTile != null && towerOnTile.rankValue < 4)
                {
                    bool upgraded = gameManager.UpgradeRandomTower(towerOnTile);
                    if (upgraded)
                    {
                        StartCoroutine(CooldownRoutine());
                    }
                }
            }
        }

        // Reset arrow to original UI location
        rectTransform.anchoredPosition = initialAnchoredPosition;
    }

    /// <summary>
    /// Animates the parent's cooldown image from full -> 0 over the cooldown duration,
    /// then re-enables dragging.
    /// </summary>
    private IEnumerator CooldownRoutine()
    {
        onCooldown = true;

        if (parentCooldownImage != null)
        {
            // Fill at 100% initially to represent the start of cooldown
            parentCooldownImage.fillAmount = 1f;
        }

        float remainingTime = cooldownDuration;
        while (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            // Update fill to reflect how much time remains
            if (parentCooldownImage != null)
            {
                parentCooldownImage.fillAmount = remainingTime / cooldownDuration;
            }

            yield return null;
        }

        // Cooldown finished
        if (parentCooldownImage != null)
        {
            // Optionally set fill to 0 to indicate off cooldown
            parentCooldownImage.fillAmount = 0f;
        }

        onCooldown = false;
    }
}
