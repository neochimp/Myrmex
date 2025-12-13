using Cinemachine;
using StarterAssets;
using TMPro; 
using UnityEngine;
using UnityEngine.UIElements;

public class WorkerAbility : MonoBehaviour
{
    // Similar to the soldier Abilities. 
    WorkerHandler foodHandler; 
    AbilitySO foodAbilitySO; 
    StarterAssetsInputs starterAssetsInputs;
    FirstPersonController firstPersonController; 
    

   
    [SerializeField] AbilitySO primaryAbility; 

    [SerializeField] GameObject playerFood; 

    [SerializeField] PheromoneTrail pheromoneTrail; 
    
    //[SerializeField] AbilitySO secondaryAbility; 

    float foodTimer = 0f; 

    void Awake()
    {
        starterAssetsInputs = gameObject.GetComponentInParent<StarterAssetsInputs>();
       // abilityAnimator = gameObject.GetComponentInParent<Animator>(); 
        firstPersonController = gameObject.GetComponentInParent<FirstPersonController>(); 

        foodHandler = GetComponentInChildren<WorkerHandler>();
        foodAbilitySO = primaryAbility; 
    }

    // There is no Start method because there is nothing to switch to,
    // The worker abilities are static (they don't get upgraded or change)


    void Update()
    {   
        if (!firstPersonController.IsPaused())
        {
            HandleFood(); 
            SenseFood(); 
        }
    }
    void HandleFood()
    {
        foodTimer += Time.deltaTime;

        if (!starterAssetsInputs.primary)
        {   
            // eliminate one indentation block
            // If no input detected from player, then dont handle it.
            return;
        }

        if(foodTimer >= foodAbilitySO.FireRate)
        {   
            // Ability to pickup food also has a firing rate, to prevent it getting spammed or wierd behaviors.

            foodHandler.FireWorkerAbility(foodAbilitySO); 
            foodTimer = 0f; 
            // False until next click (toggle on/off) 
            starterAssetsInputs.PrimaryInput(false); 
        }
    }

    void SenseFood()
    {
        if (starterAssetsInputs.secondary)
        {   
            // While input detected, trail is always set to true. 
            pheromoneTrail.ShowTrail(true);  
        }
        else if (pheromoneTrail.TrailShowing())
        {   
            // If the trail is showing with no fire input
            // Then hide trail
            pheromoneTrail.ShowTrail(false); 
        }
    }

    void HandlePheremones()
    {
        // no timer needed. 
        // This activates on hold.
        //Find the nearest object tagged Food within X meters.

        //If found, spawn a visual effect that points from the player to that food.

        //Let the effect exist for Y seconds, then disappear.

       //respect pause and cooldown like other abilities.”

       //1. Get player position
       Transform currentTransform = this.gameObject.GetComponentInParent<Transform>(); 
       Vector3 loc = currentTransform.position; 
       //2. Find all objects tagged food. 
       //3. Loop through food. 
       //4. Find closest food.
       float smallest = 9999;
       FoodItem smallestFood = null;   
       foreach (FoodItem food in FindObjectsByType<FoodItem>(FindObjectsSortMode.None))
       {
            if(food.DistanceToTarget(currentTransform) < smallest)
            {
                smallest = food.DistanceToTarget(currentTransform);
                smallestFood = food;  
            }
       }
       Debug.Log("This is the smallest food " + smallestFood + "it is " + smallest + " distance away.");

       // Calculate the steps needed
       int steps = (int)smallest;
       // Classic vector formula for direction finding.
       
       Vector3 direction = (smallestFood.foodLocation().position - loc).normalized;
       for (int i = 0; i < steps; i++)
       {
        // Current position + direction we need to travel * the magnitude (number of steps)
        //Instantiate(pheremoneVFX, loc + (direction * i), Quaternion.identity);
       } 

       // steps = distance/spacing
       // loop from i = 0 to steps
       // trailPoint = playerPos + direction * (spacing * i)
       // instantiate a pheremone puff at that position 
    }
}

