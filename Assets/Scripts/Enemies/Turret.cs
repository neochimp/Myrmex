using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : MonoBehaviour
{   
    // Functionality for a rotating turret. 
    [SerializeField] GameObject projectilePrefab; 

    // Initialize this to the barrel of the turret (not the base)
    [SerializeField] Transform turretHead; 

    // Initialize this to the players camera root
    [SerializeField] Transform playerTargetPoint;
    [SerializeField] Transform projectileSpawnPoint; 

    [SerializeField] float fireRate = 2f; 

    [SerializeField] int damage = 2; 

    PlayerHealth player; 

    void Start()
    {   
        // Utilizes a co-routine, fire it once. 
        player = FindFirstObjectByType<PlayerHealth>(); 
        StartCoroutine(FireRoutine()); 
    }

    void Update()
    {   
        // Constantly swivel to find the player (camera root)
        turretHead.LookAt(playerTargetPoint); 
    }

    IEnumerator FireRoutine()
    {
        while(player)
        {   
            // Instantiate a projectile at the spawn point. 
            yield return new WaitForSeconds(fireRate);
            Projectile newProjectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, turretHead.rotation).GetComponent<Projectile>();
            newProjectile.Init(damage);
            newProjectile.transform.LookAt(playerTargetPoint);
        }
    }
}
