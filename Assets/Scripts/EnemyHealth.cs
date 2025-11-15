using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    const int startingHealth = 50;
    int currentHealth;

    void Awake()
    {   
        // Initialize health. 
        currentHealth = startingHealth; 
    }

    void CheckHealth()
    {   
        // Simply check if health is less than zero and destroy if true. 
        if(currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    public void TakeDamage(int damageAmount)
    {
        // Public to be called by weapons script (or others)
        currentHealth -= damageAmount;
        CheckHealth();
        // Could also add knockback here
    }
}
