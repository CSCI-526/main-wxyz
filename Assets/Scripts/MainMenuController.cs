using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1);//进入游戏场景
    }


    public void QuitGame()
    {
        Debug.Log("退出游戏...");
        Application.Quit();//退出游戏
    }
}
