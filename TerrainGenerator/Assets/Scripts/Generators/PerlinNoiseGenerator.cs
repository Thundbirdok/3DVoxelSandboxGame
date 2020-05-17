using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseGenerator : IWorldGenerator
{

    void IWorldGenerator.GenerateWorld(World world)
    {

        InitChunks(world);

        SetBioms(world);

        BuildTerrain(world);

        SmoothingCliffs(world);

        AddWater(world);

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

    private void SetBioms(World world)
    {

        float borderMin = 0.6f, borderMax = 0.65f;

        BiomeAttributes biome = world.WorldAttributes.BiomeAttributes[0];

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                Vector2Int ColumnPos = new Vector2Int(x, z);

                float noise = PerlinNoise.Get2DPerlin(world, ColumnPos, 0.0f, biome.BiomeScale);

                if (noise < 0.5)
                {

                    world.Bioms[x, z] = 0;

                    SetOceanColumn(ColumnPos, noise, world, biome);

                }
                else
                {

                    float borderNoise = PerlinNoise.Get2DPerlin(world, new Vector2Int(x, z), 1f, world.WorldAttributes.WorldScale);

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

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                if (world.Bioms[x, z] == -2)
                {

                    FillBorder(world, new Vector2Int(x, z), UnityEngine.Random.Range(1, world.WorldAttributes.BiomeAttributes.Length));

                }

            }

        }

    }

    private void SetOceanColumn(Vector2Int ColumnPos, float noise, World world, BiomeAttributes biome)
    {

        int terrainHeight = biome.SolidGroundHeight + Mathf.FloorToInt(biome.BiomeHeight * noise);

        for (int i = 1; i < biome.OctavesNumber; ++i)
        {

            terrainHeight -= Mathf.FloorToInt((1 / Mathf.Pow(2, i))
                * biome.BiomeHeight
                * PerlinNoise.Get2DPerlin(world, ColumnPos, i, biome.BiomeScale * Mathf.Pow(2, i)));

        }

        Vector2Int ChunkCoord = world.GetChunkCoord(ColumnPos);
        Vector2Int InChunkCoord = world.GetInChunkCoord(ColumnPos);

        int y;

        for (y = terrainHeight; y > 0; --y)
        {

            world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] = 2;

        }

        world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, 0, InChunkCoord.y] = 1;

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

                if (biome != 0)
                {

                    if (biome == -1)
                    {
                        biome = 1;
                    }

                    SetColumn(new Vector2Int(x, z), world, world.WorldAttributes.BiomeAttributes[biome]);

                }

            }

        }

    }

    private int GetTerrainHeight(Vector2 pos, World world, BiomeAttributes biome)
    {

        int terrainHeight = biome.SolidGroundHeight;

        for (int i = 0; i < biome.OctavesNumber; ++i)
        {

            terrainHeight += Mathf.FloorToInt((1 / Mathf.Pow(2, i)) * biome.BiomeHeight * PerlinNoise.Get2DPerlin(world, pos, i, biome.BiomeScale * Mathf.Pow(2, i)));

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

    private void SmoothingCliffs(World world)
    {

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                if (world.Bioms[x, z] == 0 || world.Bioms[x, z] == -1)
                {

                    SmoothingColumn(new Vector2(x, z), world);

                    SmoothingColumn(new Vector2(x - 1, z - 1), world);
                    SmoothingColumn(new Vector2(x + 1, z - 1), world);
                    SmoothingColumn(new Vector2(x - 1, z + 1), world);
                    SmoothingColumn(new Vector2(x + 1, z + 1), world);

                }

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

            return world.GetTopSoilBlockHeight(pos);


        }
        else
        {

            return 0;

        }

    }

    private void AddWater(World world)
    {

        int waterHeight = world.WorldAttributes.BiomeAttributes[0].SolidGroundHeight + world.WorldAttributes.BiomeAttributes[0].BiomeHeight;

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                if (world.Bioms[x, z] == 0 || world.Bioms[x, z] == -1)
                {

                    if (world.Bioms[x, z] == 0)
                    {

                        AddWaterColumn(new Vector2Int(x, z), world, waterHeight);

                    }
                    else if (world.Bioms[x, z] == -1)
                    {

                        SetRiverPart(new Vector2Int(x, z), world);

                    }

                }

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

            if (world.IsVoxelInWorld(pos) && world.Bioms[pos.x, pos.y] != 0 && world.Bioms[pos.x, pos.y] != -1)
            {

                if (world.GetTopSoilBlockHeight(pos) < waterHeight)
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

    private void SetRiverPart(Vector2Int start, World world)
    {

        int avrgY = 0;
        int counter = world.WorldAttributes.RiverPart;

        Queue<Vector2Int> columns = new Queue<Vector2Int>();
        Queue<Vector2Int> checkedColumns = new Queue<Vector2Int>();

        columns.Enqueue(start);

        while (columns.Count != 0 && counter != 0)
        {

            Vector2Int pos = columns.Dequeue();

            if (world.IsVoxelInWorld(pos) && world.Bioms[pos.x, pos.y] == -1)
            {

                --counter;

                checkedColumns.Enqueue(pos);

                world.Bioms[pos.x, pos.y] = -2;

                avrgY += world.GetTopSoilBlockHeight(pos);

                columns.Enqueue(new Vector2Int(pos.x - 1, pos.y));
                columns.Enqueue(new Vector2Int(pos.x + 1, pos.y));
                columns.Enqueue(new Vector2Int(pos.x, pos.y - 1));
                columns.Enqueue(new Vector2Int(pos.x, pos.y + 1));
                columns.Enqueue(new Vector2Int(pos.x - 1, pos.y - 1));
                columns.Enqueue(new Vector2Int(pos.x - 1, pos.y + 1));
                columns.Enqueue(new Vector2Int(pos.x + 1, pos.y - 1));
                columns.Enqueue(new Vector2Int(pos.x + 1, pos.y + 1));
            }

        }

        avrgY /= world.WorldAttributes.RiverPart;

        while (checkedColumns.Count != 0)
        {

            Vector2Int pos = checkedColumns.Dequeue();

            Vector2Int ChunkCoord = world.GetChunkCoord(pos);
            Vector2Int InChunkCoord = world.GetInChunkCoord(pos);

            int blockHeight = GetTopBlockHeight(pos, world);

            if (blockHeight >= avrgY + world.WorldAttributes.RiverDepth)
            {

                world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, blockHeight + 1, InChunkCoord.y] = 9;

            }
            else
            {

                for (int y = avrgY + world.WorldAttributes.RiverDepth; y > 0 && world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] == 0; --y)
                {

                    world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] = 9;

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

        int terrainHeight = world.GetTopSoilBlockHeight(ColumnPos);

        world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, terrainHeight, InChunkCoord.y] = biome.MainVoxel;

        int groundDepth = UnityEngine.Random.Range(biome.GroundDepthMin, biome.GroundDepthMax + 1);

        for (y = terrainHeight - 1; y > terrainHeight - groundDepth && y > 0; --y)
        {

            world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] = biome.SecondVoxel;

        }

    }

}