using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Nest : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] int FoodRequired = 10; // The amount of food that needs to be returned to win the game. 
    [SerializeField] GameObject[] SpawnableAnts;
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        gameManager.SetCondition(FoodRequired); // Set the food required to win the game, serialized here. 
    }

    public int getCondition()
    {
        return FoodRequired; // The amount of food that needs to be returned to win the game.
    }

    public void DeliverFood()
    { // Decrease food count by 1. 
        gameManager.AdjustFoodCount(-1);
        if (Random.Range(0, 2) == 0)
        {
            spawnAnt();
        }
    }

    void spawnAnt()
    {
        Instantiate(SpawnableAnts[Random.Range(0, SpawnableAnts.Length)], new Vector3(transform.position.x, transform.position.y, transform.position.z + 4), Quaternion.identity);
    }
}
