using UnityEngine;
using System;

public class MapGenerator : MonoBehaviour
{
    public int width;
    public int length;

    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int randomFillPercent;

    public int smoothingFactor = 5;

    int[,] map;

    void Start()
    {
        GenerateMap();
    }

    void Update()
    {
        
    }

    private void GenerateMap()
    { 
        map = new int[width, length];
        RandomFillMap();    

        for(int i = 0; i < smoothingFactor; i++)
            SmoothMap();

        CaveMeshGenerator meshGen = GetComponent<CaveMeshGenerator>();
        meshGen.GenerateMesh(map, 1);
    }

    private void RandomFillMap()
    {
        if (useRandomSeed)
            seed = Time.time.ToString();

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                if (x == 0 || x == width - 1 || z == 0 || z == length - 1)
/*//change to invincible wall value*/ map[x, z] = 1;
                else
                map[x, z] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
            }
        }
    }

    private void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, z);

                if (neighbourWallTiles > 4)
                    map[x, z] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, z] = 0;
            }
        }
    }

    private int GetSurroundingWallCount(int gridX, int gridZ)
    {
        int wallCount = 0;

        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourZ = gridZ - 1; neighbourZ <= gridZ + 1; neighbourZ++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourZ >= 0 && neighbourZ < length)
                {
                    if (neighbourX != gridX || neighbourZ != gridZ)
                    {
                        wallCount += map[neighbourX, neighbourZ];
                        //count invincible walls as 1
                    }
                }
                else wallCount++;
            }
        }

        return wallCount;
    }

   /* void OnDrawGizmos()
    {
        if (map != null)
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < length; z++)
                {
                    Gizmos.color = (map[x, z] == 1) ? Color.black : Color.white;
                    Vector3 pos = new Vector3(-width / 2 + x + .5f, 0, -length / 2 + z + .5f);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
    }*/
}