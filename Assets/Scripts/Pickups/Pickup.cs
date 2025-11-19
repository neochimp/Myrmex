using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{   
    void OnTriggerEnter(Collider other) 
    {   
        if(other.tag == "Player")
        {   
            ActiveWeapon activeWeapon; 
            activeWeapon = other.GetComponentInChildren<ActiveWeapon>();
            OnPickup(activeWeapon); 
            Destroy(this.gameObject); 
        }
    }

    protected abstract void OnPickup(ActiveWeapon activeWeapon); 
}
