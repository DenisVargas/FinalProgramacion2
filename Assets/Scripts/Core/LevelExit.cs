using UnityEngine;
using UnityEditor;

[AddComponentMenu("CoreGame/ExitLevel", 2) ,RequireComponent(typeof(Collider))]
public class LevelExit : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] LevelManager level;
    [SerializeField] sceneIndex sceneIndex;
    [SerializeField] bool saveAtExit = true;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        level = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            level.SaveGame();
            AssetDatabase.SaveAssets();

            if (level.CurrentScene == sceneIndex.Lvl1 && sceneIndex == sceneIndex.Lvl2)
                GameProgressTracker.NotifyCompletedLevel(level.CurrentScene);
            if (level.CurrentScene == sceneIndex.Lvl2 && sceneIndex == sceneIndex.Lvl3)
                GameProgressTracker.NotifyCompletedLevel(sceneIndex.Lvl2);
            if (level.CurrentScene == sceneIndex.Lvl3 && sceneIndex == sceneIndex.Victory)
                GameProgressTracker.NotifyCompletedLevel(sceneIndex.Lvl3);

            Game.LoadScene(sceneIndex);
        }
    }
}
