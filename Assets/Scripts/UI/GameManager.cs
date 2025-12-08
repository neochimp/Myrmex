using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; 
using Cinemachine; 

public class GameManager : MonoBehaviour
{   
    [SerializeField] TMP_Text enemiesText; 
    [SerializeField] GameObject winText; 

    [SerializeField] GameObject respawnUI; 

    const string ENEMIES_STRING = "Enemies: ";
    int enemiesRemaining = 0;

    [SerializeField] PlayerManager playerManager;

    void Start()
    {
        playerManager.Respawn(); 
    }

    public void AdjustEnemyCount(int amount)
    {   
        // *note* This tracker could easily be adjusted to track multiple types of enemies, such as spiders, mantises, etc. 

        // Increase the enemies remaining tracker (or decrease if given negative argument value)
        enemiesRemaining += amount;
        // Adjust the TMP_Text object, including the newly updated value. 
        enemiesText.text = ENEMIES_STRING + enemiesRemaining.ToString(); 

        if (enemiesRemaining <= 0)
        {
            // Currently, the player wins the game when no enemies remain. 
            // This can be changed as development continues. 
            //winText.SetActive(true); 
            Debug.Log("No more enemies");
            //playerManager.SpawnWorker(); // this works
            playerManager.Respawn(); // this works better
        }
    }
    public void RespawnSoldier()
    {   
        Debug.Log("Respawning Soldier");
        playerManager.SpawnSoldier();
        respawnUI.SetActive(false);
    }

    public void RespawnWorker()
    {   
        Debug.Log("Respawning Soldier");
        playerManager.SpawnWorker();
        respawnUI.SetActive(false);
    }

    public void RestartLevel()
    {   
        // Obtain the current scene from the build hierarchy.
        // Load that scene. 
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene); 
    }

    public void Quit()
    {   
        // You can't actually Application.Quit the editor, has to be built. 
        Debug.LogWarning("Will not work in Unity editor"); 
        Application.Quit(); 
    }
}
