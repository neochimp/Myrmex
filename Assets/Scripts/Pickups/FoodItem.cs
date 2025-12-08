using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodItem : MonoBehaviour
{   
    // Activate this on and off to trigger food pickup
    [SerializeField] GameObject foodPrefab; 
    public void PickupFood()
    {
        // Activate the food model in the worker (so it looks like we picked up food)
        // destroy this model
        
        Destroy(this.gameObject); 
    }
}
