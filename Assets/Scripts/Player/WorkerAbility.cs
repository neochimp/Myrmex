using Cinemachine;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class WorkerAbility : MonoBehaviour
{
    // Similar to the soldier Abilities. 
    WorkerHandler foodHandler;
    AbilitySO foodAbilitySO;
    StarterAssetsInputs starterAssetsInputs;
    FirstPersonController firstPersonController;

    [SerializeField] AbilitySO primaryAbility;

    [SerializeField] GameObject playerFood;

    [SerializeField] PheromoneTrail pheromoneTrail;

    [SerializeField] GameObject pheremoneContainer;
    [SerializeField] GameObject foodText;
    [SerializeField] GameObject workerHeader;
    [SerializeField] GameObject workerControlsUI;

    float foodTimer = 0f;

    void Awake()
    {
        starterAssetsInputs = gameObject.GetComponentInParent<StarterAssetsInputs>();
        // abilityAnimator = gameObject.GetComponentInParent<Animator>(); 
        firstPersonController = gameObject.GetComponentInParent<FirstPersonController>();

        foodHandler = GetComponentInChildren<WorkerHandler>();
        foodAbilitySO = primaryAbility;
    }

    // There is no Start method because there is nothing to switch to,
    // The worker abilities are static (they don't get upgraded or change)


    void Update()
    {
        if (!firstPersonController.IsPaused())
        {
            HandleFood();
            SenseFood();
        }
    }

    void OnEnable()
    {
        foodText.SetActive(true); // Display remaining food for workers. 
        workerControlsUI.SetActive(true); // Display worker controls.
        workerHeader.SetActive(true); // Display worker header.
    }

    void OnDisable()
    {
        if (pheremoneContainer)
        {
            pheremoneContainer.SetActive(false); // Soldier will not require this UI
        }
        if (pheromoneTrail)
        {
            pheromoneTrail.ShowTrail(false); // No trail remaining on screen please. 
        }
        if (foodText)
        {
            foodText.SetActive(false); // Soldier does not need to see food UI. 
        }
        if (workerHeader)
        {
            workerHeader.SetActive(false); // Soldier does not need to see worker header.
        }
        if (workerControlsUI)
        {
            workerControlsUI.SetActive(false); // Soldier does not need to see worker controls.
        }
    }
    void HandleFood()
    {
        foodTimer += Time.deltaTime;

        if (!starterAssetsInputs.primary)
        {
            // eliminate one indentation block
            // If no input detected from player, then dont handle it.
            return;
        }

        if (foodTimer >= foodAbilitySO.FireRate)
        {
            // Ability to pickup food also has a firing rate, to prevent it getting spammed or wierd behaviors.

            foodHandler.FireWorkerAbility(foodAbilitySO);
            foodTimer = 0f;
            // False until next click (toggle on/off) 
            starterAssetsInputs.PrimaryInput(false);
        }
    }

    void SenseFood()
    {
        if (starterAssetsInputs.secondary)
        {
            // While input detected, trail is always set to true. 
            pheromoneTrail.ShowTrail(true);
            pheremoneContainer.SetActive(true);
        }
        else if (pheromoneTrail.TrailShowing())
        {
            // If the trail is showing with no fire input
            // Then hide trail
            pheromoneTrail.ShowTrail(false);
            pheremoneContainer.SetActive(false);
        }
    }
}

