using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
        [SerializeField] int startingHealth = 5;
        // The virtual camera is used for display after game over. 
        [SerializeField] CinemachineVirtualCamera virtualCamera; 
        [SerializeField] Transform weaponCamera; 
        [SerializeField] GameObject overlayUI; 
    int currentHealth;
    const int virtualCameraPriority = 20; 

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
            //Unparent the weapon cam to prevent errors after player deletion. 
            weaponCamera.parent = null; 
            // Increase priority to instigate a switch of cameras. 
            virtualCamera.Priority = virtualCameraPriority;
            // Deactivate UI (because it's game over)
            overlayUI.SetActive(false); 
            // Destroy the player. 
            Destroy(gameObject); 
        }
    }
}
