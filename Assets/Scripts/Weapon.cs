using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    StarterAssetsInputs starterAssetsInputs;
    ParticleSystem muzzleFlash;
    Animator weaponAnimator;
    [SerializeField] GameObject hitEffect; 
    const string SHOOT_STRING = "Shoot"; 
    void Awake()
    {
        // Get this main script from the parent (player)
        // This starter assets script belongs to the PlayerCapsule
        // It is created by unity for moduler input binding (comes in the starter assets). 
        starterAssetsInputs = gameObject.GetComponentInParent<StarterAssetsInputs>();
        muzzleFlash = gameObject.GetComponentInChildren<ParticleSystem>();
        weaponAnimator = gameObject.GetComponentInParent<Animator>(); 
    }
    void Update()
    {
        HandleShoot(); 
    }

    void HandleShoot()
    {
        // Use the cameras position (reference is precached by unity, no need to save variable)
        // Then cast in the forward direction from the camera
        // Use out keyword with raycast hit
        // Raycast will travel for a distance of Infinity (essentially no distance cap)

        // Note that RayCastHit will only return a value if we hit a collider. This prevents null ref

        RaycastHit hit;
        const int damageAmount = 45;

        if (!starterAssetsInputs.shoot)
        {
            // eliminate one indentation block
            return;
        }

        muzzleFlash.Play();
        // You can see docs for this but arguments: animation name, layer, and time to begin animation (0f = beginning)
        weaponAnimator.Play(SHOOT_STRING, 0, 0f);
        // Then use this method to turn the public bool back to false. 
        // could also just have gotten the public shoot bool and turned it false, but using the method is clearer.
        starterAssetsInputs.ShootInput(false);

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity)) //&& hit.collider.tag == "Enemy"
        {
            if (hit.collider.tag == "Enemy")
            {
                EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                enemyHealth.TakeDamage(damageAmount);
            }
            // You could also just check that enemyHealth returns null with if(enemyHealth) but I chose to use a tag. 
            // EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
            // enemyHealth.TakeDamage(damageAmount);
            Vector3 hitLocation = hit.point;
            Quaternion angle = hitEffect.transform.rotation;
            Instantiate(hitEffect, hitLocation, angle);
        }
    }
}
