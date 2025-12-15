using UnityEngine;

public class FoodSource : MonoBehaviour
{   
    // Representing the big food source, from which smaller particles fall. 
    //stageCutoffs[0] is the starting health and the subsequent numbers are the corresponding values to start the next stage
    [SerializeField] int[] stageCutoffs = {10, 5, 2};
    [SerializeField] int particleAmount; 
    [SerializeField] float particleSpawnRange; 

    const float groundPosition = -2.3f; 
    [SerializeField] GameObject foodParticle; 

    [SerializeField] GameObject[] stage;
    private int activeStage = 0;

    [SerializeField] Transform anchorPoint; // The anchor must always be grounded on the nav mesh. 

    // GameManager used for adjusting UI
    GameManager gameManager; 
    int currentFood;
    
    void Awake()
    {
        currentFood = stageCutoffs[0];
    }
    void Start()
    {   
        // Begin in Stage 0 form. 
        stage[0].SetActive(true);
        stage[1].SetActive(false); 
        stage[2].SetActive(false); 

    }

    void NextStage()
    {   
        Debug.Log("Turning off " + activeStage);
        stage[activeStage].SetActive(false);
        activeStage++;
        Debug.Log("Turning on " + activeStage);
        stage[activeStage].SetActive(true);
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
        // Simply check if currentFood is less than zero and destroy if true. 
        if(currentFood <= 0)
        {   
            BreakOffFood();
            Destroy(this.gameObject); 
            return;
        }

        if(currentFood <= stageCutoffs[activeStage+1])
        {
            //once the current food cutoff is met
            NextStage();
            BreakOffFood();
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
