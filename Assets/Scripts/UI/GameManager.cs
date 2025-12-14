using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; 
using Cinemachine;
using StarterAssets;

public class GameManager : MonoBehaviour
{   
    [SerializeField] TMP_Text enemiesText; 
    [SerializeField] GameObject winText; 

    [SerializeField] GameObject respawnUI;

    [SerializeField] GameObject ammoContainer; 

    [SerializeField] CinemachineVirtualCamera gameOverCamera;
    [SerializeField] CinemachineVirtualCamera playerFollowCamera; 

    [SerializeField] StarterAssetsInputs starterAssetsInputs; // Required in order to detect input
    [SerializeField] FirstPersonController FirstPersonController; // Also required for handling input (onPause for example). 

    [SerializeField] PlayerManager playerManager;

    const string ENEMIES_STRING = "Enemies: ";
    int enemiesRemaining = 0;

    // The highest priority camera will always be the POV. 
    // So adjusting priority switches cameras (that's unity behavior)
    const int virtualCameraPriority = 20;

    bool isSoldier;
    bool isWorker; 

    void Start()
    {
        Respawn(); 
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
    public void RespawnSoldier()
    {   
        SwitchPlayerCamera(); 
        FirstPersonController.PauseGame(); 
        isSoldier = true;
        isWorker = false; 
        playerManager.SpawnAnt(isSoldier, isWorker);
        respawnUI.SetActive(false);
        starterAssetsInputs.SetCursorState(true);
    }

    public void RespawnWorker()
    {   
        SwitchPlayerCamera();
        FirstPersonController.PauseGame();
        ammoContainer.SetActive(false); // worker does not use ammo UI
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
