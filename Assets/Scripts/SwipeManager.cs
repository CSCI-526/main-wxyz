using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    public BoardManager boardManager;
    
    public UIManager uiManager;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private bool isSwiping = false;

    private float swipeCooldown = 0.3f;

    void Update()
    {
        if (uiManager != null && uiManager.isPaused)
        {
            return;
        }

        if (DragArrow.isDraggingArrow || (Time.time - DragArrow.lastDragEndTime) < swipeCooldown)
            return;

        DetectKeyboardInput();
        DetectTouchInput();
    }

    private void DetectKeyboardInput()
    {
        Vector2Int direction = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            direction = new Vector2Int(-1, 0);
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            direction = new Vector2Int(1, 0);
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            direction = new Vector2Int(0, 1);
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            direction = new Vector2Int(0, -1);

        if (direction != Vector2Int.zero)
        {
            boardManager.MoveTowers(direction);
            Debug.Log($"Moved: {direction}");
        }
    } 


    private void DetectTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    isSwiping = true;
                    break;

                case TouchPhase.Ended:
                    if (isSwiping)
                    {
                        endTouchPosition = touch.position;
                        ProcessSwipe();
                        isSwiping = false;
                    }
                    break;
            }
        }
    }

    private void ProcessSwipe()
    {
        Vector2 swipeDelta = endTouchPosition - startTouchPosition;
        if (swipeDelta.magnitude < 50) return;

        Vector2Int direction = Vector2Int.zero;
        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
        {
            direction = (swipeDelta.x > 0) ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
        }
        else
        {
            direction = (swipeDelta.y > 0) ? new Vector2Int(0, 1) : new Vector2Int(0, -1);
        }

        if (direction != Vector2Int.zero)
        {
            boardManager.MoveTowers(direction);
            Debug.Log($"Swiped: {direction}");
        }
    }
}
