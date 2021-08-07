using UnityEngine;

[CreateAssetMenu(fileName = "SaveGame", menuName = "CoreData/SaveGame", order = 3)]
public class CheckPointData : ScriptableObject
{
    public bool playerSavedGame = false;
    public sceneIndex lastScenePlayed = sceneIndex.Lvl1;
    public int healthAmmount;

    [Header("Rifle")]
    public int BulletsInMagazine;
    public int BulletsInBackPack;
    public bool hasInfiniteBullets;
}
