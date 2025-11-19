using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : Pickup
{   
    // The type of weapon this pickup symbolizes. 
    [SerializeField] WeaponSO weaponSO; 

    protected override void OnPickup(ActiveWeapon activeWeapon)
    {
        activeWeapon.SwitchWeapon(weaponSO);
    }
}
