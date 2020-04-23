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
	private int powerOfTwoOfWorldSizeInChunks;

	[SerializeField]
	private float worldScale;

	[SerializeField]
	private int riverDepth;

	[SerializeField]
	private int riverPart;

	[SerializeField]
	private BiomeAttributes[] biomeAttributes;

	public int ChunkWidth { get => chunkWidth; }
	public int ChunkHeight { get => chunkHeight; }
	public int WorldSizeInChunks { get => 1 << powerOfTwoOfWorldSizeInChunks; }	
	public int WorldSizeInBlocks { get => WorldSizeInChunks * chunkWidth; }
	public float WorldScale { get => worldScale; }
	public int RiverDepth { get => riverDepth; }
	public int RiverPart { get => riverPart; }
	public BiomeAttributes[] BiomeAttributes { get => biomeAttributes; set => biomeAttributes = value; }
	
}
