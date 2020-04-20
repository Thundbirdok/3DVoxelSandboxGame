using System;
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

	public int[,] Bioms;

	private IWorldGenerator generator;

    private void Start()
	{

		Chunks = new Chunk[WorldAttributes.WorldSizeInChunks, WorldAttributes.WorldSizeInChunks];

		Bioms = new int[WorldAttributes.WorldSizeInBlocks, WorldAttributes.WorldSizeInBlocks];

		generator = new FullRandom();

		GenerateWorld(); 

	}

	private void Update()
	{		

	}
	
	public void GenerateWorldFullRandom()
	{

		generator = new FullRandom();

		GenerateWorld();

	}

	public void GenerateWorldRandomHills()
	{

		generator = new RandomHills();

		GenerateWorld();

	}

	public void GenerateWorldPerlinNoise()
	{

		generator = new PerlinNoise();

		GenerateWorld();

	}

	private void GenerateWorld()
	{

		generator.GenerateWorld(this);

	}

	public void CreateChunk(Vector2Int coord)
	{

		if (Chunks[coord.x, coord.y] == null) {

			Chunks[coord.x, coord.y] = new Chunk(new Vector2Int(coord.x, coord.y), this, WorldAttributes, BlocksAttributes);

		}
		else
		{

			Chunks[coord.x, coord.y].Clear();

		}

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
	
	public Vector2Int GetChunkCoord(Vector3 pos)
	{

		int x = Mathf.FloorToInt(pos.x / WorldAttributes.ChunkWidth);
		int z = Mathf.FloorToInt(pos.z / WorldAttributes.ChunkWidth);

		return new Vector2Int(x, z);

	}

	public Vector2Int GetChunkCoord(Vector2 pos)
	{

		int _x = Mathf.FloorToInt(pos.x / WorldAttributes.ChunkWidth);
		int _z = Mathf.FloorToInt(pos.y / WorldAttributes.ChunkWidth);

		return new Vector2Int(_x, _z);

	}

	public Vector2Int GetInChunkCoord(Vector3 pos)
	{

		int x = Mathf.FloorToInt(pos.x % WorldAttributes.ChunkWidth);
		int z = Mathf.FloorToInt(pos.z % WorldAttributes.ChunkWidth);

		return new Vector2Int(x, z);

	}

	public Vector2Int GetInChunkCoord(Vector2 pos)
	{

		int _x = Mathf.FloorToInt(pos.x % WorldAttributes.ChunkWidth);
		int _z = Mathf.FloorToInt(pos.y % WorldAttributes.ChunkWidth);

		return new Vector2Int(_x, _z);

	}

	internal void UpdateChunks()
	{

		for (int x = 0; x < WorldAttributes.WorldSizeInChunks; ++x)
		{

			for (int z = 0; z < WorldAttributes.WorldSizeInChunks; ++z)
			{

				Chunks[x, z].Update();

			}

		}

	}

}
