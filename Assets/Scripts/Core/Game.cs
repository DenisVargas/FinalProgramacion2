using UnityEngine;
using UnityEngine.SceneManagement;


public enum sceneIndex : uint
{
    MainMenu = 0,
    Credits =5,
    Introduction,
    Lvl1= 1,
    Lvl2 = 2,
    Lvl3 = 3,
    Victory = 4,
    Defeat= 6
}

public static class Game
{
    public static void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
    public static void LoadScene(sceneIndex sceneIndex)
    {
        SceneManager.LoadScene((int)sceneIndex);
    }
    public static void CloseGame()
    {
        Application.Quit();
    }
}
