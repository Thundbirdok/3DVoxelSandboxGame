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

    private void Start()
	{

		Chunks = new Chunk[WorldAttributes.WorldSizeInChunks, WorldAttributes.WorldSizeInChunks];

		GenerateWorld(); 

	}

	private void Update()
	{



	}

	private void GenerateWorld()
	{

		for (int x = 0; x < WorldAttributes.WorldSizeInChunks; ++x)
		{

			for (int z = 0; z < WorldAttributes.WorldSizeInChunks; ++z)
			{

				CreateChunk(new ChunkCoord(x, z));

			}

		}

		for (int x = 0; x < WorldAttributes.WorldSizeInChunks; ++x)
		{

			for (int z = 0; z < WorldAttributes.WorldSizeInChunks; ++z)
			{

				GenerateChunk(new ChunkCoord(x, z));

			}

		}

	}

	void GenerateChunk(ChunkCoord coord)
	{

		for (byte y = 0; y < WorldAttributes.ChunkHeight; ++y)
		{

			for (int x = (y * WorldAttributes.ChunkWidth) / WorldAttributes.ChunkHeight; x < WorldAttributes.ChunkWidth; ++x)
			{

				for (int z = (y * WorldAttributes.ChunkWidth) / WorldAttributes.ChunkHeight; z < WorldAttributes.ChunkWidth; ++z)
				{

					if (y == WorldAttributes.ChunkHeight - 1)
					{

						Chunks[coord.x, coord.z].voxelMap[x, y, z] = 3;

					}
					else if (y == 0)
					{

						Chunks[coord.x, coord.z].voxelMap[x, 0, z] = 1;

					}
					else
					{

						Chunks[coord.x, coord.z].voxelMap[x, y, z] = (byte)(2 + y % 11);

					}

				}

			}

		}

		Chunks[coord.x, coord.z].Update();

	}

	private void CreateChunk(ChunkCoord coord)
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
