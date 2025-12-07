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
                Debug.Log("Picking up food!"); 
                Destroy(hit.collider.gameObject); 
                playerFood.SetActive(true); 
            }
            // If you worker ant is "carrying food", then dropping is available. 
            // Only one food carried at a time (toggle on/off)
            else if (playerFood.activeInHierarchy)
            {   
                // DROP FOOD
                Instantiate(foodPrefab, hit.point, Quaternion.identity);
                playerFood.SetActive(false); 
            }
        } 
    }
}
