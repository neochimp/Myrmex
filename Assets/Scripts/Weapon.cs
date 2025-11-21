using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using StarterAssets;
using UnityEngine;

public class Weapon : MonoBehaviour
{
 
   ParticleSystem muzzleFlash;
   CinemachineImpulseSource impulseSource; 
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
        // Use the cameras position (reference is precached by unity, no need to save variable)
        // Then cast in the forward direction from the camera
        // Use out keyword with raycast hit
        // Raycast will travel for a distance of Infinity (essentially no distance cap)

        // Note that RayCastHit will only return a value if we hit a collider. This prevents null ref

        RaycastHit hit;
        impulseSource.GenerateImpulse(); 
        muzzleFlash.Play();

        
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, interactionLayers, QueryTriggerInteraction.Ignore))
        {   
            if (hit.collider.tag == "Enemy")
            {
                EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                enemyHealth.TakeDamage(weaponSO.Damage);
                Instantiate(weaponSO.DamageEffect, hit.point, Quaternion.identity);
            }
            else
            {
                Instantiate(weaponSO.HitEffect, hit.point, Quaternion.identity);
            }
        }
    }
}
