using UnityEngine;

public abstract class Pickup : MonoBehaviour
{   
    // An abstract pickup class which will be inherited by children such as weapon, ammo, food pickups etc. 

    // A value which makes the pickup rotate for a visual effect.
    [SerializeField] float rotationSpeed = 100f; 

    void Update()
    {   
        // The rotation, note it's frame rate independance. 
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f); 
    }
    
    void OnTriggerEnter(Collider other) 
    {   
        if(other.tag == "Player")
        {   
            // This method calls OnPickup which will (must) be overidden by the inherited class
            // On pickup is intended to utilize various public methods of the activeWeapon
            // For this reason OnPickup recieves activeWeapon as an argument (a script attached to the ActiveWeapon obhect)
            // The pickup is then destroyed. 
            ActiveWeapon activeWeapon; 
            activeWeapon = other.GetComponentInChildren<ActiveWeapon>();
            OnPickup(activeWeapon); 
            Destroy(this.gameObject); 
        }
    }

    // All inherited classes MUST utilize OnPickup. 
    // Recall their are ways to overide this to perhaps have a version which uses no argument, or different arguments
    // This will be useful in the future for food pickups etc. 
    protected abstract void OnPickup(ActiveWeapon activeWeapon); 
}
