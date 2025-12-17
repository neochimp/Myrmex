using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    public NavMeshSurface navMeshSurface;
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;
    private bool paused = false;
    //vertex count = (xSize + 1) * (zSize + 1)
    public int xSize = 200;
    public int zSize = 200;

    public LayerMask groundMask;
    //Perlin noise testing variables
    [Header("Base Hills")]
    public float baseScale = 0.05f;
    public float baseHeight = 8f;

    [Header("Medium Detail")]
    public float midScale = 0.1f;
    public float midHeight = 3f;

    [Header("Fine Detail")]
    public float detailScale = 0.3f;
    public float detailHeight = 1f;

    [Header("Other Perlin Stuff")]
    public Vector2 noiseOffset; // for moving terrain around / randomizing
    public float scrollSpeed = 0f;
    public float clusterScale = 0.02f; // very low frequency
    public float clusterThreshold = 0.4f;


    [Header("Grass Objects")]
    public GameObject[] grass;

    [Header("Pause UI")]
    public GameObject pauseUI;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        uvs = new Vector2[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                vertices[i] = new Vector3(x, GetHeight(x, z), z);
                uvs[i] = new Vector2((float)x / xSize, (float)z / zSize);
                i++;
            }
        }
        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;

                //yield return new WaitForSeconds(0.01f);
            }
            vert++;
        }

        UpdateMesh();

        MeshCollider mc = GetComponent<MeshCollider>();
        mc.sharedMesh = mesh;

        if (navMeshSurface == null)
        {
            navMeshSurface = gameObject.AddComponent<NavMeshSurface>();
        }

        //grass spawning stuff
        //this gives the vertex at the center of a 4x4 square. This is the point we will use to find the perlin noise for density.
        for (int z = 2; z <= zSize; z += 4)
        {
            for (int x = 2; x <= xSize; x += 4)
            {
                int index = z * (xSize + 1) + x;

                float grassThreshold = 0.4f;
                float spawnDensity = Mathf.PerlinNoise(x * 0.05f, z * 0.05f);
                spawnDensity = Mathf.Clamp01((spawnDensity - grassThreshold) / (1f - grassThreshold));
                int spawnCount = (int)(spawnDensity * 60 * DistanceFromCenterNormalizedWithEdgeBias(new Vector2(x, z)));
                int borderThickness = 5;
                bool isBorder =
                  x <= borderThickness ||
                  z <= borderThickness ||
                  x >= xSize - borderThickness ||
                  z >= zSize - borderThickness;

                if (isBorder)
                {
                    spawnCount = 40;
                }

                for (int i = 0; i < spawnCount; i++)
                {
                    SpawnTerrainObject(grass[Random.Range(0, 2)], vertices[index], 0.5f);
                }
            }
        }
        navMeshSurface.collectObjects = CollectObjects.Children;
        navMeshSurface.BuildNavMesh();
        var agents = FindObjectsOfType<NavMeshAgent>();
        foreach (var agent in agents)
        {
            NavMeshHit hit;
            // Look for nearest navmesh point within a radius
            if (NavMesh.SamplePosition(agent.transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position); // teleport agent onto the navmesh
                agent.enabled = true;
            }
        }

        GameObject player = GameObject.FindWithTag("Player");
        player.transform.position = new Vector3(player.transform.position.x, GetHeight(player.transform.position.x, player.transform.position.z), player.transform.position.z);

        //correcting all food to be on terrain and not colliding with grass
        FixOverlaps();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            paused = !paused;
            pauseUI.SetActive(paused);
            Debug.Log("Pause Toggled");
        }


        FoodDropsFixY();

        Time.timeScale = paused ? 0 : 1;

        /* commenting out constant update so my computer doesnt explode with big map
                for (int i = 0, z = 0; z <= zSize; z++)
                {
                    for (int x = 0; x <= xSize; x++)
                    {
                        vertices[i].y = GetHeight(x, z);
                        i++;
                    }
                }
                noiseOffset.x += scrollSpeed * Time.deltaTime;
                UpdateMesh();
          */
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        MeshCollider mc = GetComponent<MeshCollider>();
        mc.sharedMesh = null;
        mc.sharedMesh = mesh;
    }
    private void OnDrawGizmos()
    {

        if (vertices == null)
        {
            return;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }

    float GetHeight(float x, float z)
    {
        float nx = x + noiseOffset.x;
        float nz = z + noiseOffset.y;

        // Cluster mask
        float cluster = Mathf.PerlinNoise(nx * clusterScale, nz * clusterScale);
        // Optional: sharpen the mask a bit so clusters are more defined
        cluster = Mathf.Clamp01((cluster - clusterThreshold) / (1f - clusterThreshold));
        // Now cluster is ~0 in flat regions, >0 where hills should appear

        float big = Mathf.PerlinNoise(nx * baseScale, nz * baseScale) * baseHeight * cluster;
        float mid = Mathf.PerlinNoise(nx * midScale, nz * midScale) * midHeight * cluster;
        float small = Mathf.PerlinNoise(nx * detailScale, nz * detailScale) * detailHeight * cluster;

        return big + mid + small;
    }

    void SpawnTerrainObject(GameObject obj, Vector3 pos, float scale)
    {
        Vector3 randPos = new Vector3(pos.x - 2f + Random.Range(0.0f, 4.0f), pos.y - 0.5f, pos.z - 2f + Random.Range(0.0f, 4.0f));
        GameObject grassBlade = Instantiate(obj, randPos, Quaternion.Euler(-90, 0, Random.Range(0, 360)));
        grassBlade.transform.parent = transform;
        grassBlade.transform.localScale = new Vector3(scale, scale, scale);
    }

    float DistanceFromCenterNormalizedWithEdgeBias(Vector2 pos)
    {
        Vector2 center = new Vector2(xSize / 2f, zSize / 2f);
        float rampValue = 1.5f;
        return Mathf.Pow((Vector2.Distance(pos, center) / (xSize / 2f)), rampValue);
    }

    void FixOverlaps()
    {
        GameObject[] foods = GameObject.FindGameObjectsWithTag("FoodShell");
        foreach (var food in foods)
        {
            food.transform.position = SnapToTerrain(food.transform.position);
            int attempts = 0;
            while (IsOverlappingGrass(food) && attempts < 100 /*max attempts*/)
            {
                Vector2 offset2D = Random.insideUnitCircle.normalized * 2f;
                Vector3 newPos = food.transform.position;
                newPos.x += offset2D.x;
                newPos.z += offset2D.y;

                newPos = SnapToTerrain(newPos);
                food.transform.position = newPos;

                attempts++;
            }
        }
    }

    bool IsOverlappingGrass(GameObject food)
    {
        Collider[] hits = Physics.OverlapSphere(food.transform.position, 1f /*Radius to check*/);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Grass") || hit.CompareTag("FoodShell")) return true;
        }
        return false;
    }

    Vector3 SnapToTerrain(Vector3 pos)
    {
        return new Vector3(pos.x, GetHeight(pos.x, pos.z), pos.z);
    }
    void FoodDropsFixY()
    {
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
        foreach (var food in foods)
        {
            food.transform.position = SnapToTerrain(food.transform.position);
        }

    }


}

