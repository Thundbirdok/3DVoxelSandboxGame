using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{    
    [SerializeField]
    private WorldAttributes worldAttributes;

    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private MeshFilter meshFilter;

    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    bool[,,] voxelMap;

    void Start()
    {        

        voxelMap = new bool[worldAttributes.ChunkWidth, worldAttributes.ChunkHeight, worldAttributes.ChunkWidth];        

        GenerateVoxelMap();
        CreateMeshData();
        CreateMesh();

    }

    void GenerateVoxelMap()
    {

        for (int y = 0; y < worldAttributes.ChunkHeight; ++y)
        {
            for (int x = 0; x < worldAttributes.ChunkWidth; ++x)
            {
                for (int z = 0; z < worldAttributes.ChunkWidth; ++z)
                {

                    voxelMap[x, y, z] = false;

                }
            }
        }

        for (int y = 0; y < worldAttributes.ChunkHeight; ++y)
        {
            for (int x = y; x < worldAttributes.ChunkWidth && x < y + (worldAttributes.ChunkWidth / 2); ++x)
            {
                for (int z = y; z < worldAttributes.ChunkWidth && z < y + (worldAttributes.ChunkWidth / 2); ++z)
                {

                    voxelMap[x, y, z] = true;

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

        return voxelMap[x, y, z];

    }

    void AddVoxelDataToChunk(Vector3 pos)
    {
        if (CheckVoxel(pos))
        {
            for (int p = 0; p < 6; ++p)
            {

                if (!CheckVoxel(pos + VoxelData.faceChecks[p]))
                {

                    vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
                    vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
                    vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
                    vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

                    uvs.Add(VoxelData.voxelUvs[0]);
                    uvs.Add(VoxelData.voxelUvs[1]);
                    uvs.Add(VoxelData.voxelUvs[2]);
                    uvs.Add(VoxelData.voxelUvs[3]);

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

}
