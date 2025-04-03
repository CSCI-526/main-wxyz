using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{
    public GameObject levelSelectPanel;

    public void StartGame()
    {
        levelSelectPanel.SetActive(true); // 显示选择面板
    }

        public void LoadTutorial()
    {
        SceneManager.LoadScene(3);
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene(1);
    }



    public void QuitGame()
    {
        Debug.Log("退出游戏...");
        Application.Quit();//退出游戏
    }
}
