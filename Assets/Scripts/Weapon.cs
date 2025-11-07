using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class Weapon : MonoBehaviour
{   
    StarterAssetsInputs starterAssetsInputs; 
    void Awake()
    {   
        // Get this main script from the parent (player)
        // This starter assets script is created by the FirstPersonController, a controller given by unity for moduler input binding. 
        starterAssetsInputs = gameObject.GetComponentInParent<StarterAssetsInputs>(); 
    }
    void Update()
    {
        Shoot(); 
    }

    void Shoot()
    {   
        // Use the cameras position (reference is precached by unity, no need to save variable)
        // Then cast in the forward direction from the camera
        // Use out keyword with raycast hit
        // Raycast will travel for a distance of Infinity (essentially no distance cap)

        RaycastHit hit;

        if (starterAssetsInputs.shoot && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity))
        {
            Debug.Log(hit.collider.name);
        }
        starterAssetsInputs.ShootInput(false); // use the method to turn the public bool back to false. if you want to be super safe.
    }
}
