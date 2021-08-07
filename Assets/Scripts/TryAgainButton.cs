using UnityEngine;

public class TryAgainButton : MonoBehaviour
{
    LevelManager level;

    private void Awake()
    {
        level = FindObjectOfType<LevelManager>();
    }

    public void LoadLastLevel()
    {
        var save = level.LoadGame();

        if (!save.playerSavedGame)
            Game.LoadScene(sceneIndex.Lvl1); //Morimos en el primer nivel asi que no se guardo nada.
        else
        {
            if (save.lastScenePlayed == sceneIndex.Lvl1)
                Game.LoadScene(sceneIndex.Lvl2);
            if (save.lastScenePlayed == sceneIndex.Lvl2 || 
                save.lastScenePlayed == sceneIndex.Lvl3)
                Game.LoadScene(sceneIndex.Lvl3);
        }
    }
}
