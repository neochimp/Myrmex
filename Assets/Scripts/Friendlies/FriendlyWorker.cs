using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class FriendlyWorker : MonoBehaviour
{
    NavMeshAgent agent;
    float maxHealth = 10;
    float currentHealth;
    bool holdingFood;

    public Transform holdPoint; //this is where the food will stick to when the ant is holding food.
    public Transform home;
    private GameObject heldFood;
    Transform target;
    void Awake()
    {   
        // Initialize the agent component (attached to this object)
        agent = GetComponent<NavMeshAgent>(); 
        agent.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided");
        if (other.CompareTag("Food") && !holdingFood)
        {
            PickUp(other.gameObject);
        }
        if (other.CompareTag("Nest"))
        {
            Drop();
        }
        
    }

    void PickUp(GameObject food)
    {
        Debug.Log("picking up");
        heldFood = food;

        Collider col = food.GetComponent<Collider>();
        if(col != null)
        {
            col.enabled = false;
        }

        food.transform.SetParent(holdPoint);
        food.transform.localPosition = new Vector3(0, 2, 0);
        food.transform.localRotation = Quaternion.identity;
        holdingFood = true;
    }

    void Drop()
    {
        Debug.Log("Dropping");
        if(heldFood == null) return;
        GameObject food = heldFood;
        heldFood = null;
        food.transform.SetParent(null);
        food.transform.position = transform.position;
        Collider col = food.GetComponent<Collider>();
        if(col != null)
        {
            col.enabled = true;
        }
        holdingFood = false;
        //for visual testing purposes
        Destroy(food);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!holdingFood)
        {
            target = FindNearestFood();
            agent.SetDestination(target.position);
        }
        else
        {
           agent.SetDestination(home.position);
        }

    }

    Transform FindNearestFood()
    {
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
        if (foods == null) return null;
        Transform nearest = null;
        float best = Mathf.Infinity;

        foreach(var i in foods)
        {
            float d = Vector3.Distance(transform.position, i.transform.position);
            if(d < best)
            {
                best = d;
                nearest = i.transform;
            }
        }
         
        return nearest;
    }
}
