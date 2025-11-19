using Cinemachine;
using StarterAssets;
using Unity.Mathematics;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{   
    Weapon currentWeapon; 
    StarterAssetsInputs starterAssetsInputs;

    FirstPersonController firstPersonController; 
    Animator weaponAnimator;

    float t = 0f; 
    
    [SerializeField] WeaponSO weaponSO; 
    [SerializeField] GameObject zoomVignette; 

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
        currentWeapon = GetComponentInChildren<Weapon>(); 
        cam = GameObject.FindAnyObjectByType<CinemachineVirtualCamera>(); 
    }

    void Update()
    {    
        HandleShoot(); 
        HandleZoom(); 
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
        }

        Weapon newWeapon = Instantiate(weaponSO.WeaponPrefab, transform).GetComponent<Weapon>(); 
        currentWeapon = newWeapon; 
        this.weaponSO = weaponSO; 
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

        if(t >= weaponSO.FireRate)
        {
           // You can see docs for this but arguments: animation name, layer, and time to begin animation (0f = beginning)
            weaponAnimator.Play(SHOOT_STRING, 0, 0f);
            currentWeapon.Shoot(weaponSO); 
            t = 0f; 
        }

        if(!weaponSO.IsAutomatic)
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
        if (!weaponSO.CanZoom) return; 

        if (starterAssetsInputs.zoom)
        {   
            zoomVignette.SetActive(true);
            firstPersonController.RotationSpeed = weaponSO.ZoomRotationSpeed; // MAGIC NUMBER
            cam.m_Lens.FieldOfView = weaponSO.ZoomAmount;
        }
        else
        {   
            zoomVignette.SetActive(false);
            firstPersonController.RotationSpeed = weaponSO.DefaultRotationSpeed; //MAGIC NUMBER
            cam.m_Lens.FieldOfView = weaponSO.DefaultFOV; 
        }

    }
}
