using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using StarterAssets;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{   
    // Create a slideable range for the player health
    // Max is currently 10 because we use ten bars in the image.
    // This could be changed by adding more bars to the array.     
    [Range(1, 10)]
    [SerializeField] int startingHealth = 10;
    // The virtual camera is used for display after game over (pan out effect). 

    // It is easiest just to serialize the various cameras and the UI canvas. 
    [SerializeField] CinemachineVirtualCamera virtualCamera; 
    [SerializeField] Transform weaponCamera; 
    //[SerializeField] GameObject overlayUI; 
    // An array of shield bars, intented to decrease as the player takes damage. 
    [SerializeField] UnityEngine.UI.Image[] shieldBars; 
    [SerializeField] GameObject gameOverUI;  
    int currentHealth;

    // The highest priority camera will always be the POV. 
    // So adjusting priority switches cameras (that's unity behavior)
    const int virtualCameraPriority = 20; 

    void Awake()
    {   
        // Initialize health. 
        currentHealth = startingHealth; 
        AdjustShieldUI(); 
    }
    public void TakeDamage(int damageAmount)
    {
        // Public, intended to be called by weapons script (or any script which damages the player)
        currentHealth -= damageAmount;
        // Adjust UI bars to display changes. 
        AdjustShieldUI();
        
        // Check if health is less than zero and call a GameOver result if true. 
        if(currentHealth <= 0)
        {      
            PlayerGameOver(); 
        }
    }

    void PlayerGameOver()
    {
        // Unparent the weapon cam to prevent errors after player deletion. 
        // It would be childed to a destroyed object
        weaponCamera.parent = null; 
        // Increase priority to instigate a switch of cameras. 
        virtualCamera.Priority = virtualCameraPriority;
        // Deactivate Cursor/Lock cursor: (because it's game over)
        StarterAssetsInputs starterAssetsInputs = gameObject.GetComponent<StarterAssetsInputs>(); 
        starterAssetsInputs.SetCursorState(false); 
        // Display the game over screen 
        gameOverUI.SetActive(true);
        // Destroy the player. 
        Destroy(gameObject);
    }

    public void AdjustShieldUI()
    {   
        // Iterate through the array of UI bar images
        for (int i = 0; i < shieldBars.Length; i++)
        {
            if (i < currentHealth)
            {   
                shieldBars[i].gameObject.SetActive(true); 
            }
            else
            // Only display the amount which matches our currentHealth 
            {
                shieldBars[i].gameObject.SetActive(false);
            }
        }
    }
}
