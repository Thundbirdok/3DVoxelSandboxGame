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

        world.UpdateChunks();

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

    public void InitChunks(World world)
    {

        for (int x = 0; x < world.WorldAttributes.WorldSizeInChunks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInChunks; ++z)
            {

                world.CreateChunk(new Vector2Int(x, z));

            }

        }

    }

    public void SetBioms(World world)
    {

        float borderMin = 0.6f, borderMax = 0.65f;

        BiomeAttributes biome = world.WorldAttributes.BiomeAttributes[0];

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                Vector2Int ColumnPos = new Vector2Int(x, z);

                float noise = Noise.Get2DPerlin(world, ColumnPos, 0.0f, world.WorldAttributes.WorldScale /*biome.BiomeScale*/);

                if (noise < 0.5)
                {

                    world.Bioms[x, z] = 0;

                    int terrainHeight = biome.SolidGroundHeight + Mathf.FloorToInt(biome.BiomeHeight * noise);

                    for (int i = 1; i < biome.OctavesNumber; ++i)
                    {

                        terrainHeight -= Mathf.FloorToInt((1 / Mathf.Pow(2, i)) * biome.BiomeHeight * Noise.Get2DPerlin(world, ColumnPos, i, biome.BiomeScale * Mathf.Pow(2, i)));

                    }

                    Vector2Int ChunkPos = new Vector2Int(x / world.WorldAttributes.ChunkWidth, z / world.WorldAttributes.ChunkWidth);
                    Vector2Int InChunkPos = new Vector2Int(x % world.WorldAttributes.ChunkWidth, z % world.WorldAttributes.ChunkWidth);

                    int y;

                    for (y = biome.SolidGroundHeight + biome.BiomeHeight; y > terrainHeight; --y)
                    {

                        world.Chunks[ChunkPos.x, ChunkPos.y].voxelMap[InChunkPos.x, y, InChunkPos.y] = 9;

                    }

                    world.Chunks[ChunkPos.x, ChunkPos.y].voxelMap[InChunkPos.x, terrainHeight, InChunkPos.y] = 6;

                    int groundDepth = UnityEngine.Random.Range(biome.GroundDepthMin, biome.GroundDepthMax + 1);

                    for (y = terrainHeight - 1; y > terrainHeight - groundDepth; --y)
                    {

                        world.Chunks[ChunkPos.x, ChunkPos.y].voxelMap[InChunkPos.x, y, InChunkPos.y] = 6;

                    }

                    for (; y > 0; --y)
                    {

                        world.Chunks[ChunkPos.x, ChunkPos.y].voxelMap[InChunkPos.x, y, InChunkPos.y] = 2;

                    }

                    world.Chunks[ChunkPos.x, ChunkPos.y].voxelMap[InChunkPos.x, 0, InChunkPos.y] = 1;

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

    private void FillBorder(World world, Vector2Int start, int biome)
    {

        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        stack.Push(start);

        while (stack.Count != 0)
        {

            Vector2Int pos = stack.Pop();

            if (pos.x >= 0 && pos.x < world.WorldAttributes.WorldSizeInBlocks && pos.y >= 0 && pos.y < world.WorldAttributes.WorldSizeInBlocks)
            {

                if (world.Bioms[pos.x, pos.y] == -2)
                {

                    world.Bioms[pos.x, pos.y] = biome;

                    stack.Push(new Vector2Int(pos.x - 1, pos.y));
                    stack.Push(new Vector2Int(pos.x + 1, pos.y));
                    stack.Push(new Vector2Int(pos.x, pos.y - 1));
                    stack.Push(new Vector2Int(pos.x, pos.y + 1));

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

        Vector2Int ChunkPos = world.GetChunkCoord(ColumnPos);
        Vector2Int InChunkPos = world.GetInChunkCoord(ColumnPos);

        int y;

        world.Chunks[ChunkPos.x, ChunkPos.y].voxelMap[InChunkPos.x, terrainHeight, InChunkPos.y] = biome.MainVoxel;

        int groundDepth = UnityEngine.Random.Range(biome.GroundDepthMin, biome.GroundDepthMax + 1);

        for (y = terrainHeight - 1; y > terrainHeight - groundDepth; --y)
        {

            world.Chunks[ChunkPos.x, ChunkPos.y].voxelMap[InChunkPos.x, y, InChunkPos.y] = biome.SecondVoxel;

        }

        for (; y > 0; --y)
        {

            world.Chunks[ChunkPos.x, ChunkPos.y].voxelMap[InChunkPos.x, y, InChunkPos.y] = 2;

        }

        world.Chunks[ChunkPos.x, ChunkPos.y].voxelMap[InChunkPos.x, 0, InChunkPos.y] = 1;

    }

}