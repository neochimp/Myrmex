using UnityEngine;

public class EnemyHealth : MonoBehaviour
// Contains general behavior for all types of enemy health. 
{
    [SerializeField] int startingHealth = 10;
    [SerializeField] ParticleSystem explodeVFX;

    // GameManager used for adjusting UI
    GameManager gameManager;
    SpawnGate parentGate;
    int currentHealth;
    [SerializeField] AudioClip deathSound;

    void Awake()
    {
        // Initialize health to serialized starting health. 
        currentHealth = startingHealth;
    }

    void Start()
    {
        // This should eventually be refactored into a "dependancy injection" passing this from the spawn gate,
        // to each individual enemy. 
        // That's because too many gameManager objects is going to cause performance issues. Sooner rather than later. 
        gameManager = FindFirstObjectByType<GameManager>();
        // Adjust UI count by 1. 
        gameManager.AdjustEnemyCount(1);
        parentGate = GetComponentInParent<SpawnGate>();
    }

    public void TakeDamage(int damageAmount)
    {
        // Public function, to be called by weapons script (and other callers that could damage enemy)
        currentHealth -= damageAmount;

        // Simply check if health is less than zero and destroy if true. 
        if (currentHealth <= 0)
        {
            SelfDestruct();
        }
    }

    public void SelfDestruct()
    {
        // Destroy the object while insantiating an explosion effect. 
        // This is called in Robot.cs
        // The explosion effect itself will cause damage to the player, see Explosion.cs for details. 
        //Instantiate(explodeVFX, transform.position, Quaternion.identity);
        // Adjust the UI tracker. 
        parentGate.currentSpawns--;
        AudioSource.PlayClipAtPoint(deathSound, transform.position, 0.8f);
        gameManager.AdjustEnemyCount(-1);
        Destroy(gameObject);
        Debug.Log("Died");
    }
}
