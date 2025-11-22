using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : Pickup
{   
    // A inheritance of the Pickup class
    // Simple enough, the pickup stores an ammo amount which can be retrieved by OnPickup
    [SerializeField] int ammoAmount = 100;

    protected override void OnPickup(ActiveWeapon activeWeapon)
    {   
        // Active weapon contains the public method AdjustAmmo
        activeWeapon.AdjustAmmo(ammoAmount); 
    }
}
