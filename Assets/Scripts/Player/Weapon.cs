using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using StarterAssets;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    // Contains the main functionality for all weapons in common
    // That is: the ability to shoot. 
   ParticleSystem muzzleFlash;

   // Impulse source is for screen shake effects
   CinemachineImpulseSource impulseSource; 

   // The interaction layers are how we adjust raycasting
   // For example if we don't want to shoot pickups, add them to their own protected layer
   [SerializeField] LayerMask interactionLayers; 

   void Start()
    {
        // Yes we could have serialized a field, but I don't want a bunch of serializations. 
       muzzleFlash = GetComponentInChildren<ParticleSystem>();  
       impulseSource = GetComponent<CinemachineImpulseSource>(); 
    }
    public void Shoot(WeaponSO weaponSO)
    {   
        // **A NOTE ON RAYCASTING**
        // Use the cameras position (reference is precached by unity as Camera, so no need to save variable)
        // Then, cast our from the forward direction of main camera
        // Use out keyword with raycast hit, :out: stores the information of objects we hit (among other data)
        // Raycast will travel for a distance of Infinity (essentially no distance cap on projectiles)

        // Note that RayCastHit will only return a value if we hit a collider. This prevents null ref (for example if player shoots at sky)
        RaycastHit hit;
        // Generate a screen shake
        impulseSource.GenerateImpulse(); 
        // Muzzle flash particle effect
        muzzleFlash.Play();

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, interactionLayers, QueryTriggerInteraction.Ignore))
        {   
            if (hit.collider.tag == "Enemy")
            {
                EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                enemyHealth.TakeDamage(weaponSO.Damage);
                // A special particle effect to visualize damage. 
                Instantiate(weaponSO.DamageEffect, hit.point, Quaternion.identity);
            }
            else
            {   
                // Just generate the standard visual for shooting an object. 
                Instantiate(weaponSO.HitEffect, hit.point, Quaternion.identity);
            }
        }
    }
}
