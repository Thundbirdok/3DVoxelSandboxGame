using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldAttributes", menuName = "TerrainGenerator/World Attributes")]
public class WorldAttributes : ScriptableObject
{
	
	[SerializeField]
	private int chunkWidth = 0;
	[SerializeField]
	private int chunkHeight = 0;

	[SerializeField]
	private int powerOfTwoOfWorldSizeInChunks = 0;

	[Header("Bioms")]
	[SerializeField]
	private int sitesNumber = 0;

	[SerializeField]
	private int sitesMinDistance = 0;

	[SerializeField]
	private int boarderBrushRadius = 0;

	[SerializeField]
	private float worldScale = 0;

	[SerializeField]
	private int checkBiomeDistance = 0;

	[SerializeField]
	private int smoothingBrushRadius = 0;

	[SerializeField]
	private int smoothingCheckBrushRadius = 0;

	[SerializeField]
	private int oceanHeight = 0;

	[Header("Rivers")]
	[SerializeField]
	private int riverDepth = 0;

	[SerializeField]
	private int riverBrushRadius = 0;

	[SerializeField]
	private float riverBrushScale = 0;

	[SerializeField]
	private BiomeAttributes[] biomeAttributes = null;

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
