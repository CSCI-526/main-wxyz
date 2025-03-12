using UnityEngine;
using UnityEngine.UI;

public class MainButton : MonoBehaviour
{
    public GameManager gameManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed in MainButton script");
        }
    }


    public void OnButtonClicked()
    {      
        Debug.Log("Button clicked");
        if (gameManager != null)
        {
            bool spawned = gameManager.SpawnRandomTower();
            Debug.Log("Spawn result: " + spawned);
            Debug.Log("Button clicked");  
            if (spawned)
            {
                gameManager.DeductCost();
            }
        }
        else
        {
            Debug.LogError("GameManager reference is missing on SpawnTowerButton.");
        }
    }

    // TBD Add the changing appearance of the button when the player has enough gold to spawn a tower and disable the click if not enough gold

}
