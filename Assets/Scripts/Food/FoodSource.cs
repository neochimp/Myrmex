using Unity.VisualScripting;
using UnityEngine;

public class FoodSource : MonoBehaviour
{   
    // Representing the big food source, from which smaller particles fall. 
    [SerializeField] int startingFoodHealth = 10;
    [SerializeField] int stageTwoCutoff = 5; 

    [SerializeField] int particleAmount; 
    [SerializeField] float particleSpawnRange; 

    const float groundPosition = -2.3f; 
    [SerializeField] GameObject foodParticle; 

    [SerializeField] GameObject stageOne;
    [SerializeField] GameObject stageTwo; 

    [SerializeField] Transform anchorPoint; // The anchor must always be grounded on the nav mesh. 

    // GameManager used for adjusting UI
    GameManager gameManager; 
    int currentFood;
    
    void Awake()
    {
        currentFood = startingFoodHealth;
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
    
    void BreakOffFood()
    {
        for (int i = 0; i < particleAmount; i++)
        {
            GameObject piece = Instantiate(foodParticle, transform);

            Vector3 localOffset = new Vector3(
                Random.Range(-particleSpawnRange, particleSpawnRange),
                groundPosition, // Otherwise particles float in the air. 
                Random.Range(-particleSpawnRange, particleSpawnRange)
            );

            piece.transform.localPosition = localOffset;
            piece.transform.localRotation = Quaternion.identity; // relative rotation 
            piece.transform.SetParent(null); // Remove from parent source hierarchy (because that object will be destroyed)
        }
    }
    public void TakeChomp(int chompAmount)
    {
        // Public function, to be called by WorkerHandler.cs 
        currentFood -= chompAmount;
        if (currentFood <= (stageTwoCutoff) && stageOne.activeInHierarchy)
        {   
            // Once the stage/two cut off
            NextStage(); 
            BreakOffFood(); 
        }
        // Simply check if currentFood is less than zero and destroy if true. 
        else if(currentFood <= 0)
        {   
            BreakOffFood();
            Destroy(this.gameObject); 
        }
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
