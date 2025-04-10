using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    public GameObject towerPrefab;
    // Reference to the new UI cooldown + panel script
    [Header("UI Overlay Manager (Cooldown + Panel Toggle)")]
    public UIToggleAndOverlay uiOverlayManager;

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
        // If paused or on cooldown, do nothing
        if ((uiManager != null && uiManager.isPaused) || 
            (uiOverlayManager != null && uiOverlayManager.IsOnCooldown))
        {
            return;
        }

        isDraggingArrow = true;

        // Light up possible tiles
        if (boardManager != null)
        {
            boardManager.LightAllTileHovers();
        }

        // Calculate pointer offset
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
        // If paused or on cooldown, do nothing
        if ((uiManager != null && uiManager.isPaused) || 
            (uiOverlayManager != null && uiOverlayManager.IsOnCooldown))
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

        // Convert arrow position to world coords
        Vector3 arrowScreenPos = RectTransformUtility.WorldToScreenPoint(
            eventData.pressEventCamera, 
            rectTransform.position
        );
        Vector3 arrowWorldPos = Camera.main.ScreenToWorldPoint(arrowScreenPos);
        arrowWorldPos.z = 0f;

        // Update tile highlight for the tile under the arrow
        if (boardManager != null)
        {
            TileController tileUnderArrow = boardManager.GetTileUnderPosition(arrowWorldPos);
            boardManager.UpdateTileHoverStatesAllButOne(tileUnderArrow);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // If paused or on cooldown, do nothing
        if ((uiManager != null && uiManager.isPaused) || 
            (uiOverlayManager != null && uiOverlayManager.IsOnCooldown))
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
                if (towerOnTile != null && towerOnTile.rankValue <= 4)
                {
                    bool upgraded = gameManager.UpgradeRandomTower(towerOnTile, towerPrefab);
                    if (upgraded)
                    {
                        gameManager.AddChangeColorCount();
                        // 1) Start the cooldown on the UI overlay
                        if (uiOverlayManager != null)
                        {
                            uiOverlayManager.StartCooldown();
                            uiOverlayManager.CloseColorPanel();  // Hide the panel after a successful upgrade
                        }
                    }
                }
            }
        }

        // Reset arrow to original UI location
        rectTransform.anchoredPosition = initialAnchoredPosition;
    }
}
