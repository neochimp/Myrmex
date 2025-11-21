using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MonoBehaviour
{
    FirstPersonController player;  
    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>(); 
    }
    void Start()
    {
        player = FindFirstObjectByType<FirstPersonController>();  
    }
    void Update()
    {   
        if(!player)
        {
            return; 
        }
        agent.SetDestination(player.transform.position);
    }

    void OnTriggerEnter(Collider other) {
        // Remove this method for non-explosive enemies such as ants. 
        // EnemyHealth.cs can keep many capabilities.
        // But they dont always need to be called. 
        // The ant just lacks a trigger right now. We dont want self destructing ants. 
        if (other.tag == "Player")
        {
            EnemyHealth enemyHealth = GetComponent<EnemyHealth>(); 
            enemyHealth.SelfDestruct(); 
        }
    }
}
