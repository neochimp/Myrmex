using Cinemachine;
using StarterAssets;
using TMPro; 
using UnityEngine;

public class WorkerAbility : MonoBehaviour
{
    // Similar to the soldier Abilities. 
    WorkerHandler foodHandler; 
    AbilitySO foodAbilitySO; 
    StarterAssetsInputs starterAssetsInputs;
    FirstPersonController firstPersonController; 
    Animator abilityAnimator;

   
    [SerializeField] AbilitySO primaryAbility; 

    [SerializeField] GameObject playerFood; 
    
    //[SerializeField] AbilitySO secondaryAbility; 

    float foodTimer = 0f; 

    void Awake()
    {
        starterAssetsInputs = gameObject.GetComponentInParent<StarterAssetsInputs>();
       // abilityAnimator = gameObject.GetComponentInParent<Animator>(); 
        firstPersonController = gameObject.GetComponentInParent<FirstPersonController>(); 

        foodHandler = GetComponentInChildren<WorkerHandler>();
        foodAbilitySO = primaryAbility; 
    }

    // There is no Start method because there is nothing to switch to,
    // The worker abilities are static (they don't get upgraded or change)

    void Update()
    {   
        if (!firstPersonController.isPaused())
        {
            HandleFood(); 
        }
    }
    void HandleFood()
    {
        foodTimer += Time.deltaTime;

        if (!starterAssetsInputs.primary)
        {   
            // eliminate one indentation block
            // If no input detected from player, then dont handle it.
            return;
        }

        if(foodTimer >= foodAbilitySO.FireRate)
        {   
            // Ability to pickup food also has a firing rate, to prevent it getting spammed or wierd behaviors.

            foodHandler.FireWorkerAbility(foodAbilitySO); 
            foodTimer = 0f; 
            // False until next click (toggle on/off) 
            starterAssetsInputs.PrimaryInput(false); 
        }
    } 
}

