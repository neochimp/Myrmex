using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class PheromoneTrail : MonoBehaviour
{
    public Transform home;
    public Transform targetItem;
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

    private List<Vector3> lastTrailPoints = new List<Vector3>();
    // Start is called before the first frame update
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
        mat.color = new Color(0.5f, 1f, 0.8f, 0.7f); // minty glow with transparency

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

    void Update()
    {
        if(lastTrailPoints == null || lastTrailPoints.Count == 0) return;

        puffTimer += Time.deltaTime;
        if(puffTimer >= puffInterval)
        {
            puffTimer = 0f;
            
            for(int i = 0; i < puffsPerInterval; i++)
            {
                int idx = Random.Range(0, lastTrailPoints.Count);
                Vector3 basePos = lastTrailPoints[idx];

                Vector2 offset2D = Random.insideUnitCircle*tubeRadius;
                Vector3 spawnPos = basePos + new Vector3(offset2D.x, 0f, offset2D.y);

                spawnPos += Vector3.up*0.05f;

                GameObject puff = Instantiate(pheromoneVFXPrefab, spawnPos, Quaternion.identity, home);
            }
        }
    }

    void UpdatePath()
    {
        if (home == null || targetItem == null)
        {
            line.positionCount = 0;
            return;
        }

        if (!NavMesh.CalculatePath(home.position, targetItem.position, NavMesh.AllAreas, path))
        {
            line.positionCount = 0;
            return;
        }

        // If only 2 corners, still generate smooth and terrain-adjusted points.
        List<Vector3> finalPoints = new List<Vector3>();

        float segmentStep = 0.3f;     // smaller = smoother line
        float hoverHeight = 1f;    // how high above ground the line floats
        LayerMask groundMask = LayerMask.GetMask("Ground"); // Adjust to your terrain layer(s)

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
