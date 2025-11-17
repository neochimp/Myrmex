using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{   
    // The type of weapon this pickup symbolizes. 
    [SerializeField] WeaponSO weaponSO; 
    
    // The active weapon script attacked to the players weapon.
    // Serializing is not the only way to achieve this. 
    ActiveWeapon activeWeapon; 

    void OnTriggerEnter(Collider other) 
    {   
        if(other.tag == "Player")
        {
            activeWeapon = other.GetComponentInChildren<ActiveWeapon>();
            activeWeapon.SwitchWeapon(weaponSO);
            Destroy(gameObject);  
        }
    }
    
}
