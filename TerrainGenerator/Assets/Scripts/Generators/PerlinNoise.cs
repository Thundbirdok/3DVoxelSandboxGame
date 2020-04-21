using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : IWorldGenerator
{

    private static class Noise
    {

        public static float Get2DPerlin(World world, Vector2 pos, float offset, float scale)
        {

            return Mathf.PerlinNoise((pos.x + 0.1f) / world.WorldAttributes.ChunkWidth * scale + offset, (pos.y + 0.1f) / world.WorldAttributes.ChunkWidth * scale + offset);

        }

        public static bool Get3DPerlin(Vector3 position, float offset, float scale, float threshold)
        {

            float x = (position.x + offset + 0.1f) * scale;
            float y = (position.y + offset + 0.1f) * scale;
            float z = (position.z + offset + 0.1f) * scale;

            float AB = Mathf.PerlinNoise(x, y);
            float BC = Mathf.PerlinNoise(y, z);
            float AC = Mathf.PerlinNoise(x, z);
            float BA = Mathf.PerlinNoise(y, x);
            float CB = Mathf.PerlinNoise(z, y);
            float CA = Mathf.PerlinNoise(z, x);

            if ((AB + BC + AC + BA + CB + CA) / 6f > threshold)
                return true;
            else
                return false;

        }

    }

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

                float noise = Noise.Get2DPerlin(world, ColumnPos, 0.0f, biome.BiomeScale);

                if (noise < 0.5)
                {

                    world.Bioms[x, z] = 0;

                    GetOceanColumn(ColumnPos, noise, world, biome);

                }
                else
                {

                    float borderNoise = Noise.Get2DPerlin(world, new Vector2Int(x, z), 1f, world.WorldAttributes.WorldScale);

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

    private void GetOceanColumn(Vector2Int ColumnPos, float noise, World world, BiomeAttributes biome)
    {

        int terrainHeight = biome.SolidGroundHeight + Mathf.FloorToInt(biome.BiomeHeight * noise);

        for (int i = 1; i < biome.OctavesNumber; ++i)
        {

            terrainHeight -= Mathf.FloorToInt((1 / Mathf.Pow(2, i)) * biome.BiomeHeight * Noise.Get2DPerlin(world, ColumnPos, i, biome.BiomeScale * Mathf.Pow(2, i)));

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

                    GetColumn(new Vector2Int(x, z), world, world.WorldAttributes.BiomeAttributes[biome]);

                }

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

    private void GetColumn(Vector2Int ColumnPos, World world, BiomeAttributes biome)
    {

        int terrainHeight = GetTerrainHeight(ColumnPos, world, biome);

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

                if (world.Bioms[x, z] == 0)
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

            return world.GetTopBlockHeight(pos);


        }
        else
        {

            return 0;

        }

    }

    private void AddWater(World world)
    {

        BiomeAttributes biome = world.WorldAttributes.BiomeAttributes[0];

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                if (world.Bioms[x, z] == 0)
                {

                    Vector2 pos = new Vector2(x, z);
                    Vector2Int ChunkCoord = world.GetChunkCoord(pos);
                    Vector2Int InChunkCoord = world.GetInChunkCoord(pos);

                    for (int y = biome.SolidGroundHeight + biome.BiomeHeight; y > 0 && world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] == 0; --y)
                    {

                        world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] = 9;

                    }

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

                if (biome >= 0)
                {

                    AddSoilInColumn(new Vector2(x, z), world, world.WorldAttributes.BiomeAttributes[biome]);

                }

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