using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{   
    [SerializeField] ParticleSystem explodeVFX; 
    const int startingHealth = 50;
    int currentHealth;

    void Awake()
    {   
        // Initialize health. 
        currentHealth = startingHealth; 
    }

    public void SelfDestruct()
    {   
        Instantiate(explodeVFX, transform.position, Quaternion.identity);  
        Destroy(gameObject);
    }
    
    public void TakeDamage(int damageAmount)
    {
        // Public to be called by weapons script (or others)
        currentHealth -= damageAmount;
        
        // Simply check if health is less than zero and destroy if true. 
        if(currentHealth <= 0)
        {   
            SelfDestruct(); 
        }
    }
}
