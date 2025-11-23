using UnityEngine;

public class Explosion : MonoBehaviour
{   
    // The only data we are concerned with is the size of the explosion
    // And the damage it outputs. 
    [SerializeField] float radius = 1.5f; 
    [SerializeField] int damageAmount = 10; 

    void Start() 
    {   
        Explode(); 
    }

    // Uncomment this if you want to see the radius of explosions as red lines drawn in the scene view
    // It is useful for debugging
    //void OnDrawGizmos() 
    //{
    //    Gizmos.color = Color.red; 
     //   Gizmos.DrawWireSphere(transform.position, radius);
    //}

    void Explode()
    // Note, don't forget that the explosion particle effect is what causes damage.
    // NOT the object it's attached to.
    // This works well in code, but it's not the most intuitive. 
    {   
        // Locate the center of the explosion particle effect. 
        Vector3 center = transform.position; 
        // Create a list of colliders that this effect intersects with.
        Collider[] hits = Physics.OverlapSphere(center, radius);
        // Iterate through that list, and determine if the players collider is among them. 
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {   
                // If so, damage the player using the public method retrieved from PlayerHealth.cs
                PlayerHealth playerHealth = hit.GetComponentInParent<PlayerHealth>();
                playerHealth.TakeDamage(damageAmount);
                // Stop iterating through the list, we might save some memory here. 
                break;  
            }
        }
    }
}
