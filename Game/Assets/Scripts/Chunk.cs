using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{

	private readonly WorldAttributes worldAttributes;
	private readonly BlocksAttributes blocksAttributes;

	private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;

	public Vector2Int coord;
	private GameObject chunkObject;

	private int vertexIndex = 0;
	private List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();
	private List<Vector2> uvs = new List<Vector2>();

	public byte[,,] Voxels;

	public Chunk(Vector2Int _coord, World _world, WorldAttributes _worldAttributes, BlocksAttributes _blocksAttributes)
	{

		worldAttributes = _worldAttributes;
		blocksAttributes = _blocksAttributes;

		coord = _coord;
		chunkObject = new GameObject();
		chunkObject.transform.position = new Vector3(coord.x * worldAttributes.ChunkWidth, 0f, coord.y * worldAttributes.ChunkWidth);

		Voxels = new byte[worldAttributes.ChunkWidth, worldAttributes.ChunkHeight, worldAttributes.ChunkWidth];		

		meshRenderer = chunkObject.AddComponent<MeshRenderer>();
		meshFilter = chunkObject.AddComponent<MeshFilter>();        

		chunkObject.transform.SetParent(_world.transform);
		meshRenderer.material = blocksAttributes.Material;

		chunkObject.name = coord.x + ", " + coord.y;

	}
	
	private void ClearMeshData()
	{

		vertexIndex = 0;
		vertices.Clear();
		triangles.Clear();
		uvs.Clear();

	}

	public void Clear()
	{

		ClearMeshData();

		for (int i = 0; i < worldAttributes.ChunkWidth; ++i)
		{

			for (int j = 0; j < worldAttributes.ChunkHeight; ++j)
			{

				for (int k = 0; k < worldAttributes.ChunkWidth; ++k)
				{

					Voxels[i, j, k] = 0;

				}

			}

		}

	}

	public void Update ()
	{

		ClearMeshData();
		UpdateChunk();
		CreateMesh();

	}

	private void UpdateChunk()
	{		

		for (int y = 0; y < worldAttributes.ChunkHeight; ++y)
		{

			for (int x = 0; x < worldAttributes.ChunkWidth; ++x)
			{

				for (int z = 0; z < worldAttributes.ChunkWidth; ++z)
				{

					UpdateMeshData(new Vector3(x, y, z));

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

		return blocksAttributes.Blocktypes[Voxels[x, y, z]].isSolid;

	}

	private void UpdateMeshData(Vector3 pos)
	{

		if (CheckVoxel(pos))
		{

			for (int p = 0; p < 6; p++)
			{

				if (!CheckVoxel(pos + VoxelData.faceChecks[p]))
				{

					byte blockID = Voxels[(int)pos.x, (int)pos.y, (int)pos.z];

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
