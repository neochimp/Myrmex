using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerManager : MonoBehaviour
{   
    [SerializeField] GameObject WorkerAbilities;
    [SerializeField] GameObject SoldierAbilities;

    [SerializeField] Transform SpawnPoint; 

    [SerializeField] PlayerHealth playerHealth; 

   // [SerializeField] FirstPersonController inputController; 
    [SerializeField] CharacterController characterController; 

    bool isSoldier = false;
    bool isWorker = false; 
    void Start() 
    {   
        // Spawn as soldier
        SpawnSoldier(); 
    }

    public void SpawnSoldier()
    {   
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
