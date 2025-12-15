using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MonoBehaviour
{   
    // Controls the robot enemy

    // The player controller is ported from the unity starter assets script (I did not make it)
    FirstPersonController player;
    // Critical for travelling on the nav mesh surface.   
    NavMeshAgent agent;

    void Awake()
    {   
        // Initialize the agent component (attached to this object)
        agent = GetComponent<NavMeshAgent>(); 
    }
    void Start()
    {   
        // Access the players controller (for input/output controls)
        player = FindFirstObjectByType<FirstPersonController>();  
    }
    void Update()
    {   
        if(!player)
        {   
            // No player controller == nothing to do/possible game over condition
            return; 
        }
        // The robot intends to seek out the players current location (chasing)
        agent.SetDestination(player.transform.position);
    }

    void OnTriggerEnter(Collider other) {
        // Handles explosive behavior
        // Robot pre-fabs contain triggers in the form of capsule colliders
        // When this trigger is set...
        if (other.tag == "Player")
        {   
            // Use the enemy health script to call the public self destruct method. 
            // The robot will explode and damage the player. 
            // The damage itself is caused by Explosion.cs (the explosion prefab damages the player, NOT the robot)
            EnemyHealth enemyHealth = GetComponent<EnemyHealth>(); 
            enemyHealth.SelfDestruct(); 
        }
    }
}
