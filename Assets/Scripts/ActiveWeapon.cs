using Cinemachine;
using StarterAssets;
using Unity.Mathematics; 
using TMPro; 
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{   
    Weapon currentWeapon; 
    StarterAssetsInputs starterAssetsInputs;

    FirstPersonController firstPersonController; 
    Animator weaponAnimator;

    float t = 0f; 
    int currentAmmo; 
    
    [SerializeField] WeaponSO startingWeapon; 
    [SerializeField] GameObject zoomVignette; 

    [SerializeField] Camera weaponCamera; 
    [SerializeField] TMP_Text ammoText; 
    
    WeaponSO currentWeaponSO;
    CinemachineVirtualCamera cam; 
    const string SHOOT_STRING = "Shoot";

    void Awake()
    {
        // This starter assets script belongs to the PlayerCapsule
        // It is created by unity for moduler input binding (comes in the starter assets). 
        starterAssetsInputs = gameObject.GetComponentInParent<StarterAssetsInputs>();
        weaponAnimator = gameObject.GetComponentInParent<Animator>(); 
        firstPersonController = gameObject.GetComponentInParent<FirstPersonController>(); 
        zoomVignette.SetActive(false); 
        // These references are on gameobjects which must already exist, either the parent or the object itself..
        // So it's okay to call on awake(), others should go in start. 
    }

    void Start()
    {   
        SwitchWeapon(startingWeapon); 
        //AdjustAmmo(currentWeaponSO.MagazineSize); 
        cam = GameObject.FindAnyObjectByType<CinemachineVirtualCamera>(); 
    }

    void Update()
    {    
        HandleShoot(); 
        HandleZoom(); 
    }

    public void AdjustAmmo(int amount)
    {   
        // Important delta adjustment, we never want to surpass the magazine amount. 
        currentAmmo += amount; 

        if (currentAmmo > currentWeaponSO.MagazineSize) 
        {
            currentAmmo = currentWeaponSO.MagazineSize;
        }

        ammoText.text = currentAmmo.ToString("D2"); 
    }

    public void SwitchWeapon(WeaponSO weaponSO)
    {
        if (currentWeapon)
        {
            Destroy(currentWeapon.gameObject);
            // UNZOOM
            zoomVignette.SetActive(false);
            firstPersonController.RotationSpeed = weaponSO.DefaultRotationSpeed; 
            cam.m_Lens.FieldOfView = weaponSO.DefaultFOV;
            weaponCamera.fieldOfView = weaponSO.DefaultFOV; 
        }

        Weapon newWeapon = Instantiate(weaponSO.WeaponPrefab, transform).GetComponent<Weapon>(); 
        currentWeapon = newWeapon; 
        currentWeaponSO = weaponSO;
        // Now refill the magazine, but never by more than the mag capacity. 
        AdjustAmmo(currentWeaponSO.MagazineSize); 
    }

    void HandleShoot()
    {   
        //*NOTE*
        // OnShoot in StarterAssetsInputs.cs will fire on press and release
        // On release it will return a false value for .shoot (isPressed == false)
        // For this reason a release of the button also triggers a return of handleshoot(). 

        t += Time.deltaTime;

        if (!starterAssetsInputs.shoot)
        {
            // eliminate one indentation block
            return;
        }

        if(t >= currentWeaponSO.FireRate && currentAmmo > 0)
        {
           // You can see docs for this but arguments: animation name, layer, and time to begin animation (0f = beginning)
            weaponAnimator.Play(SHOOT_STRING, 0, 0f);
            currentWeapon.Shoot(currentWeaponSO); 
            t = 0f; 
            AdjustAmmo(-1);
        }

        if(!currentWeaponSO.IsAutomatic)
        {
            starterAssetsInputs.ShootInput(false); 
        }
        // Then use this method to turn the public bool back to false. 
        // could also just have gotten the public shoot bool and turned it false, but using the method is clearer.

        // A note on this pattern:
        /*
        The scriptable objects for weapons contain all their data.
        The concern for the weapon script itself is the specific ray casting concerns and actual mechanism.
        The active weapon here, handles the rest including animations, declerations, and updates. This is how we will seperate concerns.
        Scriptable objects are going to be one the most useful things for this game as it builds further. Seperation of concerns, specifically,
        seperating data from functionality.  
        */
    }

    void HandleZoom()
    {
        // Allow any object to zoom, IF the scriptable object itself can zoom.
        if (!currentWeaponSO.CanZoom) return; 

        if (starterAssetsInputs.zoom)
        {   
            zoomVignette.SetActive(true);
            firstPersonController.RotationSpeed = currentWeaponSO.ZoomRotationSpeed; // MAGIC NUMBER
            cam.m_Lens.FieldOfView = currentWeaponSO.ZoomAmount;
            weaponCamera.fieldOfView = currentWeaponSO.ZoomAmount; 
        }
        else
        {   
            zoomVignette.SetActive(false);
            firstPersonController.RotationSpeed = currentWeaponSO.DefaultRotationSpeed; //MAGIC NUMBER
            cam.m_Lens.FieldOfView = currentWeaponSO.DefaultFOV; 
            weaponCamera.fieldOfView = currentWeaponSO.DefaultFOV; 
        }

    }
}
