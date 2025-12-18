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

    void PickUp(GameObject food)
    {
        Debug.Log("picking up");
        heldFood = food;

        Collider col = food.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        food.transform.SetParent(holdPoint);
        food.transform.localPosition = new Vector3(0, 1.5f, 0.5f);
        food.transform.localRotation = Quaternion.identity;
        holdingFood = true;
    }

    void Drop()
    {
        Debug.Log("Dropping");
        if (heldFood == null) return;
        GameObject food = heldFood;
        heldFood = null;
        food.transform.SetParent(null);
        food.transform.position = transform.position;
        Collider col = food.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
        holdingFood = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        SnapAgentToNavMesh(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (!holdingFood)
        {
            if (target = FindNearestFood())
            {
                agent.SetDestination(target.position);
                if (arrived())
                {
                    PickUp(target.gameObject);
                }
            }
        }
        else
        {
            target = home;
            agent.SetDestination(target.position);
            if (arrived())
            {
                Drop();
                holdingFood = false;
            }
        }



    }

    Transform FindNearestFood()
    {
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
        if (foods == null) return null;
        Transform nearest = null;
        float best = Mathf.Infinity;

        foreach (var i in foods)
        {
            float d = Vector3.Distance(transform.position, i.transform.position);
            if (d < best)
            {
                best = d;
                nearest = i.transform;
            }
        }

        return nearest;
    }
    public void SnapAgentToNavMesh(Vector3 desiredPosition)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(desiredPosition, out hit, 10f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
            Debug.Log("Agent warped to nearest NavMesh Point: " + hit.position);
        }
        else
        {
            Debug.LogWarning("Could not find a valid nav mesh position near: " + desiredPosition);
        }
    }

    bool arrived()
    {
        if (Vector3.Distance(transform.position, target.position) < 1f)
        {
            Debug.Log("hit");
            return true;
        }
        return false;
    }
}
