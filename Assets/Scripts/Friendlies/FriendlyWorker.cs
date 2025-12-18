using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class FriendlyWorker : MonoBehaviour
{
    NavMeshAgent agent;
    float maxHealth = 5;
    float currentHealth;
    bool holdingFood = false;

    public Transform holdPoint; //this is where the food will stick to when the ant is holding food.
    Transform home;
    private GameObject heldFood = null;
    Transform target;

    public AudioClip deathSound;
    private AudioSource sfx;

    public
    void Awake()
    {
        // Initialize the agent component (attached to this object)
        agent = GetComponent<NavMeshAgent>();
        home = GameObject.FindWithTag("Nest").transform;
        currentHealth = maxHealth;
        sfx = gameObject.GetComponent<AudioSource>();

        holdingFood = false;
        heldFood = null;
    }

    bool TryPickUp(GameObject food)
    {
        if (food == null)
        {
            return false;
        }

        if (!food.CompareTag("Food"))
        {
            return false;
        }

        food.tag = "CarriedFood";
        if (food.transform.childCount > 0)
        {
            Transform child = food.transform.GetChild(0);
            if (child != null)
            {
                child.tag = "CarriedFood";
            }
        }

        Debug.Log("picking up");
        heldFood = food;
        holdingFood = true;
        Collider col = food.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
        food.transform.SetParent(holdPoint);
        food.transform.localPosition = new Vector3(0, 1.5f, 0.5f);
        food.transform.localRotation = Quaternion.identity;

        return true;
    }

    void Drop()
    {
        if (heldFood == null) return;

        Debug.Log("Dropping");
        GameObject food = heldFood;
        heldFood = null;
        holdingFood = false;
        food.transform.SetParent(null);
        food.transform.position = transform.position;
        Collider col = food.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SnapAgentToNavMesh(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (agent == null || home == null) return;

        if (!holdingFood)
        {
            if (target == null || !target.CompareTag("Food"))
            {
                target = FindNearestFood();
            }
            if (target != null)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
                if (ArrivedAt(target.position))
                {
                    bool picked = TryPickUp(target.gameObject);
                    if (!picked)
                    {
                        target = null;
                    }
                }
            }
        }
        else
        {
            if (heldFood == null)
            {
                holdingFood = false;
                target = null;
                return;
            }
            agent.isStopped = false;
            target = home;
            agent.SetDestination(home.position);
            if (ArrivedAt(home.position))
            {
                Drop();
                target = null;
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

    bool ArrivedAt(Vector3 point)
    {
        if (agent.pathPending) return false;

        if (agent.remainingDistance <= agent.stoppingDistance + 0.1)
        {
            return true;
        }
        return Vector3.Distance(transform.position, point) < 1f;
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

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Worker ant health left: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        AudioSource.PlayClipAtPoint(deathSound, transform.position, 0.8f);
        Destroy(gameObject);
    }
}
