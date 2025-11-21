using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] float radius = 1.5f; 
    [SerializeField] int damageAmount = 10; 

    void Start() 
    {   
        Explode(); 
    }

    void OnDrawGizmos() 
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    void Explode()
    {   
        // Locate the center of the explosion particle effect. 
        Vector3 center = transform.position; 
        // Create a list of colliders which this effect intersects with.
        Collider[] hits = Physics.OverlapSphere(center, radius);
        // Iterate through the list and determine if the players collider is among them. 
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {   
                // If so, damage the player using the public method from PlayerHealth.cs
                PlayerHealth playerHealth = hit.GetComponentInParent<PlayerHealth>();
                playerHealth.TakeDamage(damageAmount);
                break;  
            }
        }
    }
}
