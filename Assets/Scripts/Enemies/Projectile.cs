using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{   
    PlayerHealth playerHealth;
    int damage = 2; 
    
    [SerializeField] float speed = 5f; 

    [SerializeField] Rigidbody projectile;

    void Update() 
    {
        FireProjectile(); 
    }
    void FireProjectile()
    {   
        // Use the velocity not other forces, this gives the best physics effect.
        // note frame rate independance. 
        projectile.velocity = transform.forward * speed * Time.deltaTime; 
    }

    void OnTriggerEnter(Collider other) 
    {
        playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth)
        {
            playerHealth.TakeDamage(damage); 
        }
    }

    public void Init(int damage)
    {
        this.damage = damage; 
    }
}
