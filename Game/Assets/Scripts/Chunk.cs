using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{

    private World world;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    public Vector2Int coord;
    private GameObject chunkObject;

    private int vertexIndex = 0;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

    public byte[,,] Voxels;

    private bool _isActive;

    public bool isActive
    {

        get { return _isActive; }

        set
        {

            _isActive = value;

            if (chunkObject != null)
            {

                chunkObject.SetActive(value);

            }

        }

    }

    public Vector3 position
    {

        get { return chunkObject.transform.position; }

    }

    public Chunk(Vector2Int _coord, World _world)
    {
        world = _world;

        coord = _coord;
        chunkObject = new GameObject();
        chunkObject.transform.position = new Vector3(coord.x * world.WorldAttributes.ChunkWidth, 0f, coord.y * world.WorldAttributes.ChunkWidth);

        Voxels = new byte[world.WorldAttributes.ChunkWidth, world.WorldAttributes.ChunkHeight, world.WorldAttributes.ChunkWidth];

        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshFilter = chunkObject.AddComponent<MeshFilter>();

        chunkObject.transform.SetParent(_world.transform);
        meshRenderer.material = world.BlocksAttributes.Material;

        chunkObject.name = coord.x + ", " + coord.y;

        isActive = false;

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

        for (int i = 0; i < world.WorldAttributes.ChunkWidth; ++i)
        {

            for (int j = 0; j < world.WorldAttributes.ChunkHeight; ++j)
            {

                for (int k = 0; k < world.WorldAttributes.ChunkWidth; ++k)
                {

                    Voxels[i, j, k] = 0;

                }

            }

        }

    }

    public void Update()
    {

        ClearMeshData();
        UpdateChunk();
        CreateMesh();

    }

    private void UpdateChunk()
    {

        for (int y = 0; y < world.WorldAttributes.ChunkHeight; ++y)
        {

            for (int x = 0; x < world.WorldAttributes.ChunkWidth; ++x)
            {

                for (int z = 0; z < world.WorldAttributes.ChunkWidth; ++z)
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

        if (x < 0 || x >= world.WorldAttributes.ChunkWidth || y < 0 || y >= world.WorldAttributes.ChunkHeight || z < 0 || z >= world.WorldAttributes.ChunkWidth)
        {

            return false;

        }

        return world.BlocksAttributes.Blocktypes[Voxels[x, y, z]].isSolid;

    }

    bool IsVoxelInChunk(int x, int y, int z)
    {

        if (x < 0 || x >= world.WorldAttributes.ChunkWidth || y < 0 || y >= world.WorldAttributes.ChunkHeight || z < 0 || z >= world.WorldAttributes.ChunkWidth)
        {

            return false;

        }
        else
        {

            return true;

        }

    }

    bool IsVoxelInChunk(Vector3 pos)
    {

        if (pos.x < 0 || pos.x >= world.WorldAttributes.ChunkWidth || pos.y < 0 || pos.y >= world.WorldAttributes.ChunkHeight || pos.z < 0 || pos.z >= world.WorldAttributes.ChunkWidth)
        {

            return false;

        }
        else
        {

            return true;

        }

    }

    public void EditVoxel(Vector3 pos, byte newID)
    {

        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        x -= Mathf.FloorToInt(position.x);
        z -= Mathf.FloorToInt(position.z);

        Voxels[x, y, z] = newID;

        UpdateSurroundingVoxels(x, y, z);

        Update();

    }

    void UpdateSurroundingVoxels(int x, int y, int z)
    {

        Vector3 thisVoxel = new Vector3(x, y, z);

        for (int p = 0; p < 6; p++)
        {

            Vector3 currentVoxel = thisVoxel + VoxelData.faceChecks[p];

            if (!IsVoxelInChunk(currentVoxel))
            {

                Vector2Int ChunkCoord = world.GetChunkCoord(currentVoxel + position);

                world.Chunks[ChunkCoord.x, ChunkCoord.y].Update();

            }

        }

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

                    AddTexture(world.BlocksAttributes.Blocktypes[blockID].GetTextureID(p));

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

        float y = textureID / world.BlocksAttributes.TextureAtlasSizeInBlocks;
        float x = textureID - (y * world.BlocksAttributes.TextureAtlasSizeInBlocks);

        x *= world.BlocksAttributes.NormalizedBlockTextureSize;
        y *= world.BlocksAttributes.NormalizedBlockTextureSize;

        y = 1f - y - world.BlocksAttributes.NormalizedBlockTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + world.BlocksAttributes.NormalizedBlockTextureSize));
        uvs.Add(new Vector2(x + world.BlocksAttributes.NormalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + world.BlocksAttributes.NormalizedBlockTextureSize, y + world.BlocksAttributes.NormalizedBlockTextureSize));

    }

}
