using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine.UI; 
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyHealth : MonoBehaviour
{   
    [SerializeField] int startingHealth = 10;
    [SerializeField] ParticleSystem explodeVFX; 
    int currentHealth;

    void Awake()
    {   
        // Initialize health. 
        currentHealth = startingHealth; 
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

    public void SelfDestruct()
    {   
        Instantiate(explodeVFX, transform.position, Quaternion.identity);  
        Destroy(gameObject);
    }
}
