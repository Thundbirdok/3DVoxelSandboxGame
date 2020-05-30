using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseGenerator : IWorldGenerator
{

    World world;

    int[,] heightMap;
    int[,] waterMap;

    void IWorldGenerator.GenerateWorld(World _world)
    {

        world = _world;

        Init();

        SetBioms();

        SetHeightMap();

        SmoothingBorders();

        AddWater();

        BuildTerrain();

        AddSoil();

        world.UpdateChunks();

    }

    private void Init()
    {

        waterMap = new int[world.WorldAttributes.WorldSizeInBlocks, world.WorldAttributes.WorldSizeInBlocks];

        for (int x = 0; x < world.WorldAttributes.WorldSizeInChunks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInChunks; ++z)
            {

                world.CreateChunk(new Vector2Int(x, z));

                waterMap[x, z] = 0;

            }

        }

    }

    private void SetBioms()
    {

        float borderMin = 0.54f, borderMax = 0.55f;        

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                Vector2Int ColumnPos = new Vector2Int(x, z);

                float noise = PerlinNoise.Get2DPerlin(world, ColumnPos, 0.0f, world.WorldAttributes.WorldScale);

                if (noise < 0.5)
                {

                    world.Bioms[x, z] = 0;                    

                }
                else
                {

                    float borderNoise = PerlinNoise.Get2DPerlin(world, ColumnPos, 1f, world.WorldAttributes.WorldScale);

                    if (borderMin < borderNoise && borderMax > borderNoise)
                    {

                        world.Bioms[x, z] = -1;

                    }
                    else
                    {

                        world.Bioms[x, z] = -2;

                    }

                }

            }

        }

        PutBioms();

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

    private void SetHeightMap()
    {

        heightMap = new int[world.WorldAttributes.WorldSizeInBlocks, world.WorldAttributes.WorldSizeInBlocks];

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                heightMap[x, z] = GetTerrainHeight(new Vector2Int(x, z));

            }

        }

    }

    private int GetTerrainHeight(Vector2Int pos)
    {

        int biomeIndex = world.Bioms[pos.x, pos.y];

        if (biomeIndex == -1)
        {

            biomeIndex = 1;

        }

        BiomeAttributes biome = world.WorldAttributes.BiomeAttributes[biomeIndex];

        int terrainHeight = biome.SolidGroundHeight;

        for (int i = 0; i < biome.OctavesNumber; ++i)
        {

            terrainHeight += Mathf.FloorToInt((1 / Mathf.Pow(2, i)) * biome.BiomeHeight * PerlinNoise.Get2DPerlin(world, pos, i, biome.BiomeScale * Mathf.Pow(2, i)));

        }

        return terrainHeight;

    }   

    private void SmoothingBorders()
    {

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                if (world.Bioms[x, z] == -1)
                {

                    SmoothingPoint(new Vector2Int(x, z), world.WorldAttributes.SmoothingBrushRadius + world.WorldAttributes.BoarderBrushRadius);

                }

            }

        }

    }

    private void SmoothingColumn(Vector2Int pos)
    {

        if (!world.IsVoxelInWorld(pos))
        {

            return;

        }

        int avrgHeight = 0;
        int count = 0;

        foreach (var column in Brush(pos, world.WorldAttributes.SmoothingCheckBrushRadius))
        {

            avrgHeight += heightMap[column.x, column.y];
            ++count;

        }

        avrgHeight = Mathf.RoundToInt((float)avrgHeight / count);

        if (waterMap[pos.x, pos.y] != 0)
        {

            waterMap[pos.x, pos.y] += heightMap[pos.x, pos.y] - avrgHeight;

            if (waterMap[pos.x, pos.y] < 0)
            {

                waterMap[pos.x, pos.y] = 0;

            }

        }
        else if (avrgHeight < heightMap[pos.x, pos.y])
        {

            if (world.IsVoxelInWorld(new Vector2Int(pos.x - 1, pos.y)) && avrgHeight < heightMap[pos.x - 1, pos.y] + waterMap[pos.x - 1, pos.y] && waterMap[pos.x - 1, pos.y] != 0)
            {

                waterMap[pos.x, pos.y] = heightMap[pos.x - 1, pos.y] + waterMap[pos.x - 1, pos.y] - avrgHeight;

                world.Bioms[pos.x, pos.y] = 0;

            }
            else if (world.IsVoxelInWorld(new Vector2Int(pos.x + 1, pos.y)) && avrgHeight < heightMap[pos.x + 1, pos.y] + waterMap[pos.x + 1, pos.y] && waterMap[pos.x + 1, pos.y] != 0)
            {

                waterMap[pos.x, pos.y] = heightMap[pos.x + 1, pos.y] + waterMap[pos.x + 1, pos.y] - avrgHeight;

                world.Bioms[pos.x, pos.y] = 0;

            }
            else if (world.IsVoxelInWorld(new Vector2Int(pos.x, pos.y - 1)) && avrgHeight < heightMap[pos.x, pos.y - 1] + waterMap[pos.x, pos.y - 1] && waterMap[pos.x, pos.y - 1] != 0)
            {

                waterMap[pos.x, pos.y] = heightMap[pos.x, pos.y - 1] + waterMap[pos.x, pos.y - 1] - avrgHeight;

                world.Bioms[pos.x, pos.y] = 0;

            }
            else if (world.IsVoxelInWorld(new Vector2Int(pos.x, pos.y + 1)) && avrgHeight < heightMap[pos.x, pos.y + 1] + waterMap[pos.x, pos.y + 1] && waterMap[pos.x, pos.y + 1] != 0)
            {

                waterMap[pos.x, pos.y] = heightMap[pos.x, pos.y + 1] + waterMap[pos.x, pos.y + 1] - avrgHeight;

                world.Bioms[pos.x, pos.y] = 0;

            }

        }

        heightMap[pos.x, pos.y] = avrgHeight;

    }

    private void SmoothingPoint(Vector2Int center, int radius)
    {

        foreach (var pos in Brush(center, radius))
        {

            SmoothingColumn(pos);

        }

    }

    private void AddWater()
    {

        float borderMin = 0.54f, borderMax = 0.55f;

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                if (world.Bioms[x, z] == 0)
                {

                    AddWaterColumn(new Vector2Int(x, z), world.WorldAttributes.OceanHeight);

                }
                else
                {

                    Vector2Int pos = new Vector2Int(x, z);

                    float riverNoise = PerlinNoise.Get2DPerlin(world, pos, 1f, world.WorldAttributes.WorldScale);

                    if (borderMin < riverNoise && borderMax > riverNoise)
                    {

                        SetRiverPoint(Vector2Int.FloorToInt(pos));

                    }
                    else if (world.Bioms[x, z] == -1)
                    {

                        world.Bioms[x, z] = 0;

                        SetRiverPoint(Vector2Int.FloorToInt(pos));

                    }

                }

            }

        }

    }

    private void AddWaterColumn(Vector2Int start, int waterHeight)
    {

        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        if (heightMap[start.x, start.y] < waterHeight)
        {

            waterMap[start.x, start.y] = waterHeight - heightMap[start.x, start.y];

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

                if (heightMap[pos.x, pos.y] < waterHeight)
                {

                    world.Bioms[pos.x, pos.y] = 0;

                    waterMap[pos.x, pos.y] = waterHeight - heightMap[pos.x, pos.y];

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

        int minHeight;
        Queue<Vector2Int> checkedRiverColumns;

        CheckRiverColumns(center, out checkedRiverColumns, out minHeight);

        int centerHeight = minHeight - world.WorldAttributes.RiverDepth;

        foreach (var pos in checkedRiverColumns)
        {

            float distance = Vector2Int.Distance(center, pos);

            int height = Mathf.RoundToInt(distance * distance * world.WorldAttributes.RiverBrushScale) + centerHeight;

            if (heightMap[pos.x, pos.y] > height)
            {

                heightMap[pos.x, pos.y] = height;

            }

        }

        foreach (var pos in Brush(center, world.WorldAttributes.RiverBrushRadius + world.WorldAttributes.SmoothingBrushRadius))
        {

            if (!checkedRiverColumns.Contains(pos) && world.Bioms[pos.x, pos.y] != 0)
            {

                SmoothingColumn(pos);

            }

        }

        foreach (var pos in checkedRiverColumns)
        {

            AddWaterColumn(pos, world.WorldAttributes.OceanHeight);

        }

    }

    private void CheckRiverColumns(Vector2Int center, out Queue<Vector2Int> checkedColumns, out int minHeight)
    {

        minHeight = world.WorldAttributes.ChunkHeight;
        checkedColumns = new Queue<Vector2Int>();

        foreach (var pos in Brush(center, world.WorldAttributes.RiverBrushRadius))
        {

            checkedColumns.Enqueue(pos);

            if (minHeight > heightMap[pos.x, pos.y] + waterMap[pos.x, pos.y])
            {

                minHeight = heightMap[pos.x, pos.y] + waterMap[pos.x, pos.y];

            }

        }

    }

    private void BuildTerrain()
    {

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                Vector2Int pos = new Vector2Int(x, z);

                Vector2Int ChunkCoord = world.GetChunkCoord(pos);
                Vector2Int InChunkCoord = world.GetInChunkCoord(pos);

                for (int y = heightMap[x, z] + waterMap[x, z]; y > 0 && y > heightMap[x, z]; --y)
                {

                    world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] = 9;

                }

                for (int y = heightMap[x, z]; y > 0; --y)
                {

                    world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] = 2;

                }

                world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, 0, InChunkCoord.y] = 1;

            }

        }

    }

    private void AddSoil()
    {

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                int biome = world.Bioms[x, z];

                AddSoilInColumn(new Vector2Int(x, z), world.WorldAttributes.BiomeAttributes[biome]);

            }

        }

    }

    private void AddSoilInColumn(Vector2Int pos, BiomeAttributes biome)
    {

        Vector2Int ChunkCoord = world.GetChunkCoord(pos);
        Vector2Int InChunkCoord = world.GetInChunkCoord(pos);

        int y;

        int terrainHeight = heightMap[pos.x, pos.y];

        if (terrainHeight >= world.WorldAttributes.ChunkHeight || terrainHeight < 0)
        {

            Debug.Log(pos.x + " " + pos.y + " " + heightMap[pos.x, pos.y]);

        }

        world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, terrainHeight, InChunkCoord.y] = biome.MainVoxel;

        int groundDepth = Random.Range(biome.GroundDepthMin, biome.GroundDepthMax + 1);

        for (y = heightMap[pos.x, pos.y] - 1; y > heightMap[pos.x, pos.y] - groundDepth && y > 0; --y)
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

}