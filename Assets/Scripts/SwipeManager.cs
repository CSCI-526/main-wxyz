using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    public BoardManager boardManager;

    void Update()
    {   
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            boardManager.MoveTowers(new Vector2Int(-1, 0)); 
            Debug.Log("Left");
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            boardManager.MoveTowers(new Vector2Int(1, 0));  
            Debug.Log("Right");
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            boardManager.MoveTowers(new Vector2Int(0, 1)); 
            Debug.Log("Up");
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            boardManager.MoveTowers(new Vector2Int(0, -1)); 
            Debug.Log("Down");
        }
    }
}
