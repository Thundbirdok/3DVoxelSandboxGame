using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldAttributes", menuName = "TerrainGenerator/World Attribute")]
public class WorldAttributes : ScriptableObject
{
    
    [SerializeField]
    private int chunkWidth;
    [SerializeField]
    private int chunkHeight;

    public int ChunkWidth { get => chunkWidth; }
    public int ChunkHeight { get => chunkHeight; }

}
