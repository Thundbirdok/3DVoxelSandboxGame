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
	private int sitesNumber;

	[SerializeField]
	private int sitesMinDistance;

	[SerializeField]
	private int boarderBrushRadius;

	[SerializeField]
	private float worldScale;

	[SerializeField]
	private int checkBiomeDistance;

	[SerializeField]
	private int smoothingBrushRadius;

	[SerializeField]
	private int smoothingCheckBrushRadius;

	[SerializeField]
	private int oceanHeight;

	[SerializeField]
	private int riverDepth;


	[SerializeField]
	private int riverBrushRadius;

	[SerializeField]
	private float riverBrushScale;

	[SerializeField]
	private BiomeAttributes[] biomeAttributes;

	public int ChunkWidth { get => chunkWidth; }
	public int ChunkHeight { get => chunkHeight; }
	public int WorldSizeInChunks { get => 1 << powerOfTwoOfWorldSizeInChunks; }	
	public int WorldSizeInBlocks { get => WorldSizeInChunks * chunkWidth; }
	public int SitesNumber { get => sitesNumber; }
	public int SitesMinDistance { get => sitesMinDistance; }
	public int BoarderBrushRadius { get => boarderBrushRadius; }
	public float WorldScale { get => worldScale; }
	public int CheckBiomeDistance { get => checkBiomeDistance; }
	public int SmoothingBrushRadius { get => smoothingBrushRadius; }
	public int SmoothingCheckBrushRadius { get => smoothingCheckBrushRadius; }
	public int OceanHeight { get => oceanHeight; }
	public int RiverDepth { get => riverDepth; }	
	public int RiverBrushRadius { get => riverBrushRadius; }
	public float RiverBrushScale { get => riverBrushScale; }
	public BiomeAttributes[] BiomeAttributes { get => biomeAttributes; }
	
}
