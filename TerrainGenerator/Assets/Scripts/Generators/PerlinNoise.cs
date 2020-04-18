using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : IWorldGenerator
{

    private static class Noise
    {

        public static float Get2DPerlin(World world, Vector2 coord, float offset, float scale)
        {

            return Mathf.PerlinNoise((coord.x + 0.1f) / world.WorldAttributes.ChunkWidth * scale + offset, (coord.y + 0.1f) / world.WorldAttributes.ChunkWidth * scale + offset);

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

        for (int x = 0; x < world.WorldAttributes.WorldSizeInChunks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInChunks; ++z)
            {

                world.CreateChunk(new ChunkCoord(x, z));

                for (int x1 = 0; x1 < world.WorldAttributes.ChunkWidth; ++x1)
                {

                    for (int z1 = 0; z1 < world.WorldAttributes.ChunkWidth; ++z1)
                    {

                        GetColumn(new Vector2Int(x, z), new Vector2Int(x1, z1), new Vector2Int(x * world.WorldAttributes.ChunkWidth + x1,
                            z * world.WorldAttributes.ChunkWidth + z1), world, world.WorldAttributes.BiomeAttributes[0]);

                    }

                }

                world.Chunks[x, z].Update();

            }

        }

    }

    private int GetTerrainHeight(Vector2 pos, World world, BiomeAttributes biome)
    {

        int terrainHeight = biome.SolidGroundHeight;

        for (int i = 0; i < biome.OctavesNumber; ++i)
        {

            terrainHeight += Mathf.FloorToInt((1 / Mathf.Pow(2, i)) * biome.BiomeHeight * Noise.Get2DPerlin(world, new Vector2(pos.x, pos.y), i, biome.BiomeScale * Mathf.Pow(2, i)));

        }

        return terrainHeight;

    }

    private void GetColumn(Vector2Int ChunkPos, Vector2Int InChunkPos, Vector2Int ColumnPos, World world, BiomeAttributes biome)
    {

        int terrainHeight = GetTerrainHeight(ColumnPos, world, biome);

        if (terrainHeight >= world.WorldAttributes.ChunkHeight)
        {

            Debug.Log(terrainHeight);

        }

        int y;

        world.Chunks[ChunkPos.x, ChunkPos.y].voxelMap[InChunkPos.x, terrainHeight, InChunkPos.y] = 3;

        int groundDepth = Random.Range(biome.GroundDepthMin, biome.GroundDepthMax + 1);

        for (y = terrainHeight - 1; y > terrainHeight - groundDepth; --y)
        {

            world.Chunks[ChunkPos.x, ChunkPos.y].voxelMap[InChunkPos.x, y, InChunkPos.y] = 4;

        }

        for (; y > 0; --y)
        {

            world.Chunks[ChunkPos.x, ChunkPos.y].voxelMap[InChunkPos.x, y, InChunkPos.y] = 2;

        }

        world.Chunks[ChunkPos.x, ChunkPos.y].voxelMap[InChunkPos.x, 0, InChunkPos.y] = 1;

    }

}