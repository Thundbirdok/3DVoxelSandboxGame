using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldAttributes", menuName = "TerrainGenerator/World Attributes")]
public class WorldAttributes : ScriptableObject
{
    
    [SerializeField]
    private int chunkWidth;
    [SerializeField]
    private int chunkHeight;

    [SerializeField]
    private int worldSizeInChunks;   

    public int ChunkWidth { get => chunkWidth; }
    public int ChunkHeight { get => chunkHeight; }
    public int WorldSizeInChunks { get => worldSizeInChunks; }
    
    [SerializeField]
    public int WorldSizeInBlocks { get => worldSizeInChunks * chunkWidth; }
}
