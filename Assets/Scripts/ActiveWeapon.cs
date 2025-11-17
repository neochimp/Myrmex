using StarterAssets; 
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{   
    Weapon currentWeapon; 
    StarterAssetsInputs starterAssetsInputs;
    Animator weaponAnimator;

    float t = 0f; 
    
    [SerializeField] WeaponSO weaponSO; 
 
    const string SHOOT_STRING = "Shoot"; 
    void Awake()
    {
        // This starter assets script belongs to the PlayerCapsule
        // It is created by unity for moduler input binding (comes in the starter assets). 
        starterAssetsInputs = gameObject.GetComponentInParent<StarterAssetsInputs>();
        weaponAnimator = gameObject.GetComponentInParent<Animator>(); 
        // These references are on gameobjects which must already exist, either the parent or the object itself..
        // So it's okay to call on awake(), others should go in start. 
    }

    void Start()
    {
        currentWeapon = GetComponentInChildren<Weapon>(); 
    }

    void Update()
    {   
        t += Time.deltaTime; 
        HandleShoot(); 
    }

    public void SwitchWeapon(WeaponSO weaponSO)
    {
        if (currentWeapon)
        {
            Destroy(currentWeapon.gameObject);
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
}
