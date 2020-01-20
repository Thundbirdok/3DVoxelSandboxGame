using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [SerializeField]
    private WorldAttributes worldAttributes;
    [SerializeField]
    private BlocksAttributes blocksAttributes;

    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private MeshFilter meshFilter;

    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    byte[,,] voxelMap;

    void Start()
    {

        voxelMap = new byte[worldAttributes.ChunkWidth, worldAttributes.ChunkHeight, worldAttributes.ChunkWidth];

        GenerateVoxelMap();
        CreateMeshData();
        CreateMesh();

    }

    void GenerateVoxelMap()
    {

        for (byte y = 0; y < worldAttributes.ChunkHeight; ++y)
        {

            for (byte x = 0; x < worldAttributes.ChunkWidth; ++x)
            {

                for (byte z = 0; z < worldAttributes.ChunkWidth; ++z)
                {

                    voxelMap[x, y, z] = 0;

                }

            }

        }

        for (byte y = 0; y < worldAttributes.ChunkHeight; ++y)
        {

            for (int x = (y * worldAttributes.ChunkWidth) / worldAttributes.ChunkHeight; x < worldAttributes.ChunkWidth; ++x)
            {

                for (int z = (y * worldAttributes.ChunkWidth) / worldAttributes.ChunkHeight; z < worldAttributes.ChunkWidth; ++z)
                {

                    if (y == worldAttributes.ChunkHeight - 1)
                    {

                        voxelMap[x, y, z] = 3;

                    }
                    else if (y == 0)
                    {

                        voxelMap[x, 0, z] = 1;

                    }
                    else
                    {

                        voxelMap[x, y, z] = (byte)(2 + y % 5);

                    }

                }

            }

        }

    }

    void CreateMeshData()
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

    bool CheckVoxel(Vector3 pos)
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

    void AddVoxelDataToChunk(Vector3 pos)
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

    void CreateMesh()
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

    void AddTexture(int textureID)
    {

        float y = textureID / VoxelData.TextureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData.TextureAtlasSizeInBlocks);

        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;

        y = 1f - y - VoxelData.NormalizedBlockTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));

    }

}
