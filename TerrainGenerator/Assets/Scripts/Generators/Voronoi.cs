using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Voronoi : IWorldGenerator
{

    World world;

    void IWorldGenerator.GenerateWorld(World _world)
    {

        world = _world;

        InitChunks();

        List<VoronoiDiagram.GraphEdge> Edges = SetBioms();

        BuildTerrain();

        SmoothingBorders(Edges);

        AddWater(Edges);

        AddSoil();

        world.UpdateChunks();

    }

    private void InitChunks()
    {

        for (int x = 0; x < world.WorldAttributes.WorldSizeInChunks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInChunks; ++z)
            {

                world.CreateChunk(new Vector2Int(x, z));

            }

        }

    }

    private List<VoronoiDiagram.GraphEdge> SetBioms()
    {

        List<VoronoiDiagram.GraphEdge> Edges = SetVoronoiDiagram();

        PutBioms();

        HideEdges(Edges);

        return Edges;

    }

    private void HideEdges(List<VoronoiDiagram.GraphEdge> Edges)
    {

        foreach (var edge in Edges)
        {

            int biomeA, biomeB;

            GetBorderingBioms(edge, out biomeA, out biomeB);

            if (biomeA == biomeB && biomeA != -1)
            {

                DrawLine(edge, biomeA);

            }
            else
            {

                if (biomeA != -1)
                {

                    DrawLine(edge, biomeA);

                }
                else if (biomeB != -1)
                {

                    DrawLine(edge, biomeB);

                }
                else
                {

                    DrawLine(edge, 0);

                }

            }

        }

    }

    private List<VoronoiDiagram.GraphEdge> SetVoronoiDiagram()
    {

        InitBioms(world);

        VoronoiDiagram voronoi = new VoronoiDiagram(10);

        double[] xVal = new double[world.WorldAttributes.BiomeNumber];
        double[] yVal = new double[world.WorldAttributes.BiomeNumber];

        for (int i = 0; i < world.WorldAttributes.BiomeNumber; ++i)
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

            DrawLine(edge, -1);

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

    private void DrawLine(VoronoiDiagram.GraphEdge edge, int biome)
    {

        Vector2 A = new Vector2((float)edge.x1, (float)edge.y1);
        Vector2 B = new Vector2((float)edge.x2, (float)edge.y2);

        do
        {

            DrawPoint(Vector2Int.FloorToInt(A), world.WorldAttributes.BoarderBrushRadius, biome);

            A = Vector2.MoveTowards(A, B, 1f);

        } while (A != B);

    }

    private void DrawPoint(Vector2Int center, int radius, int biome)
    {

        foreach (var point in Brush(center, radius))
        {

            if (world.IsVoxelInWorld(point))
            {

                world.Bioms[point.x, point.y] = biome;

            }

        }

    }

    private void PutBioms()
    {

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                if (world.Bioms[x, z] == -2)
                {

                    FillBorder(new Vector2Int(x, z), Random.Range(0, world.WorldAttributes.BiomeAttributes.Length));

                }

            }

        }

    }

    private void FillBorder(Vector2Int start, int biome)
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

    private void BuildTerrain()
    {

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                int biome = world.Bioms[x, z];

                if (biome < 0)
                {

                    biome = 0;
                    world.Bioms[x, z] = biome;

                }

                SetColumn(new Vector2Int(x, z), world.WorldAttributes.BiomeAttributes[biome]);

            }

        }

    }

    private int GetTerrainHeight(Vector2 pos, BiomeAttributes biome)
    {

        int terrainHeight = biome.SolidGroundHeight;

        for (int i = 0; i < biome.OctavesNumber; ++i)
        {

            terrainHeight += Mathf.FloorToInt((1 / Mathf.Pow(2, i)) * biome.BiomeHeight * Noise.Get2DPerlin(world, pos, i, biome.BiomeScale * Mathf.Pow(2, i)));

        }

        return terrainHeight;

    }

    private void SetColumn(Vector2Int ColumnPos, BiomeAttributes biome)
    {

        int terrainHeight = GetTerrainHeight(ColumnPos, biome);

        if (world.Bioms[ColumnPos.x, ColumnPos.y] == -1)
        {

            terrainHeight -= 4;

        }

        Vector2Int ChunkCoord = world.GetChunkCoord(ColumnPos);
        Vector2Int InChunkPos = world.GetInChunkCoord(ColumnPos);

        int y;

        world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkPos.x, terrainHeight, InChunkPos.y] = 2;

        for (y = terrainHeight - 1; y > 0; --y)
        {

            world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkPos.x, y, InChunkPos.y] = 2;

        }

        world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkPos.x, 0, InChunkPos.y] = 1;

    }

    private void SmoothingBorders(List<VoronoiDiagram.GraphEdge> Edges)
    {

        foreach (var edge in Edges)
        {

            int biomeA, biomeB;

            GetBorderingBioms(edge, out biomeA, out biomeB);

            if (biomeA != biomeB)
            {

                Vector2 A = new Vector2((float)edge.x1, (float)edge.y1);
                Vector2 B = new Vector2((float)edge.x2, (float)edge.y2);

                do
                {

                    SmoothingPoint(Vector2Int.FloorToInt(A), world.WorldAttributes.SmoothingBrushRadius);

                    A = Vector2.MoveTowards(A, B, 1f);

                } while (A != B);

            }

        }

    }

    private void SmoothingColumn(Vector2Int ColumnPos)
    {

        if (!world.IsVoxelInWorld(ColumnPos))
        {

            return;

        }

        int avrgHeight = 0;
        int count = 0;

        Vector2Int ChunkCoord = world.GetChunkCoord(ColumnPos);
        Vector2Int InChunkCoord = world.GetInChunkCoord(ColumnPos);

        foreach (var column in Brush(ColumnPos, world.WorldAttributes.SmoothingCheckBrushRadius))
        {

            avrgHeight += GetTopBlockHeight(column);
            ++count;

        }

        avrgHeight = Mathf.RoundToInt((float)avrgHeight / count);

        for (int y = world.WorldAttributes.ChunkHeight - 1; y > avrgHeight; --y)
        {

            world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] = 0;

        }

        for (int y = avrgHeight; y > 0 && world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] == 0; --y)
        {

            world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] = 2;

        }

    }

    private void SmoothingPoint(Vector2Int center, int radius)
    {

        foreach (var pos in Brush(center, radius))
        {

            SmoothingColumn(pos);

        }

    }

    private int GetTopBlockHeight(Vector2 pos)
    {

        if (world.IsVoxelInWorld(pos))
        {

            return world.GetTopSoilBlockHeight(pos);

        }
        else
        {

            return 0;

        }

    }

    private void AddWater(List<VoronoiDiagram.GraphEdge> Edges)
    {

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                if (world.Bioms[x, z] == 0)
                {

                    AddWaterColumn(new Vector2Int(x, z), world.WorldAttributes.OceanHeight);

                }

            }

        }

        foreach (var edge in Edges)
        {

            int biomeA, biomeB;

            GetBorderingBioms(edge, out biomeA, out biomeB);

            if (biomeA != biomeB)
            {

                Vector2 begin = new Vector2((float)edge.x1, (float)edge.y1);
                Vector2 end = new Vector2((float)edge.x2, (float)edge.y2);

                do
                {

                    SetRiverPoint(Vector2Int.FloorToInt(begin));

                    begin = Vector2.MoveTowards(begin, end, 1f);

                } while (begin != end);

            }

        }

    }

    private void AddWaterColumn(Vector2Int start, int waterHeight)
    {

        Vector2Int ChunkCoord = world.GetChunkCoord(start);
        Vector2Int InChunkCoord = world.GetInChunkCoord(start);

        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        for (int y = waterHeight; y > 0
            && world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] == 0
            || world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] == 9; --y)
        {

            world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] = 9;

        }

        stack.Push(new Vector2Int(start.x - 1, start.y));
        stack.Push(new Vector2Int(start.x + 1, start.y));
        stack.Push(new Vector2Int(start.x, start.y - 1));
        stack.Push(new Vector2Int(start.x, start.y + 1));

        while (stack.Count != 0)
        {

            Vector2Int pos = stack.Pop();

            if (world.IsVoxelInWorld(pos) && world.Bioms[pos.x, pos.y] != 0)
            {

                ChunkCoord = world.GetChunkCoord(pos);
                InChunkCoord = world.GetInChunkCoord(pos);

                if (world.GetTopSoilBlockHeight(pos) < waterHeight)
                {

                    world.Bioms[pos.x, pos.y] = 0;

                    for (int y = waterHeight; y > 0
                        && world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] == 0
                        || world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] == 9; --y)
                    {

                        world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] = 9;

                    }

                    stack.Push(new Vector2Int(pos.x - 1, pos.y));
                    stack.Push(new Vector2Int(pos.x + 1, pos.y));
                    stack.Push(new Vector2Int(pos.x, pos.y - 1));
                    stack.Push(new Vector2Int(pos.x, pos.y + 1));

                }

            }

        }

    }

    private void SetRiverPoint(Vector2Int center)
    {

        if (!world.IsVoxelInWorld(center))
        {

            return;

        }

        int centerHeight = world.WorldAttributes.OceanHeight - world.WorldAttributes.RiverDepth;

        foreach (var pos in Brush(center, world.WorldAttributes.RiverBrushRadius))
        {

            float distance = Vector2Int.Distance(center, pos);

            int height = Mathf.RoundToInt(distance * distance * world.WorldAttributes.RiverBrushScale) + centerHeight;

            int blockHeight = world.GetTopSoilBlockHeight(pos);

            if (blockHeight > height)
            {

                Vector2Int ChunkCoord = world.GetChunkCoord(pos);
                Vector2Int InChunkCoord = world.GetInChunkCoord(pos);

                for (int y = blockHeight; y > height; --y)
                {

                    world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] = 0;

                }

            }

            AddWaterColumn(pos, world.WorldAttributes.OceanHeight);

        }

    }

    private void AddSoil()
    {

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                int biome = world.Bioms[x, z];

                if (biome == -1 || biome == -2)
                {

                    biome = 0;

                }

                AddSoilInColumn(new Vector2(x, z), world.WorldAttributes.BiomeAttributes[biome]);

            }

        }

    }

    private void AddSoilInColumn(Vector2 ColumnPos, BiomeAttributes biome)
    {

        Vector2Int ChunkCoord = world.GetChunkCoord(ColumnPos);
        Vector2Int InChunkCoord = world.GetInChunkCoord(ColumnPos);

        int y;

        int terrainHeight = world.GetTopSoilBlockHeight(ColumnPos);

        world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, terrainHeight, InChunkCoord.y] = biome.MainVoxel;

        int groundDepth = Random.Range(biome.GroundDepthMin, biome.GroundDepthMax + 1);

        for (y = terrainHeight - 1; y > terrainHeight - groundDepth && y > 0; --y)
        {

            world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] = biome.SecondVoxel;

        }

    }

    private IEnumerable<Vector2Int> Brush(Vector2Int center, int radius)
    {

        if (world.IsVoxelInWorld(center))
        {

            yield return center;

        }

        for (int r = 1; r <= radius; ++r)
        {

            for (int k = -r; k <= r; ++k)
            {

                Vector2Int pos = new Vector2Int(center.x + k, center.y + r);

                if (world.IsVoxelInWorld(pos))
                {

                    yield return pos;

                }

                pos = new Vector2Int(center.x + k, center.y - r);

                if (world.IsVoxelInWorld(pos))
                {

                    yield return pos;

                }

            }

            for (int k = -r + 1; k < r; ++k)
            {

                Vector2Int pos = new Vector2Int(center.x + r, center.y + k);

                if (world.IsVoxelInWorld(pos))
                {

                    yield return pos;

                }

                pos = new Vector2Int(center.x - r, center.y + k);

                if (world.IsVoxelInWorld(pos))
                {

                    yield return pos;

                }

            }

        }

    }

    private void GetBorderingBioms(VoronoiDiagram.GraphEdge edge, out int biomeA, out int biomeB)
    {

        double x = edge.x2 - edge.x1;
        double y = edge.y2 - edge.y1;

        Vector2 normal = Vector2.Perpendicular(new Vector2((float)x, (float)y)).normalized;

        Vector2 middle = new Vector2((float)((edge.x2 + edge.x1) / 2), (float)((edge.y2 + edge.y1) / 2));

        Vector2 A = middle + (normal * world.WorldAttributes.CheckBiomeDistance);
        Vector2 B = middle + (normal * -world.WorldAttributes.CheckBiomeDistance);

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

    }

}
