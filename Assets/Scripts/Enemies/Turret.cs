using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : MonoBehaviour
{   
    // Functionality for a rotating turret. 
    [SerializeField] GameObject projectilePrefab; 

    // Initialize this to the barrel of the turret (not the base)
    // Allows for rotation of head only. 
    [SerializeField] Transform turretHead; 

    // Initialize this to the players camera root
    [SerializeField] Transform playerTargetPoint;
    
    // Fire point for projectiles
    [SerializeField] Transform projectileSpawnPoint; 

    [SerializeField] float fireRate = 2f; 

    [SerializeField] int damage = 2; 

    // The speed with which individual projectiles fire
    // Too high == tunelling, so be careful
    [SerializeField] float speed = 30f; 

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
            // At the same time, get the script of the projectils to initialize its damage and speed (pass values through Init method)
            Projectile newProjectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, turretHead.rotation).GetComponent<Projectile>();
            newProjectile.Init(damage, speed);
            // Change rotation due to parallax error, the projectile will spawn facing from the pivot point, until corrected by LookAt
            // See parallax error in rifling. 
            newProjectile.transform.LookAt(playerTargetPoint);
        }
    }
}
