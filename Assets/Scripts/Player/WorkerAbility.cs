using Cinemachine;
using StarterAssets;
using TMPro; 
using UnityEngine;

public class WorkerAbility : MonoBehaviour
{
    // This should be similar to the Soldier ability, but simpler, all we need to do is interact with a food object.
    // If the player is next to a food object, pick it up and hold it.
    // If the player clicks primary again drop it.
    // Right click drops a pheremone marker, we have a certain amount.  
    // This is the public facing interface of the Weapon GameObject
    // It is used by Pickups, and contains the direct functionality 
    // (representing the weapon which is currently functional)
    // Shooting, zooming, animations etc is all handled within this class. 
    WorkerHandler foodHandler; 

    //Ability biteAbility; // This ability is static, it never switches.  
    //AbilitySO shootAbilitySO;

    AbilitySO foodAbilitySO; 


    // starterAssetsInputs is another unity imported script (I did not make this)
    // It handles an "action map" essentially making key bindings much easier.
    StarterAssetsInputs starterAssetsInputs;

    FirstPersonController firstPersonController; 
    Animator abilityAnimator;

   
    [SerializeField] AbilitySO primaryAbility; 

    [SerializeField] GameObject playerFood; 
    
    //[SerializeField] AbilitySO secondaryAbility; 

    float foodTimer = 0f; 

    void Awake()
    {
        // Note that in awake, we get only those items which are part of self OR parent objects (bound to exist)
        starterAssetsInputs = gameObject.GetComponentInParent<StarterAssetsInputs>();
       // abilityAnimator = gameObject.GetComponentInParent<Animator>(); 
        firstPersonController = gameObject.GetComponentInParent<FirstPersonController>(); 

        foodHandler = GetComponentInChildren<WorkerHandler>();
        foodAbilitySO = primaryAbility; 
    }

    void Start()
    {   
        // Begin the game by switching to the starting weapon, and initialize the main camera. 
       // SwitchAbility(primaryAbility); 
       // cam = GameObject.FindAnyObjectByType<CinemachineVirtualCamera>(); 
    }

    void Update()
    {   
        // See these methods for more detail.
        // Let's leave update clean.  
        HandleFood(); 
    }

    //public void SwitchAbility(AbilitySO abilitySO)
  //  {   
        // Instantiate the weapons prefab through its scriptable object
        // Then Retrieve the weapon script component. 
      /// WorkerHandler newAbility = Instantiate(abilitySO.AbilityPrefab, transform).GetComponent<WorkerHandler>(); 
        // Change current values for both. 
       // foodAbilitySO = abilitySO; // CHANGE THIS currentWeaponSO
  // }

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
            // if we have food (playerFood.activeInHierarchy, DropFood)


            // else fire the 
            // STOP the instantiating should happen in the ability. 

            // so lets be clear, the ability.cs needs to do a lot of work, 
            // it should hold the food prefab, and it should determine what do based on if playerFood is active or not. 
            foodHandler.FireWorkerAbility(foodAbilitySO); 
            // Reset the time now (because we already shot)
            foodTimer = 0f; 
            // No need to decrease ammo, because the bite has unlimited ammo.
            // Set false until the next click.  
            starterAssetsInputs.PrimaryInput(false); 
        }
    } 

   // public void DropFood(){
    //    if (playerFood.activeInHierarchy)
     //   {
        //   Instantiate(foodPrefab, transform); 
        //    playerFood.SetActive(false); 
       // }
   // }
}

