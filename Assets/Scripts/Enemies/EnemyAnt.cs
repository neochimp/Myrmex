using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnt : MonoBehaviour
{
    // Controls the robot enemy

    // The player controller is ported from the unity starter assets script (I did not make it)
    FirstPersonController player;
    // Critical for travelling on the nav mesh surface.   
    NavMeshAgent agent;
    GameObject target;
    public float aggroRange = 20f;
    public float attackRange = 2.2f;
    public float attackCooldown = 2f; //seconds between attacks
    public int damage = 1;
    float attackTimer = 0f;
    public AudioClip[] attackSounds;
    private AudioSource sfx;

    void Awake()
    {
        // Initialize the agent component (attached to this object)
        agent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        // Access the players controller (for input/output controls)
        player = FindFirstObjectByType<FirstPersonController>();
        SnapAgentToNavMesh(transform.position);
        sfx = gameObject.GetComponent<AudioSource>();
    }
    void Update()
    {
        target = FindNearestTarget();
        if (target == null) return;
        agent.SetDestination(target.transform.position);

        attackTimer += Time.deltaTime;
        if (inAttackRange())
        {
            if (attackTimer >= attackCooldown)
            {
                Attack();
                attackTimer = 0f;
            }
        }
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

    GameObject FindNearestTarget()
    {
        List<GameObject> targets = new List<GameObject>();
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("npc");
        targets.AddRange(npcs);
        targets.Add(GameObject.FindGameObjectWithTag("Player"));
        GameObject nearest = null;
        float best = Mathf.Infinity;
        if (targets.Count == 0) return nearest;
        foreach (GameObject t in targets)
        {
            float d = Vector3.Distance(transform.position, t.transform.position);
            if (d < best && d < aggroRange)
            {
                best = d;
                nearest = t;
            }
        }

        return nearest;
    }

    public bool inAttackRange()
    {
        float distToTarget = Vector3.Distance(transform.position, target.transform.position);
        return distToTarget < attackRange;
    }

    void Attack()
    {
        if (target.CompareTag("Player"))
        {
            target.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
        if (target.CompareTag("npc"))
        {
            target.GetComponent<FriendlyWorker>().TakeDamage(damage);
        }

        AudioSource.PlayClipAtPoint(attackSounds[Random.Range(0, attackSounds.Length - 1)], transform.position, 1f);
    }
}
