using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{   
    [SerializeField] GameObject WorkerAbilities;
    [SerializeField] GameObject SoldierAbilities;

    [SerializeField] Transform SpawnPoint; 

    [SerializeField] PlayerHealth playerHealth; 
    [SerializeField] CharacterController characterController; 
   
    // void GameOver()
    // {
    //     // Unparent the weapon cam to prevent errors after player deletion. 
    //     // It would be childed to a destroyed object
    //     abilityCamera.parent = null; 
    //     // Increase priority to instigate a switch of cameras. 
    //     gameOverCamera.Priority = virtualCameraPriority;
    //     // Deactivate Cursor/Lock cursor: (because it's game over)
    //     StarterAssetsInputs starterAssetsInputs = gameObject.GetComponent<StarterAssetsInputs>(); 
    //     starterAssetsInputs.SetCursorState(false); 
    //     // Display the game over screen 
    //     gameOverUI.SetActive(true);
    //     // Destroy the player. 
    //     Destroy(gameObject);
    // }

    public void SpawnAnt(bool isSoldier, bool isWorker)
    {   
        // 1. Disable movement
        characterController.enabled = false;

        // 2. Teleport
        transform.position = SpawnPoint.position;

        // 3. Re-enable movement
        characterController.enabled = true;

        // 4. Switch abilities to proper mode 
        WorkerAbilities.SetActive(isWorker);
        SoldierAbilities.SetActive(isSoldier); 

        // 5. Reset health back to standard
        playerHealth.ResetHealth(); // health reset 
    }
}
