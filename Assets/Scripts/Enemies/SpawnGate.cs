using System.Collections;
using UnityEngine;

public class SpawnGate : MonoBehaviour
{   
    // Spawns a specific enemy at the given point, with a delay.
    // Can be destroyed if given collider and EnemyHealth script. 
    [SerializeField] GameObject enemy;
    PlayerHealth player;  
    [SerializeField] Transform spawnPoint; 

    // Careful with this, if it's too small you will be spamming prefabs.
    [SerializeField] float spawnDelay = 5f; 

    void Start() 
    {   
        // Call the coroutine once, upon the gates initialization. 
        player = GameObject.FindFirstObjectByType<PlayerHealth>(); 
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy() 
    {   
        // Create a co-routine which will be called every :spawnDelay: time interval
        while (player) 
        {   
            // Spawn an enemy at the given point.
            Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
            // Pause coroutine for the delay.  
            yield return new WaitForSeconds(spawnDelay);
        }
        // Now detonate the object upon player death. 
        this.gameObject.GetComponent<EnemyHealth>().SelfDestruct(); 
    }

}
