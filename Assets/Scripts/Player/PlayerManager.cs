using UnityEngine;
using Cinemachine;
using StarterAssets;

public class PlayerManager : MonoBehaviour
{   
    [SerializeField] GameObject WorkerAbilities;
    [SerializeField] GameObject SoldierAbilities;

    [SerializeField] Transform SpawnPoint; 

    [SerializeField] PlayerHealth playerHealth; 

    [SerializeField] GameObject gameOverUI;
    [SerializeField] GameObject respawnUI; 

    [SerializeField] CinemachineVirtualCamera gameOverCamera;
    [SerializeField] CinemachineVirtualCamera playerFollowCamera; 
    [SerializeField] CharacterController characterController; 

    [SerializeField] Transform abilityCamera;

    // The highest priority camera will always be the POV. 
    // So adjusting priority switches cameras (that's unity behavior)
    const int virtualCameraPriority = 20;

    bool isSoldier = false;
    bool isWorker = false; 
    void Start() 
    {   
        // Spawn as soldier
        SpawnSoldier(); 
    }

    public void Respawn()
    {
        // Unparent the weapon cam to prevent errors after player deletion. 
        // It would be childed to a destroyed object
        //abilityCamera.parent = null; 
        // Increase priority to instigate a switch of cameras. 
        gameOverCamera.Priority = virtualCameraPriority;
        // Deactivate Cursor/Lock cursor: (because it's game over)
        StarterAssetsInputs starterAssetsInputs = gameObject.GetComponent<StarterAssetsInputs>(); 
        starterAssetsInputs.SetCursorState(false); 
        // Display the game over screen 
        respawnUI.SetActive(true);
    }

    void GameOver()
    {
        // Unparent the weapon cam to prevent errors after player deletion. 
        // It would be childed to a destroyed object
        abilityCamera.parent = null; 
        // Increase priority to instigate a switch of cameras. 
        gameOverCamera.Priority = virtualCameraPriority;
        // Deactivate Cursor/Lock cursor: (because it's game over)
        StarterAssetsInputs starterAssetsInputs = gameObject.GetComponent<StarterAssetsInputs>(); 
        starterAssetsInputs.SetCursorState(false); 
        // Display the game over screen 
        gameOverUI.SetActive(true);
        // Destroy the player. 
        Destroy(gameObject);
    }

    public void SpawnSoldier()
    {   
        playerFollowCamera.Priority = virtualCameraPriority; // Return camera to follow position. 
        gameOverCamera.Priority = 0; 
        characterController.enabled = false; // disable movement (must be done in unity)

        gameObject.transform.position = SpawnPoint.position; // perform teleport

        Debug.Log("Becoming Soldier"); // form switch

        WorkerAbilities.SetActive(false);
        SoldierAbilities.SetActive(true); 
        isSoldier = true; 
        isWorker = false; 

        characterController.enabled = true; // return input

        playerHealth.ResetHealth(); // health reset
    }

    public void SpawnWorker()
    {   
        playerFollowCamera.Priority = virtualCameraPriority; // Return camera to follow position. 
        gameOverCamera.Priority = 0; 
        //Debug.Log("Original Position: " + gameObject.transform.position + " Moving to " + SpawnPoint.position);

        // 1. Disable movement
        characterController.enabled = false;

        // 2. Teleport
        transform.position = SpawnPoint.position;

        // 3. Reset velocity & input state
        //characterController.ResetInput();

        // 4. Re-enable movement
        characterController.enabled = true;

        // 5. Switch form
        Debug.Log("Becoming Worker");
        WorkerAbilities.SetActive(true);
        SoldierAbilities.GetComponent<SoldierAbility>().ResetSoldier(); // See SoldierAbility.cs 
        SoldierAbilities.SetActive(false);
    
        isSoldier = false;
        isWorker = true;

        // 6. Reset health AFTER switching role
        playerHealth.ResetHealth();
    }

    public bool IsSoldier()
    {
        return isSoldier; 
    }

    public bool IsWorker()
    {
        return isWorker; 
    }

    // Note: you might be able to get rid of this spawn worker class
    // public void SpawnWorker()
    // {
    //     // Reset Health
    //     // Reset Ammo
    //     // Reset transform 
    //     gameObject.transform.position = SpawnPoint.position;
    //     BecomeWorker(); 
    //     playerHealth.ResetHealth();
    // }

    // public void SpawnSoldier()
    // {
    //     //
    // }

    

}
