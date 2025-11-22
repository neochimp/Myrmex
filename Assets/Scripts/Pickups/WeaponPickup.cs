using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : Pickup
{   
    // The type of weapon this pickup symbolizes.
    // A good use case of the scriptable object type.  
    [SerializeField] WeaponSO weaponSO; 

    protected override void OnPickup(ActiveWeapon activeWeapon)
    {   
        // See ActiveWeapon.CS
        activeWeapon.SwitchWeapon(weaponSO);
    }
}
