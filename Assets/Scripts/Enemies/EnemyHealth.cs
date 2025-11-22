using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine.UI; 
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyHealth : MonoBehaviour
// Contains general behavior for all types of enemy health. 
{   
    [SerializeField] int startingHealth = 10;
    [SerializeField] ParticleSystem explodeVFX; 
    int currentHealth;

    void Awake()
    {   
        // Initialize health to serialized starting health. 
        currentHealth = startingHealth; 
    }
    public void TakeDamage(int damageAmount)
    {
        // Public function, to be called by weapons script (and other callers that could damage enemy)
        currentHealth -= damageAmount;
        
        // Simply check if health is less than zero and destroy if true. 
        if(currentHealth <= 0)
        {   
            SelfDestruct(); 
        }
    }

    public void SelfDestruct()
    {   
        // Destroy the object while insantiating an explosion effect. 
        // This is called in Robot.cs
        Instantiate(explodeVFX, transform.position, Quaternion.identity);  
        Destroy(gameObject);
    }
}
