using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class DifficultyMeter : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] AudioSource audioSource;
    [SerializeField] SpatialGrid spatialGrid;

   public float desiredVolume = 0f;
   public float volumeChange = 0.15f / 60;

   public float radius = 5;
   public float minVolume = 0.15f;
   public float maxVolume = 0.3f;
   public float maxDifficulty = 50f;

	void Awake()
	{
        desiredVolume = minVolume;
        audioSource.volume = desiredVolume;
	}

    // Update is called once per frame
    void Update()
    {
        desiredVolume = Mathf.Lerp(minVolume, maxVolume, Mathf.Clamp01(GetDifficulty() / maxDifficulty));

        if (audioSource.volume < desiredVolume)
            audioSource.volume += Mathf.Min(volumeChange, desiredVolume - audioSource.volume);
        else if (audioSource.volume > desiredVolume)
            audioSource.volume -= Mathf.Min(volumeChange, audioSource.volume - desiredVolume);
    }
   // IA 2  Punto 3 (Aggregate y LINKU) y Punto 2 Spatial Grid
    float GetDifficulty()
    {
        return spatialGrid.Query(player.position + new Vector3(-radius, 0, -radius),
                        player.position + new Vector3(radius, 0, radius),
                        x => Vector3.Distance(x, player.position) <= radius)
            .Select(entity => entity.GameObject.GetComponent<Enemy>())
            .Aggregate(0, (difficulty, next) => difficulty + next.GetCombatStats().Damage, difficulty => difficulty);
    }
}
