using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    public BoardManager boardManager;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private bool isSwiping = false; // 标记是否正在滑动

    void Update()
    {
        DetectKeyboardInput();
        DetectTouchInput();
    }


    private void DetectKeyboardInput()
    {
        Vector2Int direction = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) direction = new Vector2Int(-1, 0);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) direction = new Vector2Int(1, 0);
        else if (Input.GetKeyDown(KeyCode.UpArrow)) direction = new Vector2Int(0, 1);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) direction = new Vector2Int(0, -1);

        if (direction != Vector2Int.zero)
        {
            boardManager.MoveTowers(direction);
            Debug.Log($"Moved: {direction}");
        }
    }


    private void DetectTouchInput()
    {
        if (Input.touchCount > 0) // 确保屏幕上有触摸
        {
            Touch touch = Input.GetTouch(0); // 获取第一根手指触摸

            switch (touch.phase)
            {
                case TouchPhase.Began: // 触摸开始
                    startTouchPosition = touch.position;
                    isSwiping = true;
                    break;

                case TouchPhase.Ended: // 触摸结束
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

        if (swipeDelta.magnitude < 50) return; // 滑动距离太短，不处理

        Vector2Int direction = Vector2Int.zero;

        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
        {
            // 水平方向滑动
            direction = (swipeDelta.x > 0) ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
        }
        else
        {
            // 垂直方向滑动
            direction = (swipeDelta.y > 0) ? new Vector2Int(0, 1) : new Vector2Int(0, -1);
        }

        if (direction != Vector2Int.zero)
        {
            boardManager.MoveTowers(direction);
            Debug.Log($"Swiped: {direction}");
        }
    }
}
