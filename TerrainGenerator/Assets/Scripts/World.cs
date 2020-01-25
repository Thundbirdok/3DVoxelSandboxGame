using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{

	[SerializeField]
	private WorldAttributes worldAttributes;
	[SerializeField]
	private BlocksAttributes blocksAttributes;    

	Chunk[,] chunks;

	private void Start()
	{

		chunks = new Chunk[worldAttributes.WorldSizeInChunks, worldAttributes.WorldSizeInChunks];

		GenerateWorld(); 

	}

	private void Update()
	{



	}

	private void GenerateWorld()
	{

		for (int x = 0; x < worldAttributes.WorldSizeInChunks; ++x)
		{

			for (int z = 0; z < worldAttributes.WorldSizeInChunks; ++z)
			{

				CreateChunk(new ChunkCoord(x, z));

			}

		}

		for (int x = 0; x < worldAttributes.WorldSizeInChunks; ++x)
		{

			for (int z = 0; z < worldAttributes.WorldSizeInChunks; ++z)
			{

				GenerateChunk(new ChunkCoord(x, z));

			}

		}

	}

	void GenerateChunk(ChunkCoord coord)
	{

		for (byte y = 0; y < worldAttributes.ChunkHeight; ++y)
		{

			for (int x = (y * worldAttributes.ChunkWidth) / worldAttributes.ChunkHeight; x < worldAttributes.ChunkWidth; ++x)
			{

				for (int z = (y * worldAttributes.ChunkWidth) / worldAttributes.ChunkHeight; z < worldAttributes.ChunkWidth; ++z)
				{

					if (y == worldAttributes.ChunkHeight - 1)
					{

						chunks[coord.x, coord.z].voxelMap[x, y, z] = 3;

					}
					else if (y == 0)
					{

						chunks[coord.x, coord.z].voxelMap[x, 0, z] = 1;

					}
					else
					{

						chunks[coord.x, coord.z].voxelMap[x, y, z] = (byte)(2 + y % 11);

					}

				}

			}

		}

		chunks[coord.x, coord.z].Update();

	}

	private void CreateChunk(ChunkCoord coord)
	{

		chunks[coord.x, coord.z] = new Chunk(new ChunkCoord(coord.x, coord.z), this, worldAttributes, blocksAttributes);

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
