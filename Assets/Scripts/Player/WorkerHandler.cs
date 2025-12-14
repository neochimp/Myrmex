using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerHandler : MonoBehaviour
{   
    [SerializeField] GameObject foodPrefab; 
    [SerializeField] LayerMask interactionLayers; 

    [SerializeField] GameObject playerFood; 

    public void FireWorkerAbility(AbilitySO abilitySO)
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, abilitySO.range, interactionLayers, QueryTriggerInteraction.Ignore))
        {   
            // Pickup food if HIT food and NOT already carring food.
            if (hit.collider.tag == "Food" && !playerFood.activeInHierarchy)
            {   
                // PICKUP FOOD
                // Destroy the entire game object. 
                Destroy(hit.collider.GetComponentInParent<FoodItem>().gameObject);
                playerFood.SetActive(true); 
            }
            else if(hit.collider.tag == "FoodSource" && !playerFood.activeInHierarchy)
            {
                // Mining functionality prototyping 
                Debug.Log("You hit a food source"); 
                // The Scriptable Object system holds the Damage Effect, which we invoke here. 
                // (It adds a "damage effect" like we chomped the apple)
                Instantiate(abilitySO.DamageEffect, hit.point, Quaternion.identity);
                // Get the script attached to the FoodSource object and use its public method. 
                FoodSource foodSource = hit.collider.GetComponentInParent<FoodSource>();
                foodSource.TakeChomp(abilitySO.Damage); 
            }
            else if (playerFood.activeInHierarchy)
            {   
                // If you worker ant is "carrying food", then dropping is available. 
                // Only one food carried at a time (toggle on/off)
                // DROP FOOD
                Instantiate(foodPrefab, hit.point, Quaternion.identity);
                playerFood.SetActive(false); 
            }
        } 
    }
}
