using Cinemachine;
using StarterAssets;
using Unity.Mathematics; 
using TMPro; 
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{   
    // This is the public facing interface of the Weapon GameObject
    // It is used by Pickups, and contains the direct functionality 
    // (representing the weapon which is currently functional)
    // Shooting, zooming, animations etc is all handled within this class. 
    Weapon currentWeapon; 

    // starterAssetsInputs is another unity imported script (I did not make this)
    // It handles an "action map" essentialy making key bindings much easier.
    StarterAssetsInputs starterAssetsInputs;

    FirstPersonController firstPersonController; 
    Animator weaponAnimator;

    // This t is used to measure timing in HandleShoot, controlling fire rate. 
    float t = 0f; 
    int currentAmmo; 

    // You would very rarely want to change these defaults. 
    // Refers to camera zoom, and angular movement. 
    const float DefaultRotationSpeed = 1f; 

    const float DefaultFOV = 40f; 

    // It makes most sense to serialize these.
    // The camera is used to prevent clipping. 
    // The zoomVignette is for the sniper rifle zoom in (and potentially others)
    // The ammo text is part of the UI overlay. 
    [SerializeField] WeaponSO startingWeapon; 
    [SerializeField] GameObject zoomVignette; 

    [SerializeField] Camera weaponCamera; 
    [SerializeField] TMP_Text ammoText; 
    
    WeaponSO currentWeaponSO;
    CinemachineVirtualCamera cam; 
    const string SHOOT_STRING = "Shoot";

    void Awake()
    {
        // Note that in awake, we get only those items which are part of self OR parent objects (bound to exist)
        starterAssetsInputs = gameObject.GetComponentInParent<StarterAssetsInputs>();
        weaponAnimator = gameObject.GetComponentInParent<Animator>(); 
        firstPersonController = gameObject.GetComponentInParent<FirstPersonController>(); 
        // No zoomVignette because no zoom. When this is true, user sees a rifle scope. 
        zoomVignette.SetActive(false); 
    }

    void Start()
    {   
        // Begin the game by switching to the starting weapon, and initialize the main camera. 
        SwitchWeapon(startingWeapon); 
        cam = GameObject.FindAnyObjectByType<CinemachineVirtualCamera>(); 
    }

    void Update()
    {   
        // See these methods for more detail.
        // Let's leave update clean.  
        HandleShoot(); 
        HandleZoom(); 
    }

    public void AdjustAmmo(int amount)
    {   
        // Update the amount 
        currentAmmo += amount; 

        // But importantly, if the ammo amount would surpass the fixed magazine size of that scriptable object.
        if (currentAmmo > currentWeaponSO.MagazineSize) 
        {   
            // Then simply fill the magazine, but dont surpass it
            // (for example we don't want a pistol to hold 100 rounds)
            currentAmmo = currentWeaponSO.MagazineSize;
        }

        // Two decimals worth of ammo info
        // (for example 2 rounds left displays as 02)
        ammoText.text = currentAmmo.ToString("D2"); 
    }

    public void SwitchWeapon(WeaponSO weaponSO)
    {
        if (currentWeapon)
        {   
            // We first destroy the previous object
            Destroy(currentWeapon.gameObject);
            // This is important, otherwise the player might just be stuck zoomed in. 
            UnzoomWeapon(); 
        }

        // Instantiate the weapons prefab through its scriptable object
        // Then tetrieve the weapon script component. 
        Weapon newWeapon = Instantiate(weaponSO.WeaponPrefab, transform).GetComponent<Weapon>(); 
        // Change current values for both. 
        currentWeapon = newWeapon; 
        currentWeaponSO = weaponSO;
        // Now refill the magazine. (modular: note how each game object handles its own functionality, for the most part) 
        AdjustAmmo(currentWeaponSO.MagazineSize); 
    }

    void UnzoomWeapon()
    {   
        // Remove the image (no rifle scope)
        zoomVignette.SetActive(false);
        // Return to default rotation speed and camera FOV's. 
        // Rotation speed must change when zoomed, or we get extremely fast zoom movement. 
        firstPersonController.RotationSpeed = DefaultRotationSpeed; 
        cam.m_Lens.FieldOfView = DefaultFOV;
        weaponCamera.fieldOfView = DefaultFOV; 
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
        t += Time.deltaTime;

        if (!starterAssetsInputs.shoot)
        {   
            // eliminate one indentation block
            // If we dont shoot, the dont handle it.
            return;
        }

        if(t >= currentWeaponSO.FireRate && currentAmmo > 0)
        {
            // You can see docs for this but WeaponAnimator arguments: animation name, layer, and time to begin animation (0f = beginning)
            weaponAnimator.Play(SHOOT_STRING, 0, 0f);
            // A method of the Weapon.cs script
            currentWeapon.Shoot(currentWeaponSO); 
            // Reset the time now (because we already shot)
            t = 0f; 
            // Decrease ammo, and you get a nice magic number here :)
            AdjustAmmo(-1);
        }

        if(!currentWeaponSO.IsAutomatic)
        {   
            // If its not automatic, false (no shoot) UNTIL the next left mouse click
            // So if it IS, we can hold down and keep shooting. 
            starterAssetsInputs.ShootInput(false); 
        }
    }

    void HandleZoom()
    {
        // Allow any object to zoom, but only IF the scriptable object itself can zoom.
        if (!currentWeaponSO.CanZoom) return; 

        if (starterAssetsInputs.zoom)
        {   
            zoomVignette.SetActive(true);
            // If you keep the same rotation speed when the camera zoom in...
            // You will get very fast and unplayable zoom mechanics.

            // An idea for future development
            // Each weapon could hold ITS OWN zoomImage for use on the overlay
            // Then muliple weapons could zoom, with various visuals (just a thought) 
            firstPersonController.RotationSpeed = currentWeaponSO.ZoomRotationSpeed;
            cam.m_Lens.FieldOfView = currentWeaponSO.ZoomAmount;
            weaponCamera.fieldOfView = currentWeaponSO.ZoomAmount; 
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