//using System.Numerics;
using UnityEngine;

public class WorkerHandler : MonoBehaviour
{
    [SerializeField] GameObject foodPrefab;
    [SerializeField] LayerMask interactionLayers;

    [SerializeField] GameObject playerFood;

    [SerializeField] float foodXOffset = 0f;
    [SerializeField] float foodYOffset = 0.1f;

    [SerializeField] float foodZOffset = 0.1f;

    [SerializeField] AudioClip drop;
    private AudioSource sfx;
    void Awake()
    {
        sfx = gameObject.GetComponent<AudioSource>();
    }
    public void FireWorkerAbility(AbilitySO abilitySO)
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, abilitySO.range, interactionLayers, QueryTriggerInteraction.Ignore))
        {
            //PICKUP FOOD
            if (hit.collider.tag == "Food" && !playerFood.activeInHierarchy)
            {
                // Destroy the entire game object. 
                hit.collider.GetComponentInParent<FoodItem>().DestroyFood();
                playerFood.SetActive(true);
            }
            //MINE FOOD
            else if (hit.collider.tag == "FoodSource" && !playerFood.activeInHierarchy)
            {
                // Mining functionality prototyping 
                // The Scriptable Object system holds the Damage Effect, which we invoke here. 
                // (It adds a "damage effect" like we chomped the apple)
                Instantiate(abilitySO.DamageEffect, hit.point, Quaternion.identity);
                // Get the script attached to the FoodSource object and use its public method. 
                FoodSource foodSource = hit.collider.GetComponentInParent<FoodSource>();
                foodSource.TakeChomp(abilitySO.Damage);
            }
            // DROP FOOD AT NEST
            else if (hit.collider.tag == "Nest" && playerFood.activeInHierarchy)
            {
                // Deactivate food
                playerFood.SetActive(false);
                // Spawn particle effect
                Instantiate(abilitySO.DamageEffect, hit.point, Quaternion.identity);
                // Reduce food count by one using the script attached to nest. 
                hit.collider.GetComponentInParent<Nest>().DeliverFood();
                //Drop SFX
                sfx.PlayOneShot(drop);
            }
            //DROP FOOD
            else if (playerFood.activeInHierarchy)
            {
                // If you worker ant is "carrying food", then dropping is available. 
                // Only one food carried at a time (toggle on/off)
                // DROP FOOD
                Vector3 offsetHit = new Vector3(
                    hit.point.x + foodXOffset,
                    hit.point.y + foodYOffset, // Otherwise food is embedded in the floor. 
                    hit.point.z + foodZOffset
                );
                Instantiate(foodPrefab, offsetHit, Quaternion.identity);
                playerFood.SetActive(false);
                sfx.PlayOneShot(drop);
            }
        }
    }
}
