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

        List<VoronoiDiagram.GraphEdge> Edges = SetBioms(world);

        BuildTerrain(world);

        SmoothingBorders(Edges, world);

        AddWater(Edges, world);

        AddSoil(world);

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

    private List<VoronoiDiagram.GraphEdge> SetBioms(World world)
    {

        List<VoronoiDiagram.GraphEdge> Edges = SetVoronoiDiagram(world);

        PutBioms(world);

        HideEdges(Edges, world);

        return Edges;

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

        do
        {

            DrawPoint(Vector2Int.FloorToInt(begin), world, biome);

            begin = Vector2.MoveTowards(begin, end, 1f);

        } while (begin != end);

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

    private void BuildTerrain(World world)
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

                SetColumn(new Vector2Int(x, z), world, world.WorldAttributes.BiomeAttributes[biome]);

            }

        }

    }

    private int GetTerrainHeight(Vector2 pos, World world, BiomeAttributes biome)
    {

        int terrainHeight = biome.SolidGroundHeight;

        for (int i = 0; i < biome.OctavesNumber; ++i)
        {

            terrainHeight += Mathf.FloorToInt((1 / Mathf.Pow(2, i)) * biome.BiomeHeight * Noise.Get2DPerlin(world, pos, i, biome.BiomeScale * Mathf.Pow(2, i)));

        }

        return terrainHeight;

    }

    private void SetColumn(Vector2Int ColumnPos, World world, BiomeAttributes biome)
    {

        int terrainHeight = GetTerrainHeight(ColumnPos, world, biome);

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

    private void SmoothingBorders(List<VoronoiDiagram.GraphEdge> Edges, World world)
    {

        //for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        //{

        //    for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
        //    {

        //        if (world.Bioms[x, z] == 0)
        //        {

        //            SmoothingColumn(new Vector2(x, z), world);

        //            SmoothingColumn(new Vector2(x - 1, z - 1), world);
        //            SmoothingColumn(new Vector2(x + 1, z - 1), world);
        //            SmoothingColumn(new Vector2(x - 1, z + 1), world);
        //            SmoothingColumn(new Vector2(x + 1, z + 1), world);

        //        }

        //    }

        //}

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

            if (biomeA != biomeB)
            {

                Vector2 begin = new Vector2((float)edge.x1, (float)edge.y1);
                Vector2 end = new Vector2((float)edge.x2, (float)edge.y2);

                do
                {

                    SmoothingColumn(new Vector2(begin.x, begin.y), world);

                    SmoothingColumn(new Vector2(begin.x - 1, begin.y - 1), world);
                    SmoothingColumn(new Vector2(begin.x + 1, begin.y - 1), world);
                    SmoothingColumn(new Vector2(begin.x - 1, begin.y + 1), world);
                    SmoothingColumn(new Vector2(begin.x + 1, begin.y + 1), world);

                    begin = Vector2.MoveTowards(begin, end, 1f);

                } while (begin != end);

            }

        }

    }

    private void SmoothingColumn(Vector2 ColumnPos, World world)
    {

        if (!world.IsVoxelInWorld(ColumnPos))
        {

            return;

        }

        int avrgY;

        Vector2Int ChunkCoord = world.GetChunkCoord(ColumnPos);
        Vector2Int InChunkCoord = world.GetInChunkCoord(ColumnPos);

        avrgY = GetTopBlockHeight(ColumnPos, world);
        avrgY += GetTopBlockHeight(new Vector2(ColumnPos.x - 1, ColumnPos.y), world);
        avrgY += GetTopBlockHeight(new Vector2(ColumnPos.x + 1, ColumnPos.y), world);
        avrgY += GetTopBlockHeight(new Vector2(ColumnPos.x, ColumnPos.y - 1), world);
        avrgY += GetTopBlockHeight(new Vector2(ColumnPos.x, ColumnPos.y + 1), world);
        avrgY += GetTopBlockHeight(new Vector2(ColumnPos.x - 1, ColumnPos.y - 1), world);
        avrgY += GetTopBlockHeight(new Vector2(ColumnPos.x - 1, ColumnPos.y + 1), world);
        avrgY += GetTopBlockHeight(new Vector2(ColumnPos.x + 1, ColumnPos.y - 1), world);
        avrgY += GetTopBlockHeight(new Vector2(ColumnPos.x + 1, ColumnPos.y + 1), world);

        avrgY /= 9;

        for (int y = world.WorldAttributes.ChunkHeight - 1; y > avrgY; --y)
        {

            world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] = 0;

        }

        for (int y = avrgY; y > 0 && world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] == 0; --y)
        {

            world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] = 2;

        }

    }

    private int GetTopBlockHeight(Vector2 pos, World world)
    {

        if (world.IsVoxelInWorld(pos))
        {

            return world.GetTopBlockHeight(pos);


        }
        else
        {

            return 0;

        }

    }

    private void AddWater(List<VoronoiDiagram.GraphEdge> Edges, World world)
    {

        int waterHeight = 25;

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                if (world.Bioms[x, z] == 0)
                {

                    AddWaterColumn(new Vector2Int(x, z), world, waterHeight);

                }

            }

        }

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

            if (biomeA != biomeB)
            {

                Vector2 begin = new Vector2((float)edge.x1, (float)edge.y1);
                Vector2 end = new Vector2((float)edge.x2, (float)edge.y2);

                do
                {

                    SetRiverPart(Vector2Int.FloorToInt(begin), world);

                    begin = Vector2.MoveTowards(begin, end, 1f);

                } while (begin != end);

            }

        }

    }

    private void AddWaterColumn(Vector2Int start, World world, int waterHeight)
    {

        Vector2Int ChunkCoord = world.GetChunkCoord(start);
        Vector2Int InChunkCoord = world.GetInChunkCoord(start);

        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        for (int y = waterHeight; y > 0 && world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] == 0; --y)
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

            ChunkCoord = world.GetChunkCoord(pos);
            InChunkCoord = world.GetInChunkCoord(pos);

            if (world.IsVoxelInWorld(pos) && world.Bioms[pos.x, pos.y] != 0)
            {

                if (world.GetTopBlockHeight(pos) < waterHeight)
                {

                    world.Bioms[pos.x, pos.y] = 0;

                    for (int y = waterHeight; y > 0 && world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] == 0; --y)
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

    private void SetRiverPart(Vector2Int center, World world)
    {

        if (!world.IsVoxelInWorld(center))
        {

            return;

        }
        
        Queue<Vector2Int> checkedColumns;

        CheckRiverColumn(center, 2, world, out checkedColumns);

        int centerHeight = 25 - world.WorldAttributes.RiverDepth;

        while (checkedColumns.Count != 0)
        {

            Vector2Int pos = checkedColumns.Dequeue();

            float distance = Vector2Int.Distance(center, pos);

            int height = Mathf.RoundToInt(distance * distance * 0.25f) + centerHeight;

            int blockHeight = GetTopBlockHeight(pos, world);

            if (blockHeight > height)
            {

                Vector2Int ChunkCoord = world.GetChunkCoord(pos);
                Vector2Int InChunkCoord = world.GetInChunkCoord(pos);

                for (int y = blockHeight; y > height; --y)
                {

                    world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] = 0;

                }

            }

            AddWaterColumn(pos, world, 25);

        }

    }

    private void CheckRiverColumn(Vector2Int center, int radius, World world, out Queue<Vector2Int> checkedColumns)
    {

        checkedColumns = new Queue<Vector2Int>();

        if (world.IsVoxelInWorld(center))
        {

            checkedColumns.Enqueue(center);

        }

        for (int r = 1; r <= radius; ++r)
        {

            for (int k = -r; k <= r; ++k)
            {

                Vector2Int pos = new Vector2Int(center.x + k, center.y + r);

                if (world.IsVoxelInWorld(pos))
                {

                    checkedColumns.Enqueue(pos);

                }

                pos = new Vector2Int(center.x + k, center.y - r);

                if (world.IsVoxelInWorld(pos))
                {

                    checkedColumns.Enqueue(pos);

                }

            }

            for (int k = -r + 1; k < r; ++k)
            {

                Vector2Int pos = new Vector2Int(center.x + r, center.y + k);

                if (world.IsVoxelInWorld(pos))
                {

                    checkedColumns.Enqueue(pos);

                }

                pos = new Vector2Int(center.x - r, center.y + k);

                if (world.IsVoxelInWorld(pos))
                {

                    checkedColumns.Enqueue(pos);

                }

            }

        }

    }

    private void AddSoil(World world)
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

                AddSoilInColumn(new Vector2(x, z), world, world.WorldAttributes.BiomeAttributes[biome]);

            }

        }

    }

    private void AddSoilInColumn(Vector2 ColumnPos, World world, BiomeAttributes biome)
    {

        Vector2Int ChunkCoord = world.GetChunkCoord(ColumnPos);
        Vector2Int InChunkCoord = world.GetInChunkCoord(ColumnPos);

        int y;

        int terrainHeight = world.GetTopBlockHeight(ColumnPos);

        world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, terrainHeight, InChunkCoord.y] = biome.MainVoxel;

        int groundDepth = UnityEngine.Random.Range(biome.GroundDepthMin, biome.GroundDepthMax + 1);

        for (y = terrainHeight - 1; y > terrainHeight - groundDepth && y > 0; --y)
        {

            world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] = biome.SecondVoxel;

        }

    }

}
