using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{

	private WorldAttributes worldAttributes;
	private BlocksAttributes blocksAttributes;

	private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;

	public ChunkCoord coord;
	GameObject chunkObject;

	private int vertexIndex = 0;
	private List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();
	private List<Vector2> uvs = new List<Vector2>();

	public byte[,,] voxelMap;

	public Chunk(ChunkCoord _coord, World _world, WorldAttributes _worldAttributes, BlocksAttributes _blocksAttributes)
	{

		worldAttributes = _worldAttributes;
		blocksAttributes = _blocksAttributes;

		coord = _coord;
		chunkObject = new GameObject();
		chunkObject.transform.position = new Vector3(coord.x * worldAttributes.ChunkWidth, 0f, coord.z * worldAttributes.ChunkWidth);

		voxelMap = new byte[worldAttributes.ChunkWidth, worldAttributes.ChunkHeight, worldAttributes.ChunkWidth];

		meshRenderer = chunkObject.AddComponent<MeshRenderer>();
		meshFilter = chunkObject.AddComponent<MeshFilter>();        

		chunkObject.transform.SetParent(_world.transform);
		meshRenderer.material = blocksAttributes.Material;

		chunkObject.name = coord.x + ", " + coord.z;

	}
	
	public void Update ()
	{
		CreateMeshData();
		CreateMesh();
	}

	private void CreateMeshData()
	{

		for (int y = 0; y < worldAttributes.ChunkHeight; ++y)
		{

			for (int x = 0; x < worldAttributes.ChunkWidth; ++x)
			{

				for (int z = 0; z < worldAttributes.ChunkWidth; ++z)
				{

					AddVoxelDataToChunk(new Vector3(x, y, z));

				}

			}

		}

	}

	private bool CheckVoxel(Vector3 pos)
	{

		int x = Mathf.FloorToInt(pos.x);
		int y = Mathf.FloorToInt(pos.y);
		int z = Mathf.FloorToInt(pos.z);

		if (x < 0 || x >= worldAttributes.ChunkWidth || y < 0 || y >= worldAttributes.ChunkHeight || z < 0 || z >= worldAttributes.ChunkWidth)
		{

			return false;

		}

		return blocksAttributes.Blocktypes[voxelMap[x, y, z]].isSolid;

	}

	private void AddVoxelDataToChunk(Vector3 pos)
	{

		if (CheckVoxel(pos))
		{

			for (int p = 0; p < 6; p++)
			{

				if (!CheckVoxel(pos + VoxelData.faceChecks[p]))
				{

					byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];

					vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
					vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
					vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
					vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

					AddTexture(blocksAttributes.Blocktypes[blockID].GetTextureID(p));

					triangles.Add(vertexIndex);
					triangles.Add(vertexIndex + 1);
					triangles.Add(vertexIndex + 2);
					triangles.Add(vertexIndex + 2);
					triangles.Add(vertexIndex + 1);
					triangles.Add(vertexIndex + 3);
					vertexIndex += 4;

				}

			}

		}

	}

	private void CreateMesh()
	{

		Mesh mesh = new Mesh
		{
			vertices = vertices.ToArray(),
			triangles = triangles.ToArray(),
			uv = uvs.ToArray()
		};

		mesh.RecalculateNormals();

		meshFilter.mesh = mesh;

	}

	private void AddTexture(int textureID)
	{

		float y = textureID / blocksAttributes.TextureAtlasSizeInBlocks;
		float x = textureID - (y * blocksAttributes.TextureAtlasSizeInBlocks);

		x *= blocksAttributes.NormalizedBlockTextureSize;
		y *= blocksAttributes.NormalizedBlockTextureSize;

		y = 1f - y - blocksAttributes.NormalizedBlockTextureSize;

		uvs.Add(new Vector2(x, y));
		uvs.Add(new Vector2(x, y + blocksAttributes.NormalizedBlockTextureSize));
		uvs.Add(new Vector2(x + blocksAttributes.NormalizedBlockTextureSize, y));
		uvs.Add(new Vector2(x + blocksAttributes.NormalizedBlockTextureSize, y + blocksAttributes.NormalizedBlockTextureSize));

	}

}
