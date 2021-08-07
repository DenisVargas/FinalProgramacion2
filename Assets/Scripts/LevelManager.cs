using UnityEngine;
using UnityEditor;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public sceneIndex CurrentScene;
    public string saveGamePath = "SaveGame";
    public bool isPlayableScene = true;

    Player player;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(this);

        if (isPlayableScene)
        {
            CheckPointData lastData = LoadGame();

            if (lastData.playerSavedGame)
            {
                player = FindObjectOfType<Player>();
                player.GetWeaponByType(WeaponType.AssaultRifle);

                if (lastData.lastScenePlayed == CurrentScene)
                {
                    player.Health = lastData.healthAmmount;

                    Weapon rifle =  player.GetWeaponByType(WeaponType.AssaultRifle);
                    rifle.LoadBulletInfo(lastData.BulletsInMagazine, lastData.BulletsInBackPack);
                }
            }
        }
    }

    public CheckPointData LoadGame()
    {
        return Resources.Load<CheckPointData>(saveGamePath);
    }

    public void SaveGame()
    {
        if (isPlayableScene)
        {
            CheckPointData lastData = LoadGame();
            player = FindObjectOfType<Player>();
            lastData.healthAmmount = player.Health;


            Weapon rifle = player.GetWeaponByType(WeaponType.AssaultRifle);
            lastData.BulletsInMagazine = rifle.Magazine;
            lastData.BulletsInBackPack = rifle.backPack;
            lastData.hasInfiniteBullets = rifle.InfiniteBullets;

            lastData.playerSavedGame = true;
        }
    }

    public void ClearGameData()
    {
        CheckPointData data = LoadGame();

        data.playerSavedGame = false;
        data.lastScenePlayed = sceneIndex.Lvl1;
        data.BulletsInMagazine = 0;
        data.BulletsInBackPack = 0;
        data.healthAmmount = 0;
        data.hasInfiniteBullets = false;
    }
}
