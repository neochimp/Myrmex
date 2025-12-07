using Cinemachine;
using StarterAssets;
using TMPro; 
using UnityEngine;

public class SoldierAbility : MonoBehaviour
{   
    // This is the public facing interface of the Weapon GameObject
    // It is used by Pickups, and contains the direct functionality 
    // (representing the weapon which is currently functional)
    // Shooting, zooming, animations etc is all handled within this class. 
    SoldierHandler shootHandler; 

    SoldierHandler biteHandler; // This ability is static, it never switches.  
    AbilitySO shootAbilitySO;

    AbilitySO biteAbilitySO; 


    // starterAssetsInputs is another unity imported script (I did not make this)
    // It handles an "action map" essentially making key bindings much easier.
    StarterAssetsInputs starterAssetsInputs;

    FirstPersonController firstPersonController; 
    Animator abilityAnimator;

    // This t is used to measure timing in HandleShoot, controlling fire rate. 
    float shootTimer = 0f; 
    // Likewise for the secondary bite ability. 
    float biteTimer = 0f; 
    int currentAmmo; 

    // You would very rarely want to change these defaults. 
    // Refers to camera zoom, and angular movement. 
    const float DefaultRotationSpeed = 1f; 

    const float DefaultFOV = 40f; 

    // It makes most sense to serialize these.
    // The camera is used to prevent clipping. 
    // The zoomVignette is for the sniper rifle zoom in (and potentially others)
    // The ammo text is part of the UI overlay. 
    [SerializeField] AbilitySO primaryAbility; 

    [SerializeField] AbilitySO secondaryAbility; 

    [SerializeField] GameObject zoomVignette; 

    // We have this present to prevent clipping of ability objects, they render over the main camera to solve that issue. 
    [SerializeField] Camera abilityCamera; 
    [SerializeField] TMP_Text ammoText; 
    
    CinemachineVirtualCamera cam; 
    const string SHOOT_STRING = "Shoot";

    const string BITE_STRING = "Bite"; 

    void Awake()
    {
        // Note that in awake, we get only those items which are part of self OR parent objects (bound to exist)
        starterAssetsInputs = gameObject.GetComponentInParent<StarterAssetsInputs>();
        abilityAnimator = gameObject.GetComponentInParent<Animator>(); 
        firstPersonController = gameObject.GetComponentInParent<FirstPersonController>(); 
        // No zoomVignette because no zoom. When this is true, user sees a rifle scope. 
        zoomVignette.SetActive(false); 
        // This line retrieves the ability script from the Mandibles prefab that is installed on the player. 
        biteHandler = GetComponentInChildren<SoldierHandler>();
        // Load in the scriptable object for bite. 
        biteAbilitySO = secondaryAbility;
    }

    void Start()
    {   
        // Begin the game by switching to the starting weapon, and initialize the main camera. 
        SwitchAbility(primaryAbility); 
        cam = GameObject.FindAnyObjectByType<CinemachineVirtualCamera>(); 
    }

    void Update()
    {   
        // See these methods for more detail.
        // Let's leave update clean.  
        HandleShoot(); 
        HandleZoom(); 
        HandleBite(); 
    }

    public void AdjustAmmo(int amount)
    {   
        // Update the amount 
        currentAmmo += amount; 

        // But importantly, if the ammo amount would surpass the fixed magazine size of that scriptable object.
        if (currentAmmo > shootAbilitySO.MagazineSize) 
        {   
            // Then simply fill the magazine, but dont surpass it
            // (for example we don't want a pistol to hold 100 rounds)
            currentAmmo = shootAbilitySO.MagazineSize;
        }

        // Two decimals worth of ammo info
        // (for example 2 rounds left displays as 02)
        ammoText.text = currentAmmo.ToString("D2"); 
    }

    public void SwitchAbility(AbilitySO abilitySO)
    {   
        if (shootHandler)
        {   
            // We first destroy the previous object
            Destroy(shootHandler.gameObject);
            // This is important, otherwise the player might just be stuck zoomed in. 
            UnzoomWeapon(); 
        }

        // Instantiate the weapons prefab through its scriptable object
        // Then Retrieve the weapon script component. 
        SoldierHandler newHandler = Instantiate(abilitySO.AbilityPrefab, transform).GetComponent<SoldierHandler>(); 
        // Change current values for both. 
        shootHandler = newHandler; 
        shootAbilitySO = abilitySO; 
        // Now refill the magazine. (modular: note how each game object handles its own functionality, for the most part) 
        AdjustAmmo(shootAbilitySO.MagazineSize); 
    }

    void UnzoomWeapon()
    {   
        // Remove the image (no rifle scope)
        zoomVignette.SetActive(false);
        // Return to default rotation speed and camera FOV's. 
        // Rotation speed must change when zoomed, or we get extremely fast zoom movement. 
        firstPersonController.RotationSpeed = DefaultRotationSpeed; 
        cam.m_Lens.FieldOfView = DefaultFOV;
        abilityCamera.fieldOfView = DefaultFOV; 
    }

    void HandleShoot()
    {   
        //*NOTE*: because this is complicated
        // OnShoot is a method in StarterAssetsInputs.cs 
        // It is action mapped to fire on press and release of the left trigger
        // On release it will return a false value for the public :shoot: bool [because (isPressed == false)]
        // For this reason a release of the button also triggers a return of handleshoot(), but with a false value of shoot. 


        // Now thats explained here is the rest:
        // We track the frame rate independant time here. 
        shootTimer += Time.deltaTime;

        if (!starterAssetsInputs.primary)
        {   
            // eliminate one indentation block
            // If we dont shoot, the dont handle it.
            return;
        }

        if(shootTimer >= shootAbilitySO.FireRate && currentAmmo > 0)
        {
            // You can see docs for this but WeaponAnimator arguments: animation name, layer, and time to begin animation (0f = beginning)
            abilityAnimator.Play(SHOOT_STRING, 0, 0f);
            // A method of the Ability.cs script attached to the ability itself 
            shootHandler.FireSoldierAbility(shootAbilitySO); 
            // Reset the time now (because we already shot)
            shootTimer = 0f; 
            // Decrease ammo, and you get a nice magic number here :)
            AdjustAmmo(-1);
        }

        if(!shootAbilitySO.IsAutomatic)
        {   
            // If its not automatic, false (no shoot) UNTIL the next left mouse click
            // So if it IS, we can hold down and keep shooting. 
            starterAssetsInputs.PrimaryInput(false); 
        }
    }

    void HandleBite()
    {
        
        biteTimer += Time.deltaTime;

        if (!starterAssetsInputs.secondary)
        {   
            // eliminate one indentation block
            // If we dont shoot, the dont handle it.
            return;
        }

        if(biteTimer >= biteAbilitySO.FireRate)
        {
            // You can see docs for this but WeaponAnimator arguments: animation name, layer, and time to begin animation (0f = beginning)
            abilityAnimator.Play(BITE_STRING, 0, 0f);
            // A method of the Weapon.cs script
            biteHandler.FireSoldierAbility(biteAbilitySO); 
            // Reset the time now (because we already shot)
            biteTimer = 0f; 
            // No need to decrease ammo, because the bite has unlimited ammo. 
        }
    }

    void HandleZoom()
    {
        // Allow any object to zoom, but only IF the scriptable object itself can zoom.
        if (!shootAbilitySO.CanZoom) return; 

        if (starterAssetsInputs.special)
        {   
            zoomVignette.SetActive(true);
            // If you keep the same rotation speed when the camera zoom in...
            // You will get very fast and unplayable zoom mechanics.

            // An idea for future development
            // Each weapon could hold ITS OWN zoomImage for use on the overlay
            // Then muliple weapons could zoom, with various visuals (just a thought) 
            firstPersonController.RotationSpeed = shootAbilitySO.ZoomRotationSpeed;
            cam.m_Lens.FieldOfView = shootAbilitySO.ZoomAmount;
            abilityCamera.fieldOfView = shootAbilitySO.ZoomAmount; 
        }
        else
        {   
            UnzoomWeapon(); 
        }
    }
}

// A note on the overall design pattern:
        /*
        The scriptable objects for weapons contain all their data.
        The concern for the weapon script itself is the specific ray casting concerns and actual mechanisms of firing.
        The active weapon here, handles direct results of INPUT
        including animations, declerations, and updates. This is how we will seperate concerns.
        Scriptable objects are going to be one of the most useful things for this game as it builds further. 
        Seperation of concerns, specifically,seperating data from functionality.  
        This is how we can expand it into more of an ANT GAME - Jordan
        */