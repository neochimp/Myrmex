using UnityEngine;

// This class is intended to manage the UI health display only, and the updating/reseting of player health.
public class PlayerHealth : MonoBehaviour
{
    // Create a slideable range for the player health
    // Max is currently 10 because we use ten bars in the image.
    // This could be changed by adding more bars to the array.     
    [Range(1, 10)]
    [SerializeField] int soldierHealth = 10;

    [Range(1, 10)]
    [SerializeField] int workerHealth = 10;
    [SerializeField] UnityEngine.UI.Image[] shieldBars;
    [SerializeField] GameManager gameManager;
    int currentHealth;

    public void TakeDamage(int damageAmount)
    {
        // Public, because it's intended to be called by weapons script (or any script which damages the player)
        currentHealth -= damageAmount;
        Debug.Log("Took damage, HP: " + currentHealth);
        // Adjust UI bars to display changes. 
        AdjustShieldUI();

        // Check if health is less than zero and call a GameOver result if true. 
        if (currentHealth <= 0)
        {
            //PlayerGameOver();
            // At this point, when the player dies, we just get the option to respawn as a new type of ant.
            // Decrease lives.  

            Debug.Log("Died?? Lives");
            gameManager.AdjustLives(-1);

        }
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

    public void ResetHealth()
    {
        if (gameManager.IsSoldier())
        {
            currentHealth = soldierHealth;
        }
        else if (gameManager.IsWorker())
        {
            currentHealth = workerHealth;
        }
        // Once the correct health level is set, adjust the UI to match. 
        AdjustShieldUI();
    }
}
