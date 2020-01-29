using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{

	[SerializeField]
	private WorldAttributes worldAttributes;
	[SerializeField]
	private BlocksAttributes blocksAttributes;

	public WorldAttributes WorldAttributes { get => worldAttributes; }
	public BlocksAttributes BlocksAttributes { get => blocksAttributes; }
    public Chunk[,] Chunks { get; private set; }

	IWorldGenerator generator;

    private void Start()
	{

		Chunks = new Chunk[WorldAttributes.WorldSizeInChunks, WorldAttributes.WorldSizeInChunks];

		generator = new FullRandom();

		GenerateWorld(); 

	}

	private void Update()
	{



	}

	private void GenerateWorld()
	{

		generator.GenerateWorld(this);

	}	

	public void CreateChunk(ChunkCoord coord)
	{

		Chunks[coord.x, coord.z] = new Chunk(new ChunkCoord(coord.x, coord.z), this, WorldAttributes, BlocksAttributes);

	}

	public bool IsVoxelInWorld(int x, int y, int z)
	{

		if (x < WorldAttributes.WorldSizeInBlocks && z < WorldAttributes.WorldSizeInBlocks && y < WorldAttributes.ChunkHeight)
		{

			return true;

		}

		return false;

	}

	public bool IsVoxelInWorld(int x, int z)
	{

		if (x < WorldAttributes.WorldSizeInBlocks && z < WorldAttributes.WorldSizeInBlocks)
		{

			return true;

		}

		return false;

	}	

}

public class ChunkCoord
{

	public int x;
	public int z;

	public ChunkCoord(int _x, int _z)
	{

		x = _x;
		z = _z;

	}

	public bool Equals(ChunkCoord other)
	{

		if (other == null)
			return false;
		else if (other.x == x && other.z == z)
			return true;
		else
			return false;

	}

}
