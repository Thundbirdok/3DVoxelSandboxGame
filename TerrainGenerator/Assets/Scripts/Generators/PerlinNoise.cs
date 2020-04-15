using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : IWorldGenerator
{

    public class BiomeAttributes
    {

        public int solidGroundHeight;
        public int terrainHeight;
        public float terrainScale;

    }

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

        BiomeAttributes Plain = new BiomeAttributes();
        
        Plain.solidGroundHeight = world.WorldAttributes.ChunkHeight / 3;
        Plain.terrainHeight = world.WorldAttributes.ChunkHeight / 2;
        Plain.terrainScale = 0.25f;

        for (int x = 0; x < world.WorldAttributes.WorldSizeInChunks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInChunks; ++z)
            {

                world.CreateChunk(new ChunkCoord(x, z));

                for (int x1 = 0; x1 < world.WorldAttributes.ChunkWidth; ++x1)
                {

                    for (int z1 = 0; z1 < world.WorldAttributes.ChunkWidth; ++z1)
                    {

                        for (int y1 = 0; y1 < world.WorldAttributes.ChunkHeight; ++y1)
                        {

                            world.Chunks[x, z].voxelMap[x1, y1, z1] 
                                = GetVoxel(new Vector3(x * world.WorldAttributes.ChunkWidth + x1, 
                                y1, z * world.WorldAttributes.ChunkWidth + z1), world, Plain);

                        }

                    }

                }

                world.Chunks[x, z].Update();

            }

        }            

    }

    public byte GetVoxel(Vector3 pos, World world, BiomeAttributes biome)
    {

        int yPos = Mathf.FloorToInt(pos.y);
        
        if (yPos == 0)
            return 1;        

        int terrainHeight = Mathf.FloorToInt(biome.terrainHeight * Noise.Get2DPerlin(world, new Vector2(pos.x, pos.z), 0, biome.terrainScale)) + biome.solidGroundHeight;
        byte voxelValue;

        if (yPos == terrainHeight)
            voxelValue = 3;
        else if (yPos < terrainHeight && yPos > terrainHeight - 4)
            voxelValue = 4;
        else if (yPos > terrainHeight)
            return 0;
        else
            voxelValue = 2;

        return voxelValue;

    }

}