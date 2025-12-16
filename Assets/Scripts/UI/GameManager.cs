using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; 
using Cinemachine;
using StarterAssets;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{   
    [SerializeField] TMP_Text enemiesText; //enemies remaining on map
    [SerializeField] TMP_Text foodText; //food required to win 

    [SerializeField] TMP_Text livesText;
    [SerializeField] GameObject winText; // win game menu
    [SerializeField] GameObject respawnUI; // respawn game menu 
    [SerializeField] GameObject gameOverUI; // For game over (out of lives)


    [SerializeField] CinemachineVirtualCamera gameOverCamera;
    [SerializeField] CinemachineVirtualCamera playerFollowCamera; 
    //[SerializeField] CinemachineVirtualCamera abilityCamera; 

    [SerializeField] StarterAssetsInputs starterAssetsInputs; // Required in order to detect input
    [SerializeField] FirstPersonController FirstPersonController; // Also required for handling input (onPause for example). 

    [SerializeField] PlayerManager playerManager;

    const string ENEMIES_STRING = "Enemies Remaining: ";
    const string FOOD_STRING = "Food Required: ";
    int enemiesRemaining = 0;
    int winningFoodCOndition; // The amount of food that needs to be returned to win the game. 
    [SerializeField] int totalLives = 3; 

    // The highest priority camera will always be the POV. 
    // So adjusting priority switches cameras (that's unity behavior)
    const int virtualCameraPriority = 20;

    bool isSoldier;
    bool isWorker; 

    void Start()
    {   
        livesText.text = totalLives.ToString();
        Respawn();
    }

    public void AdjustLives(int lives)
    {
        totalLives += lives;
        livesText.text = totalLives.ToString();
        if(totalLives <= 0)
        {
            GameOver(); 
        }
        else if (lives <= -1)
        {
            // If this is a reduction of lives. 
            Respawn(); 
        }
    }

    void GameOver()
    {
        // Unparent the weapon cam to prevent errors after player deletion. 
        // It would be childed to a destroyed object
        //abilityCamera.parent = null; 
        // Increase priority to instigate a switch of cameras. 
        gameOverCamera.Priority = virtualCameraPriority;
        // Deactivate Cursor/Lock cursor: (because it's game over)
        StarterAssetsInputs starterAssetsInputs = FindAnyObjectByType<StarterAssetsInputs>(); 
        starterAssetsInputs.SetCursorState(false); 
        // Display the game over screen 
        gameOverUI.SetActive(true); 
        // Destroy the player. 
        Destroy(FindAnyObjectByType<PlayerHealth>().gameObject);
    }

    public void SetCondition(int condition)
    {
        winningFoodCOndition = condition;
        AdjustFoodCount(0); // Update the food count.  
    }

    void WinCondition()
    {   
        // This method can be changed and adapted as we see fit, 
        // Maybe it goes to a new scene, or there is a win menu, etc. 
        // For now it's useful just to register the condition. 
        // Increase priority to instigate a switch of cameras.
        gameOverCamera.Priority = virtualCameraPriority;
        // Deactivate Cursor/Lock cursor: (because win condition met)
        StarterAssetsInputs starterAssetsInputs = FindAnyObjectByType<StarterAssetsInputs>(); 
        starterAssetsInputs.SetCursorState(false); 
        winText.SetActive(true);
    }

    void SwitchPlayerCamera()
    {
        playerFollowCamera.Priority = virtualCameraPriority; // Return camera to follow position. 
        gameOverCamera.Priority = 0; 
    }

    void SwitchGameCamera()
    {
        // Increase camera priority to initiate a switch of cameras. 
        gameOverCamera.Priority = virtualCameraPriority;
        playerFollowCamera.Priority = 0; 
    }

    public void Respawn()
    {
        // Increase camera priority to initiate a switch of cameras. 
        SwitchGameCamera(); 
        starterAssetsInputs.SetCursorState(false); 
        // Display the respawning menu (pick the type of ant to respawn as) 
        respawnUI.SetActive(true);
        FirstPersonController.PauseGame(); 
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
            //playerManager.Respawn(); // this works better
            Respawn(); 
        }
    }

    public void AdjustFoodCount(int amount)
    {
        winningFoodCOndition += amount;
        foodText.text = FOOD_STRING + winningFoodCOndition.ToString();

        if (winningFoodCOndition <= 0)
        {
            WinCondition(); 
        }
    }
    public void RespawnSoldier()
    {   
        SwitchPlayerCamera(); 
        FirstPersonController.PauseGame(); 
        isSoldier = true;
        isWorker = false; 
        //pheremoneContainer.SetActive(false); // Soldier does not use pheremone UI
        playerManager.SpawnAnt(isSoldier, isWorker);
        respawnUI.SetActive(false);
        starterAssetsInputs.SetCursorState(true);
    }

    public void RespawnWorker()
    {   
        SwitchPlayerCamera();
        FirstPersonController.PauseGame();
        //ammoContainer.SetActive(false); // worker does not use ammo UI
        isSoldier = false;
        isWorker = true; 
        playerManager.SpawnAnt(isSoldier, isWorker);
        respawnUI.SetActive(false);
        starterAssetsInputs.SetCursorState(true); 
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
        Debug.LogWarning("Quit does not work in Unity editor, but QUIT button was pressed"); 
        Application.Quit(); 
    }

    public bool IsWorker()
    {
        return isWorker; 
    }

    public bool IsSoldier()
    {
        return isSoldier; 
    }
}
