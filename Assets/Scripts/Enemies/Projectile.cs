using Unity.Mathematics;
using UnityEngine;

public class Projectile : MonoBehaviour
{   
    // Functionality for the projectiles fired from turret.
    PlayerHealth playerHealth;
    int damage = 2; 
    float speed = 30f; 

    [SerializeField] Rigidbody rb;
    [SerializeField] ParticleSystem hitVFX; 

    void Start()
    {   
        // Set movement speed once, through velocity not forces, this gives most realistic effect.
        rb.velocity = transform.forward * speed;
    }

    void OnTriggerEnter(Collider other) 
    {   
        // Projectiles contain a trigger
        // Damage player if hit
        playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth)
        {
            playerHealth.TakeDamage(damage); 
        }

        // Then initialize small explosion and destroy self. 
        Instantiate(hitVFX, transform.position, Quaternion.identity);
        Destroy(gameObject); 
    }

    public void Init(int damage, float speed)
    {   
        // Initalize serialized values from the turret, keeping things tidy. 
        this.damage = damage;
        this.speed = speed;  
    }
}
