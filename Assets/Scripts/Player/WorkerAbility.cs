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
    Ability foodAbility; 

    //Ability biteAbility; // This ability is static, it never switches.  
    //AbilitySO shootAbilitySO;

    AbilitySO foodAbilitySO; 


    // starterAssetsInputs is another unity imported script (I did not make this)
    // It handles an "action map" essentially making key bindings much easier.
    StarterAssetsInputs starterAssetsInputs;

    FirstPersonController firstPersonController; 
    Animator abilityAnimator;

   
    [SerializeField] AbilitySO primaryAbility; 

    //[SerializeField] AbilitySO secondaryAbility; 

    float foodTimer = 0f; 

    void Awake()
    {
        // Note that in awake, we get only those items which are part of self OR parent objects (bound to exist)
        starterAssetsInputs = gameObject.GetComponentInParent<StarterAssetsInputs>();
       // abilityAnimator = gameObject.GetComponentInParent<Animator>(); 
        firstPersonController = gameObject.GetComponentInParent<FirstPersonController>(); 
    }

    void Start()
    {   
        // Begin the game by switching to the starting weapon, and initialize the main camera. 
        SwitchAbility(primaryAbility); 
       // cam = GameObject.FindAnyObjectByType<CinemachineVirtualCamera>(); 
    }

    void Update()
    {   
        // See these methods for more detail.
        // Let's leave update clean.  
        HandleFood(); 
    }

    public void SwitchAbility(AbilitySO abilitySO)
    {   
        // Instantiate the weapons prefab through its scriptable object
        // Then Retrieve the weapon script component. 
        Ability newAbility = Instantiate(abilitySO.AbilityPrefab, transform).GetComponent<Ability>(); 
        // Change current values for both. 
        foodAbility = newAbility; 
        foodAbilitySO = abilitySO; // CHANGE THIS currentWeaponSO
        // Now refill the magazine. (modular: note how each game object handles its own functionality, for the most part) 
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
            // A method of the Weapon.cs script
            foodAbility.WorkerAbility(foodAbilitySO); 
            // Reset the time now (because we already shot)
            foodTimer = 0f; 
            // No need to decrease ammo, because the bite has unlimited ammo.
            // Set false until the next click.  
            starterAssetsInputs.PrimaryInput(false); 
        }
    } 
}

