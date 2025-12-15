using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class PheromoneTrail : MonoBehaviour
{
    public Transform home;
    //public Transform targetItem;
    public float refreshRate = 0.2f;

    private LineRenderer line;
    private NavMeshPath path;

    [Header("Texture Stuff")]
    //public Texture2D pheromoneTexture;
    public GameObject pheromoneVFXPrefab;
    public float puffInterval = 0.07f;
    private float puffTimer = 0f;
    public int puffsPerInterval = 3;
    public float tubeRadius = 0.08f;

    bool showTrail = false; 

    private List<Vector3> lastTrailPoints = new List<Vector3>();

    // add these:
    private Vector3 lockedTargetPos = Vector3.zero;
    private bool hasLockedTarget = false; 
    
    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true;
        path = new NavMeshPath();

        // Create material
        Material mat = new Material(Shader.Find("Unlit/Transparent"));

        // Assign texture if you have one (recommended)
        //if (pheromoneTexture != null)
            //mat.mainTexture = pheromoneTexture;

        // Soft glowing pheromone color
        mat.color = new Color(0.5f, 1f, 0.8f, 0.7f);

        line.material = mat;

        // Trail width
        line.startWidth = 0f;
        line.endWidth = 0f;

        // Texture tiling mode so it repeats
        line.textureMode = LineTextureMode.Tile;
        line.material.mainTextureScale = new Vector2(3f, 1f);
        SetupGradient();
    }

    void OnEnable()
    {
        StartCoroutine(UpdatePathRoutine());
    }

    System.Collections.IEnumerator UpdatePathRoutine()
    {
        while (true)
        {   
            UpdatePath();
            yield return new WaitForSeconds(refreshRate);
        }
    }

    public void ShowTrail(bool flag)
    {   
        // Flag the trail (either showing or NOT showing)
        showTrail = flag; 

        if (showTrail)
        {
            // lock a target when trail is triggered by true flag
            Vector3 candidate = GetClosestFood();
            line.startWidth = 0f;
            line.endWidth = 0f; 
            if (candidate == Vector3.zero)
            {   
                // no valid food found
                hasLockedTarget = false;
                line.positionCount = 0;
            }
            else
            {   
                lockedTargetPos = candidate; 
                hasLockedTarget = true;
            }
            //line.startWidth = 0.3f;
            //line.endWidth = 0.3f;
        }
        else
        {   
            // Turn off the trail
            hasLockedTarget = false;
            line.positionCount = 0;
            //line.startWidth = 0f;
            //line.endWidth = 0f; 
        }
    }

    public bool TrailShowing()
    {   
        // Return trail status
        return showTrail; 
    }

    void Update()
    {
        if (showTrail)
        {
            HandleTrail(); 
        }
    }

    void HandleTrail()
    {
        if(lastTrailPoints == null || lastTrailPoints.Count == 0) return;

        puffTimer += Time.deltaTime;
        if(puffTimer >= puffInterval)
        {
            puffTimer = 0f;
            
            for(int i = 0; i < puffsPerInterval; i++)
            {
                int idx = UnityEngine.Random.Range(0, lastTrailPoints.Count);
                Vector3 basePos = lastTrailPoints[idx];

                Vector2 offset2D = UnityEngine.Random.insideUnitCircle*tubeRadius;
                Vector3 spawnPos = basePos + new Vector3(offset2D.x, 0f, offset2D.y);

                spawnPos += Vector3.up*0.05f;

                Debug.Log("puffing");
                GameObject puff = Instantiate(pheromoneVFXPrefab, spawnPos, Quaternion.identity, home);
            }
        }
    }

    Vector3 GetClosestFood()
    {
        //Debug.Log("Calculating next closest food.");
        //1. Get home position
        // already exists as home.  
        
        //2. Find all objects tagged food.
        //and 3. Loop through food.
        float smallest = Mathf.Infinity;
        FoodSource smallestFood = null;   

        foreach (GameObject f in GameObject.FindGameObjectsWithTag("FoodSource"))
        {   
            FoodSource food = f.GetComponent<FoodSource>();
            if (food == null)
            {   
                // We are only worried about base food items with scripts attached. 
                continue;
            }
            //Debug.Log("Found a valid food item " + f.name); 
            float d = food.DistanceToTarget(home); // Squared Distance

            if (d < smallest)
            {
                smallest = d;
                smallestFood = food; 
            }
           if(food.DistanceToTarget(home) < smallest)
           {    
                smallest = food.DistanceToTarget(home);
                smallestFood = food; 
           }
        }

        // If no food found return a flag value
        if (smallestFood == null)
        {
            return Vector3.zero;
        }
        // else 
        return smallestFood.foodLocation().position;
    }
    void UpdatePath()
    {   
        // if trail is off or no target locked, do nothing
        if (home == null || !showTrail || !hasLockedTarget)
        {   
            line.positionCount = 0;
            return;
        }

        Vector3 targetPos = lockedTargetPos;

        if (!NavMesh.CalculatePath(home.position, targetPos, NavMesh.AllAreas, path))
        {   
            line.positionCount = 0;
            return;
        }
        // If only 2 corners, still generate smooth and terrain-adjusted points.
        List<Vector3> finalPoints = new List<Vector3>();

        float segmentStep = 0.3f;     // smaller = smoother line
        float hoverHeight = 1f;    // how high above ground the line floats
        LayerMask groundMask = LayerMask.GetMask("Default"); // Adjust to your terrain layer(s)

        Vector3[] corners = path.corners;

        for (int i = 0; i < corners.Length - 1; i++)
        {
            Vector3 a = corners[i];
            Vector3 b = corners[i + 1];

            float dist = Vector3.Distance(a, b);
            int steps = Mathf.CeilToInt(dist / segmentStep);

            for (int s = 0; s <= steps; s++)
            {
                float t = (float)s / steps;
                Vector3 pos = Vector3.Lerp(a, b, t);

                // Drop to terrain
                Vector3 rayStart = pos + Vector3.up * 5f;

                if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 20f, groundMask))
                {
                    pos = hit.point + Vector3.up * hoverHeight;
                }

                finalPoints.Add(pos);
            }
        }

        line.positionCount = finalPoints.Count;
        line.SetPositions(finalPoints.ToArray());

        //store trail for particle spawning
        lastTrailPoints = finalPoints;
    }

    void SetupGradient()
    {
        Gradient g = new Gradient();

        g.SetKeys(
            new GradientColorKey[]
            {
            new GradientColorKey(new Color(0.7f, 0.9f, 0.85f), 0f),
            new GradientColorKey(new Color(0.6f, 0.8f, 0.8f), 1f)
            },
            new GradientAlphaKey[]
            {
            new GradientAlphaKey(0.0f, 0f),
            new GradientAlphaKey(0.6f, 0.3f),
            new GradientAlphaKey(0.6f, 0.7f),
            new GradientAlphaKey(0.0f, 1f)
            }
        );

        line.colorGradient = g;
    }
}
