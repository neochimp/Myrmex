using Unity.VisualScripting;
using UnityEngine;

public class FoodSource : MonoBehaviour
{   
    // Representing the big food source, from which smaller particles fall. 
    [SerializeField] int startingFoodAmount = 10;
    [SerializeField] int stageTwoCutoff = 5; 
    [SerializeField] GameObject foodParticle; 

    [SerializeField] GameObject stageOne;
    [SerializeField] GameObject stageTwo; 

    [SerializeField] Transform anchorPoint; // The anchor must always be grounded on the nav mesh. 

    // GameManager used for adjusting UI
    GameManager gameManager; 
    int currentFood;
    
    public void TakeChomp(int chompAmount)
    {
        // Public function, to be called by WorkerHandler.cs 
        currentFood -= chompAmount;
        Debug.Log("You damaged the food source for an amount of " + chompAmount);
        if (currentFood <= (stageTwoCutoff))
        {   
            // Once the stage/two cut off
            NextStage(); 
        }
        // Simply check if currentFood is less than zero and destroy if true. 
        if(currentFood <= 0)
        {   
            // do some shit 
        }
    }

    void Awake()
    {
        currentFood = startingFoodAmount;
    }
    void Start()
    {   
        // Begin in Stage Two form. 
        stageOne.SetActive(true);
        stageTwo.SetActive(false); 
    }

    void NextStage()
    {   
        // Move into stage two. 
        stageOne.SetActive(false);
        stageTwo.SetActive(true); 
    }

    public float DistanceToTarget(Transform target)
    {
        Transform currentPosition = gameObject.GetComponent<Transform>(); 

        return (target.position - currentPosition.position).sqrMagnitude; 
    }

    public Transform foodLocation()
    {   
        //Debug.Log("I am " + gameObject.name + "My location is " + anchorPoint.position); 
        return anchorPoint; 
    }
}
