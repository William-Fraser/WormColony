using UnityEngine;
using System.Collections.Generic;

public class ChunkManager : MonoBehaviour
{
    // in relation to player camera generate chunks out from "player chunk"
    // for testing load around a chunk called target chunk
    // dictionary of chunks with coord key?
    class Coord
    {
        public int x;
        public int z;

        public Coord(int x, int y)
        { this.x = x; this.z = y; }
    }

    class Chunk
    {
        GameObject parent;
        LandMeshGenerator landTop;
        MarchingCubes caves;
        LandMeshGenerator landBottom;
        public Coord coord;

        public Chunk(GameObject chunkInstance, Coord coordinate) // maybe plug seed here
        {
            coord = coordinate;
            parent = chunkInstance;
            parent.transform.position = new Vector3 (coord.x, 0, coord.z);
            landTop = parent.GetComponentInChildren<LandMeshGenerator>();
            landTop.xChunkDistance = coord.x;
            landTop.zChunkDistance = coord.z;
            landTop.gameObject.name = "landTop";
            caves = parent.GetComponentInChildren<MarchingCubes>();
            caves.xChunkDistance = coord.x;
            caves.zChunkDistance = coord.z;
            landBottom = parent.GetComponentsInChildren<LandMeshGenerator>()[1];
            landBottom.xChunkDistance = coord.x;
            landBottom.zChunkDistance = coord.z;
            landBottom.gameObject.name = "landBottom";
        }
    }

    public GameObject chunkPrefab;
    public int renderDistance; // add to settings
    private int chunkSize = 25;
    private Dictionary<Coord, Chunk> Chunks;
    private Chunk TargetChunk; // could be target coord that updates with player?

    private void Start()
    {
        PreloadRadius();
    }

    // loading chunks
    // save to a list for removal
    // render zone is accessable in settings
    // chunk size is 25
    private void Create(Coord coordinate)
    {
        GameObject instancedPrefab = GameObject.Instantiate(chunkPrefab);
        Chunk newChunk = new Chunk(instancedPrefab, coordinate);
    }

    //slow load but not rendered
    //maybe slow render at higher render levels
    //to increase performance, create an update version of preload chunks that works similarly to slow unloading chunks
    //half load chunks before rendering, maybe rename preload chunks to LoadRenderRadius or something
    //to fix the slow loading create a edge load barrier that resets the preloaded 'radius', so the player doesn't fall through the world
    private void PreloadRadius()
    {
        int rd = renderDistance; // shorthand
        //load chunks the amount of render distance from start at 0, 0
        for (int x = 0 - rd; x <= rd; x++)
        {
            for (int y = 0 - rd; y <= rd; y++)
            {
                Coord chunkPos = new Coord(x*chunkSize, y*chunkSize);
                Create(chunkPos);
            }
        }
    }

    private void LoadingEdge()
    { 
        
    }

    //saving edited chunks
    //before unloading chunks check to see if any changes occured in the chunk and save it to the world 
    //with the other techniques 


    // removing chunks
    // render zone follows target chunk
    // slowly unload chunks
    private void RemovingEdge()
    { 
        
    }
}
