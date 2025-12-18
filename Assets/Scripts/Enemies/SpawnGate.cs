using System.Collections;
using UnityEngine;

public class SpawnGate : MonoBehaviour
{
    // Spawns a specific enemy at the given point, with a delay.
    // Can be destroyed if given collider and EnemyHealth script. 
    [SerializeField] GameObject enemy;
    PlayerHealth player;
    [SerializeField] Transform spawnPoint; //center of spawning
    [SerializeField] Vector3 spawnVarianceRadius = new Vector3(4f, 0, 4f); //if you want the spawning to be randomly disperesed
    // Careful with this, if it's too small you will be spamming prefabs.
    [SerializeField] float spawnDelay = 15f;
    [SerializeField] int maxSpawns = 5;
    public int currentSpawns = 0;

    void Start()
    {
        // Call the coroutine once, upon the gates initialization. 
        player = GameObject.FindFirstObjectByType<PlayerHealth>();
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        // Create a co-routine which will be called every :spawnDelay: time interval
        while (player && currentSpawns < maxSpawns)
        {
            // Spawn an enemy at the given point.
            Instantiate(enemy, randomSpawn(spawnPoint.position, spawnVarianceRadius), Quaternion.identity, transform);
            // Pause coroutine for the delay.  
            currentSpawns++;
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    Vector3 randomSpawn(Vector3 center, Vector3 varianceRadius)
    {
        return new Vector3(center.x + Random.Range(-varianceRadius.x, varianceRadius.x),
                           center.y + Random.Range(-varianceRadius.y, varianceRadius.y),
                           center.z + Random.Range(-varianceRadius.z, varianceRadius.z));
    }

    public void countEnemyDeath()
    {
        currentSpawns--;
    }

}
