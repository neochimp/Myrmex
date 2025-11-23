using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{   
    // Initialize this to the barrel of the turret (not the base)
    [SerializeField] Transform turretHead; 

    // Initialize this to the players camera root
    [SerializeField] Transform playerTargetPoint;

    void Update()
    {
        turretHead.LookAt(playerTargetPoint); 
    }
}
