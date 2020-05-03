using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Voronoi : IWorldGenerator
{

    void IWorldGenerator.GenerateWorld(World world)
    {

        InitChunks(world);

        SetBioms(world);

        world.UpdateChunks();

    }

    private void InitChunks(World world)
    {

        for (int x = 0; x < world.WorldAttributes.WorldSizeInChunks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInChunks; ++z)
            {

                world.CreateChunk(new Vector2Int(x, z));

            }

        }

    }

    private void SetBioms(World world)
    {

        List<VoronoiDiagram.GraphEdge> Edges = SetVoronoiDiagram(world);

        PutBioms(world);

        HideEdges(Edges, world);

        ///

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                Vector2Int ChunkCoord = world.GetChunkCoord(new Vector2(x, z));
                Vector2Int InChunkCoord = world.GetInChunkCoord(new Vector2(x, z));

                if (world.Bioms[x, z] == 0)
                {

                    world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, 1, InChunkCoord.y] = 9;

                }
                else if (world.Bioms[x, z] == 1)
                {

                    world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, 1, InChunkCoord.y] = world.WorldAttributes.BiomeAttributes[1].MainVoxel;

                }
                else if (world.Bioms[x, z] == 2)
                {

                    world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, 1, InChunkCoord.y] = world.WorldAttributes.BiomeAttributes[2].MainVoxel;

                }

                world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, 0, InChunkCoord.y] = 1;

            }

        }

        ///

    }

    private void HideEdges(List<VoronoiDiagram.GraphEdge> Edges, World world)
    {

        foreach (var edge in Edges)
        {

            double x = edge.x2 - edge.x1;
            double y = edge.y2 - edge.y1;

            Vector2 normal = Vector2.Perpendicular(new Vector2((float)x, (float)y)).normalized;                       

            Vector2 middle = new Vector2((float)((edge.x2 + edge.x1) / 2), (float)((edge.y2 + edge.y1) / 2));

            Vector2 A = middle + (normal * 10);
            Vector2 B = middle + (normal * -10);

            int biomeA;
            int biomeB;

            if (world.IsVoxelInWorld(A))
            {

                biomeA = world.Bioms[Mathf.FloorToInt(A.x), Mathf.FloorToInt(A.y)];

            }
            else
            {

                biomeA = 0;

            }

            if (world.IsVoxelInWorld(B))
            {

                biomeB = world.Bioms[Mathf.FloorToInt(B.x), Mathf.FloorToInt(B.y)];

            }
            else
            {

                biomeB = 0;

            }

            if (biomeA == biomeB && biomeA != -1)
            {

                DrawLine(edge, world, biomeA);

            }
            else
            {

                if (biomeA != -1)
                {

                    DrawLine(edge, world, biomeA);

                }
                else if (biomeB != -1)
                {

                    DrawLine(edge, world, biomeB);

                }
                else
                {

                    DrawLine(edge, world, 0);

                }

            }

        }

    }

    private List<VoronoiDiagram.GraphEdge> SetVoronoiDiagram(World world)
    {

        InitBioms(world);

        VoronoiDiagram voronoi = new VoronoiDiagram(10);

        double[] xVal = new double[/*world.WorldAttributes...*/25];
        double[] yVal = new double[25];

        for (int i = 0; i < 25; ++i)
        {

            xVal[i] = Random.Range(0, world.WorldAttributes.WorldSizeInBlocks);
            yVal[i] = Random.Range(0, world.WorldAttributes.WorldSizeInBlocks);

        }

        List<VoronoiDiagram.GraphEdge> Edges = voronoi.GenerateDiagram(xVal, yVal, 0, world.WorldAttributes.WorldSizeInBlocks, 0, world.WorldAttributes.WorldSizeInBlocks);

        List<VoronoiDiagram.GraphEdge> ClearedEdges = new List<VoronoiDiagram.GraphEdge>();

        for (int i = 0; i < Edges.Count; ++i)
        {

            if (Edges[i].x1 != Edges[i].x2 || Edges[i].y1 != Edges[i].y2)
            {

                ClearedEdges.Add(Edges[i]);

            }

        }

        foreach (var edge in ClearedEdges)
        {

            DrawLine(edge, world, -1);

        }

        return ClearedEdges;

    }

    private void InitBioms(World world)
    {

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                world.Bioms[x, z] = -2;

            }

        }

    }

    private void DrawLine(VoronoiDiagram.GraphEdge edge, World world, int biome)
    {

        Vector2 begin = new Vector2((float)edge.x1, (float)edge.y1);
        Vector2 end = new Vector2((float)edge.x2, (float)edge.y2);

        DrawPoint(Vector2Int.FloorToInt(begin), world, biome);

        while (begin != end)
        {

            begin = Vector2.MoveTowards(begin, end, 1f);

            DrawPoint(Vector2Int.FloorToInt(begin), world, biome);

        }

    }

    private void DrawPoint(Vector2Int pos, World world, int biome)
    {

        Vector2Int sidePoint1 = new Vector2Int(pos.x - 1, pos.y);
        Vector2Int sidePoint2 = new Vector2Int(pos.x + 1, pos.y);
        Vector2Int sidePoint3 = new Vector2Int(pos.x, pos.y - 1);
        Vector2Int sidePoint4 = new Vector2Int(pos.x, pos.y + 1);
        Vector2Int sidePoint5 = new Vector2Int(pos.x - 1, pos.y - 1);
        Vector2Int sidePoint6 = new Vector2Int(pos.x - 1, pos.y + 1);
        Vector2Int sidePoint7 = new Vector2Int(pos.x + 1, pos.y - 1);
        Vector2Int sidePoint8 = new Vector2Int(pos.x + 1, pos.y + 1);

        if (world.IsVoxelInWorld(pos))
        {

            world.Bioms[pos.x, pos.y] = biome;

        }

        if (world.IsVoxelInWorld(sidePoint1))
        {

            world.Bioms[sidePoint1.x, sidePoint1.y] = biome;

        }

        if (world.IsVoxelInWorld(sidePoint2))
        {

            world.Bioms[sidePoint2.x, sidePoint2.y] = biome;

        }

        if (world.IsVoxelInWorld(sidePoint3))
        {

            world.Bioms[sidePoint3.x, sidePoint3.y] = biome;

        }

        if (world.IsVoxelInWorld(sidePoint4))
        {

            world.Bioms[sidePoint4.x, sidePoint4.y] = biome;

        }

        if (world.IsVoxelInWorld(sidePoint5))
        {

            world.Bioms[sidePoint5.x, sidePoint5.y] = biome;

        }

        if (world.IsVoxelInWorld(sidePoint6))
        {

            world.Bioms[sidePoint6.x, sidePoint6.y] = biome;

        }

        if (world.IsVoxelInWorld(sidePoint7))
        {

            world.Bioms[sidePoint7.x, sidePoint7.y] = biome;

        }

        if (world.IsVoxelInWorld(sidePoint8))
        {

            world.Bioms[sidePoint8.x, sidePoint8.y] = biome;

        }

    }

    private void PutBioms(World world)
    {

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                if (world.Bioms[x, z] == -2)
                {

                    FillBorder(world, new Vector2Int(x, z), Random.Range(0, world.WorldAttributes.BiomeAttributes.Length));

                }

            }

        }

    }

    private void FillBorder(World world, Vector2Int start, int biome)
    {

        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        stack.Push(start);

        while (stack.Count != 0)
        {

            Vector2Int pos = stack.Pop();

            if (world.IsVoxelInWorld(pos) && world.Bioms[pos.x, pos.y] == -2)
            {

                world.Bioms[pos.x, pos.y] = biome;

                stack.Push(new Vector2Int(pos.x - 1, pos.y));
                stack.Push(new Vector2Int(pos.x + 1, pos.y));
                stack.Push(new Vector2Int(pos.x, pos.y - 1));
                stack.Push(new Vector2Int(pos.x, pos.y + 1));
                stack.Push(new Vector2Int(pos.x - 1, pos.y - 1));
                stack.Push(new Vector2Int(pos.x - 1, pos.y + 1));
                stack.Push(new Vector2Int(pos.x + 1, pos.y - 1));
                stack.Push(new Vector2Int(pos.x + 1, pos.y + 1));

            }

        }

    }

}
