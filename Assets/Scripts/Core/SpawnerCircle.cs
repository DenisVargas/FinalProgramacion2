using System.Collections;
using UnityEngine;

public enum EnemyType2
{
    Enemy1,
    Enemy2
}

public class SpawnerCircle : MonoBehaviour
{
    public EnemyType2 enemyToSpawn;
    public bool SpawnAtStart = false;
    public bool rotateTowardsPlayer = true;
    public float SpawnDelay = 10f;

    public GameObject P_Enemy1;
    public GameObject P_Enemy2;

    [SerializeField] float SpawnRange = 4f;
    [SerializeField] float MinSpawnRange = 0f;
    [SerializeField] int enemiesToSpawn = 1;
    [SerializeField] Color debug_spawnMaxRangeColor = Color.yellow;
    [SerializeField] Color debug_spawnMinRangeColor = Color.yellow;


    public float Frecuency = 10f;

    int enemiesSpawned = 0;

    private void Start()
    {
        if (SpawnAtStart)
            StartCoroutine(SpawnEnemies());
        else
            StartCoroutine(DelayedSpawn(SpawnDelay));
    }

    public void Spawn()
    {
        GameObject toSpawnPrefab = null;

        switch (enemyToSpawn)
        {
            case EnemyType2.Enemy1:
                toSpawnPrefab = P_Enemy1;
                break;
            case EnemyType2.Enemy2:
                toSpawnPrefab = P_Enemy2;
                break;
            default:
                break;
        }

        var instanciado = Instantiate(toSpawnPrefab, transform.position + GetrandomPositionInCircle(MinSpawnRange, SpawnRange), Quaternion.identity);

        Player player = FindObjectOfType<Player>();
        if (rotateTowardsPlayer && player != null)
        {
            Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
            dirToPlayer.y = 0;
            instanciado.transform.forward = dirToPlayer;
        }

        enemiesSpawned++;
    }
    private Vector3 GetrandomPositionInCircle(float minRadius, float maxRadius)
    {
        //Calculo posicion.
        float distanceFactor = Random.Range(0f, 1f);
        float factorToMinDistance = (maxRadius - minRadius) / maxRadius;
        float angle = Mathf.Deg2Rad * (Random.Range(0f, 360f));
        Vector2 circlePoint = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Vector2 dirToCircle = (circlePoint - Vector2.zero);
        Vector2 pointToMinFactor = Vector2.Lerp(Vector2.zero, circlePoint, factorToMinDistance);
        Vector2 randomPointInCircle = Vector2.Lerp(pointToMinFactor, circlePoint, distanceFactor);

        Vector3 dir = new Vector3(randomPointInCircle.x, 0, randomPointInCircle.y);
        dir *= maxRadius;
        return dir;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = debug_spawnMaxRangeColor;
        Gizmos.matrix = Matrix4x4.Scale(new Vector3(1, 0, 1));
        Gizmos.DrawWireSphere(transform.position, SpawnRange);

        Gizmos.color = debug_spawnMinRangeColor;
        Gizmos.matrix = Matrix4x4.Scale(new Vector3(1, 0, 1));
        Gizmos.DrawWireSphere(transform.position, MinSpawnRange);
    }

    IEnumerator DelayedSpawn(float time)
    {
        yield return new WaitForSeconds(time);
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (enemiesSpawned < enemiesToSpawn)
        {
            Spawn();
            yield return new WaitForSeconds(Frecuency);
        }
    }
}