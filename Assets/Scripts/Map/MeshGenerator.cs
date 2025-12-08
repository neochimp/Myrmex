using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    private bool paused = false;
    //vertex count = (xSize + 1) * (zSize + 1)
    public int xSize = 100;
    public int zSize = 100;

    //Perlin noise testing variables
    public float xPerlin = 0.3f;
    public float zPerlin = 0.3f;
    public float scalePerlin = 2f;

    [Header("Base Hills")]
    public float baseScale = 0.05f;
    public float baseHeight = 8f;

    [Header("Medium Detail")]
    public float midScale = 0.1f;
    public float midHeight = 3f;

    [Header("Fine Detail")]
    public float detailScale = 0.3f;
    public float detailHeight = 1f;

    public Vector2 noiseOffset; // for moving terrain around / randomizing
    public float scrollSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                vertices[i] = new Vector3(x, GetHeight(x, z), z);
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
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("escape"))
        {
            paused = !paused;
            Debug.Log("Pause Toggled");
        }

        Time.timeScale = paused ? 0 : 1;


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
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
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

    public float clusterScale = 0.02f; // very low frequency
    public float clusterThreshold = 0.4f;

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


}

