using Cinemachine;
using StarterAssets;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SoldierAbility : MonoBehaviour
{
    // The public facing interface of the Soldier Ants abilities,
    // It is used by Pickups, the Player GameObject itself, and contains the direct functionality 
    // Shooting, zooming, animations etc is all handled within this class. 
    SoldierHandler shootHandler; // These handler classes manage the raycasting and implementation of mechanics. 

    SoldierHandler biteHandler; // This ability is static, it never switches out.  

    AbilitySO shootAbilitySO; // Scriptable objects holding data for the two different abilities (modular)

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

    [SerializeField] GameObject ammoUI;
    [SerializeField] GameObject enemyText; // Only soldier needs to track enemies. 
    [SerializeField] GameObject soldierHeader;
    [SerializeField] GameObject soldierControlsUI;
    TMP_Text ammoText;

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
        // This line retrieves the handler script from the Mandibles prefab that is installed on the player. 
        // Prefabs hold their own handlers. 
        biteHandler = GetComponentInChildren<SoldierHandler>();
        // Load in the scriptable object for bite. 
        biteAbilitySO = secondaryAbility;
        ammoText = ammoUI.GetComponentInChildren<TMP_Text>();
    }

    void Start()
    {
        // Begin the game by switching to the starting ability, and initialize the main camera.  
        cam = GameObject.FindAnyObjectByType<CinemachineVirtualCamera>();
        SwitchAbility(primaryAbility);
    }

    void Update()
    {
        // See these methods for more detail.
        // Let's leave update clean.  
        if (!firstPersonController.IsPaused())
        {
            HandleShoot();
            HandleZoom();
            HandleBite();
        }
    }


    void OnEnable()
    {
        // When soldier abilties enabled, reset to baseline. 
        Reset();
    }

    void OnDisable()
    {
        if (ammoUI)
        {
            ammoUI.SetActive(false); // Worker does not use ammo UI
        }
        if (zoomVignette)
        {
            UnzoomWeapon(); // If you died as a soldier with the sniper zoomed, then respawned as a worker, this would be a nasty glitch. 
        }
        if (enemyText)
        {
            enemyText.SetActive(false);
        }
        if (soldierHeader)
        {
            soldierHeader.SetActive(false);
        }
        if (soldierControlsUI)
        {
            soldierControlsUI.SetActive(false);
        }
    }

    public void Reset()
    {
        enemyText.SetActive(true);
        SwitchAbility(primaryAbility); // Switch ability back to primary (as if we just spawned)
    }

    public void AdjustAmmo(int amount)
    {
        if (!ammoUI.activeInHierarchy)
        {
            // Display ammo UI if not visible.
            ammoUI.SetActive(true);
        }

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

        // Instantiate the abilities prefab through its scriptable object where it is stored. 
        // At the same time, retrieve the handler script component. 
        SoldierHandler newHandler = Instantiate(abilitySO.AbilityPrefab, transform).GetComponent<SoldierHandler>();
        // Change current handler and scriptable object, implementing a "switch" of abilities. 
        shootHandler = newHandler;
        shootAbilitySO = abilitySO;
        // Now refill the magazine. (modular: note how each game object handles its own functionality, for the most part) 
        AdjustAmmo(shootAbilitySO.MagazineSize);
    }

    public void UnzoomWeapon()
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
        // OnPrimary is a method in StarterAssetsInputs.cs 
        // It is action mapped to fire on press and release of the left trigger
        // On release it will return a false value for the public :shoot: bool [because (isPressed == false)]
        // For this reason a release of the button also triggers a return of handleshoot(), but with a false value of shoot. 


        // Now thats explained here is the rest:
        // We track the frame rate independant time here. 
        shootTimer += Time.deltaTime;

        if (!starterAssetsInputs.primary)
        {
            // eliminate one indentation block
            // If we dont shoot, then dont handle it.
            return;
        }

        if (shootTimer >= shootAbilitySO.FireRate && currentAmmo > 0)
        {
            // You can see docs for this but abilityAnimator arguments: animation name, layer, and time to begin animation (0f = beginning)
            abilityAnimator.Play(SHOOT_STRING, 0, 0f);
            // A method of the SoldierHandler.cs script attached to the ability itself 
            shootHandler.FireSoldierAbility(shootAbilitySO);
            // Reset the time now (because we already shot)
            shootTimer = 0f;
            // Decrease ammo by one
            AdjustAmmo(-1);
        }

        if (!shootAbilitySO.IsAutomatic)
        {
            // If its not automatic, false (no shoot) UNTIL the next left mouse click
            // So if it IS, we can hold down and keep shooting. 
            starterAssetsInputs.PrimaryInput(false);
        }
    }

    void HandleBite()
    {
        // These handle methods could be refactored into two seperate function,
        // But it would require so many arguments, and two seperate functions, its simpler to seperate concerns right now.

        biteTimer += Time.deltaTime;

        if (!starterAssetsInputs.secondary)
        {
            return;
        }

        if (biteTimer >= biteAbilitySO.FireRate)
        {
            abilityAnimator.Play(BITE_STRING, 0, 0f);

            biteHandler.FireSoldierAbility(biteAbilitySO);

            biteTimer = 0f;
            // No need to decrease ammo, because the bite has unlimited ammo. 
        }
    }

    void HandleZoom()
    {
        // Allow any object to zoom, but only IF the scriptable object itself can zoom.
        if (!shootAbilitySO.CanZoom) return;

        if (starterAssetsInputs.special && !zoomVignette.activeInHierarchy)
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
            starterAssetsInputs.SpecialInput(false);
        }
        else if (starterAssetsInputs.special && zoomVignette.activeInHierarchy)
        {
            // That is, if the user toggles zoom, but we are already zoomed in, untoggle it.
            // So zoom is toggle on/off NOT hold down. 
            UnzoomWeapon();
            starterAssetsInputs.SpecialInput(false);
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
