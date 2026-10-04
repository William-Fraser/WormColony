using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MarchingCubes : MonoBehaviour
{
    public int xChunkDistance;
    public int zChunkDistance;

    [SerializeField] private int genWidth = 50;
    [SerializeField] private int genHeight = 25;

    [SerializeField] private int rendWidth = 25;
    [SerializeField] private int rendHeight = 25;

    [SerializeField] float noiseScale = 1;

    [SerializeField] private float heightTresshold = 0.5f;

    bool use3DNoise = true;

    [SerializeField] bool useRandomSeed;
    [SerializeField] string seed; // when outputting seed for interface use GetHashCode()
    float plant;
    [Space(30)]
    [SerializeField] bool regenerate;
    [SerializeField] bool updateOn;
    [Space(30)]
    [SerializeField] bool visualizeNoise;
    [SerializeField] float resolution = 1;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private float[,,] heights;

    private MeshFilter meshFilter;

    void Start()
    {
        if (useRandomSeed)
            seed = Time.time.ToString();

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());
        plant = pseudoRandom.Next(0, 100);

        meshFilter = GetComponent<MeshFilter>();
        SetHeights();
        MarchCubes();
        SetMesh();
    }

    void Update()
    {
        if (regenerate || updateOn)
        { 
            SetHeights();
            MarchCubes();
            SetMesh();
            regenerate = false;
        }
    }

    private void SetMesh()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    private void SetHeights()
    {
        heights = new float[genWidth + 1, genHeight + 1, genWidth + 1];

        for (int x = 0; x < genWidth + 1; x++)
        {
            for (int y = 0; y < genHeight + 1; y++)
            {
                for (int z = 0; z < genWidth + 1; z++)
                {
                    if (use3DNoise)
                    {
                        float currentHeight = PerlinNoise3D((float)x / genWidth * noiseScale, (float)y / genHeight * noiseScale, (float)z / genWidth * noiseScale);

                        heights[x, y, z] = currentHeight;
                    }
                    else
                    {
                        float currentHeight = genHeight * Mathf.PerlinNoise(x * noiseScale, z * noiseScale);
                        float distToSufrace;

                        if (y <= currentHeight - 0.5f)
                            distToSufrace = 0f;
                        else if (y > currentHeight + 0.5f)
                            distToSufrace = 1f;
                        else if (y > currentHeight)
                            distToSufrace = y - currentHeight;
                        else
                            distToSufrace = currentHeight - y;

                        heights[x, y, z] = distToSufrace;
                    }
                }
            }
        }
    }

    private float PerlinNoise3D(float x, float y, float z)
    {
        x = x + xChunkDistance;
        z = z + zChunkDistance;
        Debug.Log($" {x }, {xChunkDistance}. {z}, {zChunkDistance}");

        // do perlin noise
        float xy = Mathf.PerlinNoise(x + plant, y + plant);
        float xz = Mathf.PerlinNoise(x + plant, z + plant);
        float yz = Mathf.PerlinNoise(y + plant, z + plant);

        float yx = Mathf.PerlinNoise(y + plant, x + plant);
        float zx = Mathf.PerlinNoise(z + plant, x + plant);
        float zy = Mathf.PerlinNoise(z + plant, y + plant);

        return (xy + xz + yz + yx + zx + zy) / 6;
    }

    private int GetConfigIndex(float[] cubeCorners)
    {
        int configIndex = 0;

        for (int i = 0; i < 8; i++)
        {
            if (cubeCorners[i] > heightTresshold)
            {
                configIndex |= 1 << i;
            }
        }

        return configIndex;
    }

    private void MarchCubes()
    {
        vertices.Clear();
        triangles.Clear();

        for (int x = 0; x < rendWidth; x++)
        {
            for (int y = 0; y < rendHeight; y++)
            {
                for (int z = 0; z < rendWidth; z++)
                {
                    float[] cubeCorners = new float[8];

                    for (int i = 0; i < 8; i++)
                    {
                        Vector3Int corner = new Vector3Int(x, y, z) + MarchingTable.Corners[i];
                        cubeCorners[i] = heights[corner.x, corner.y, corner.z];
                    }

                    MarchCube(new Vector3(x, y, z), cubeCorners);
                }
            }
        }
    }

    private void MarchCube(Vector3 position, float[] cubeCorners)
    {
        int configIndex = GetConfigIndex(cubeCorners);

        if (configIndex == 0 || configIndex == 255)
        {
            return;
        }

        int edgeIndex = 0;
        for (int t = 0; t < 5; t++)
        {
            for (int v = 0; v < 3; v++)
            {
                int triTableValue = MarchingTable.Triangles[configIndex, edgeIndex];

                if (triTableValue == -1)
                {
                    return;
                }

                Vector3 edgeStart = position + MarchingTable.Edges[triTableValue, 0];
                Vector3 edgeEnd = position + MarchingTable.Edges[triTableValue, 1];

                Vector3 vertex = (edgeStart + edgeEnd) / 2;

                vertices.Add(vertex);
                triangles.Add(vertices.Count - 1);

                edgeIndex++;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!visualizeNoise || !Application.isPlaying)
        {
            return;
        }

        for (int x = 0; x < genWidth + 1; x++)
        {
            for (int y = 0; y < genHeight + 1; y++)
            {
                for (int z = 0; z < genWidth + 1; z++)
                {
                    Gizmos.color = new Color(heights[x, y, z], heights[x, y, z], heights[x, y, z], 1);
                    Gizmos.DrawSphere(new Vector3(x * resolution, y * resolution, z * resolution), 0.2f * resolution);
                }
            }
        }
    }
}