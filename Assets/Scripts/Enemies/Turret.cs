using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{   
    [SerializeField] GameObject projectilePrefab; 

    // Initialize this to the barrel of the turret (not the base)
    [SerializeField] Transform turretHead; 

    // Initialize this to the players camera root
    [SerializeField] Transform playerTargetPoint;
    [SerializeField] Transform projectileSpawnPoint; 

    [SerializeField] float fireRate = 2f; 

    PlayerHealth player; 

    void Start()
    {
        player = FindFirstObjectByType<PlayerHealth>(); 
        StartCoroutine(FireRoutine()); 
    }

    void Update()
    {
        turretHead.LookAt(playerTargetPoint); 
    }

    IEnumerator FireRoutine()
    {
        while(player)
        {
            yield return new WaitForSeconds(fireRate);
            Instantiate(projectilePrefab, projectileSpawnPoint.position, turretHead.rotation); 
        }
    }
}
