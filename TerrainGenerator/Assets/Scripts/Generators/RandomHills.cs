using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class RandomHills : IWorldGenerator
{

    void IWorldGenerator.GenerateWorld(World world)
    {

        System.Random rand = new System.Random();

        for (int x = 0; x < world.WorldAttributes.WorldSizeInChunks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInChunks; ++z)
            {

                world.CreateChunk(new ChunkCoord(x, z));

            }

        }

        for (int i = 0; i < Math.Pow(world.WorldAttributes.WorldSizeInChunks, 2); ++i)
        {

            int x = rand.Next(1, world.WorldAttributes.WorldSizeInBlocks);
            int z = rand.Next(1, world.WorldAttributes.WorldSizeInBlocks);

            int y = rand.Next(1, world.WorldAttributes.ChunkHeight);

            if (y > world.WorldAttributes.ChunkHeight / 2)
            {

                AddColumn(world, rand, x, y, z, 3);

                int r = 1;

                do
                {

                    AddRing(world, rand, x, y - rand.Next(0, 3), z, r, 3);

                    y -= rand.Next(1, 3);

                    ++r;

                } while (y > world.WorldAttributes.ChunkHeight / 2);

            }
            else if (y < world.WorldAttributes.ChunkHeight / 3)
            {

                AddWaterColumn(world, rand, x, y, z, 6, world.WorldAttributes.ChunkHeight / 2);

                int r = 1;

                do
                {

                    AddWaterRing(world, rand, x, y + rand.Next(0, 3), z, r, 6, world.WorldAttributes.ChunkHeight / 2);

                    y += rand.Next(1, 3);

                    ++r;

                } while (y <= world.WorldAttributes.ChunkHeight / 2);

            }

        }

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                if (ColumnCheck(world, x, z) == 0)
                {

                    AddColumn(world, rand, x, world.WorldAttributes.ChunkHeight / 2, z, 3);

                }

            }

        }

        for (int x = 0; x < world.WorldAttributes.WorldSizeInChunks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInChunks; ++z)
            {

                world.Chunks[x, z].Update();

            }

        }

    }

    private void AddRing(World world, System.Random rand, int x, int y, int z, int r, byte id)
    {        

        for (int k = -r; k <= r; ++k)
        {

            int y1 = y - rand.Next(0, 3);

            if (y1 < world.WorldAttributes.ChunkHeight / 2)
            {

                y1 = world.WorldAttributes.ChunkHeight / 2;

            }

            if (x + k >= 0 && x + k < world.WorldAttributes.WorldSizeInBlocks
                && z - r >= 0)
            {

                AddColumn(world, rand, x + k, y1, z - r, id);

            }

        }

        for (int k = -r; k <= r; ++k)
        {

            int y1 = y - rand.Next(0, 3);

            if (y1 < world.WorldAttributes.ChunkHeight / 2)
            {

                y1 = world.WorldAttributes.ChunkHeight / 2;

            }

            if (x + k >= 0 && x + k < world.WorldAttributes.WorldSizeInBlocks
                && z + r < world.WorldAttributes.WorldSizeInBlocks)
            {

                AddColumn(world, rand, x + k, y1, z + r, id);

            }
        }

        for (int k = -r + 1; k < r; ++k)
        {

            int y1 = y - rand.Next(0, 3);

            if (y1 < world.WorldAttributes.ChunkHeight / 2)
            {

                y1 = world.WorldAttributes.ChunkHeight / 2;

            }

            if (x - r >= 0 && z + k >= 0 && z + k < world.WorldAttributes.WorldSizeInBlocks)
            {

                AddColumn(world, rand, x - r, y1, z + k, id);

            }

        }

        for (int k = -r + 1; k < r; ++k)
        {

            int y1 = y - rand.Next(0, 3);

            if (y1 < world.WorldAttributes.ChunkHeight / 2)
            {

                y1 = world.WorldAttributes.ChunkHeight / 2;

            }

            if (z + k >= 0 && z + k < world.WorldAttributes.WorldSizeInBlocks
                && x + r < world.WorldAttributes.WorldSizeInBlocks)
            {

                AddColumn(world, rand, x + r, y1, z + k, id);

            }

        }

    }

    private void AddWaterRing(World world, System.Random rand, int x, int y, int z, int r, byte id, int waterY)
    {        

        for (int k = -r; k <= r; ++k)
        {

            int y1 = y + rand.Next(0, 3);

            if (y1 > waterY)
            {

                y1 = waterY - 1;

            }

            if (x + k >= 0 && x + k < world.WorldAttributes.WorldSizeInBlocks
                && z - r >= 0)
            {

                AddWaterColumn(world, rand, x + k, y1, z - r, id, waterY);

            }

        }

        for (int k = -r; k <= r; ++k)
        {

            int y1 = y + rand.Next(0, 3);

            if (y1 > waterY)
            {

                y1 = waterY - 1;

            }

            if (x + k >= 0 && x + k < world.WorldAttributes.WorldSizeInBlocks
                && z + r < world.WorldAttributes.WorldSizeInBlocks)
            {

                AddWaterColumn(world, rand, x + k, y1, z + r, id, waterY);

            }

        }

        for (int k = -r + 1; k < r; ++k)
        {

            int y1 = y + rand.Next(0, 3);

            if (y1 > waterY)
            {

                y1 = waterY - 1;

            }

            if (x - r >= 0 && z + k >= 0 && z + k < world.WorldAttributes.WorldSizeInBlocks)
            {

                AddWaterColumn(world, rand, x - r, y1, z + k, id, waterY);

            }

        }

        for (int k = -r + 1; k < r; ++k)
        {

            int y1 = y + rand.Next(0, 3);

            if (y1 > waterY)
            {

                y1 = waterY - 1;

            }

            if (z + k >= 0 && z + k < world.WorldAttributes.WorldSizeInBlocks
                && x + r < world.WorldAttributes.WorldSizeInBlocks)
            {

                AddWaterColumn(world, rand, x + r, y1, z + k, id, waterY);

            }

        }

    }

    private void AddColumn(World world, System.Random rand, int x, int y, int z, byte id)
    {

        ChunkCoord chunkCoord = world.GetChunkCoord(x, z);

        int yPr = ColumnCheck(world, x, z);

        int xCh = x % world.WorldAttributes.ChunkWidth;
        int zCh = z % world.WorldAttributes.ChunkWidth;

        if (yPr != 0 && y != yPr)
        {

            if (rand.Next(0, 2) > 0 && !world.BlocksAttributes.Blocktypes[world.Chunks[chunkCoord.x, chunkCoord.z].voxelMap[xCh, yPr, zCh]].isLiquid)
            {

                world.Chunks[chunkCoord.x, chunkCoord.z].voxelMap[xCh, (y + yPr) / 2, zCh] = id;

            }
            else
            {

                world.Chunks[chunkCoord.x, chunkCoord.z].voxelMap[xCh, (y + yPr) / 2, zCh]
                    = world.Chunks[chunkCoord.x, chunkCoord.x].voxelMap[xCh, yPr, zCh];

            }

            if (y > yPr)
            {

                for (int y1 = (y + yPr) / 2 - 1; y1 >= yPr; --y1)
                {

                    world.Chunks[chunkCoord.x, chunkCoord.z].voxelMap[xCh, y1, zCh] = 2;

                }

            }
            else if (y < yPr)
            {

                for (int y1 = (y + yPr) / 2 + 1; y1 <= yPr; ++y1)
                {

                    world.Chunks[chunkCoord.x, chunkCoord.z].voxelMap[xCh, y1, zCh] = 0;

                }

            }

        }
        else
        {            

            world.Chunks[chunkCoord.x, chunkCoord.z].voxelMap[xCh, y, zCh] = id;

            for (int y1 = y - 1; y1 > 0; --y1)
            {

                world.Chunks[chunkCoord.x, chunkCoord.z].voxelMap[xCh, y1, zCh] = 2;

            }

            world.Chunks[chunkCoord.x, chunkCoord.z].voxelMap[xCh, 0, zCh] = 1;

        }

    }

    private void AddWaterColumn(World world, System.Random rand, int x, int y, int z, byte id, int waterY)
    {

        ChunkCoord chunkCoord = world.GetChunkCoord(x, z);

        int yPr = ColumnCheck(world, x, z);

        int xCh = x % world.WorldAttributes.ChunkWidth;
        int zCh = z % world.WorldAttributes.ChunkWidth;

        if (yPr != 0 && y != yPr)
        {

            if (rand.Next(0, 2) > 0 && world.BlocksAttributes.Blocktypes[world.Chunks[chunkCoord.x, chunkCoord.z].voxelMap[xCh, yPr, zCh]].isLiquid)
            {

                world.Chunks[chunkCoord.x, chunkCoord.z].voxelMap[xCh, (y + yPr) / 2, zCh] = id;

            }
            else
            {

                world.Chunks[chunkCoord.x, chunkCoord.z].voxelMap[xCh, (y + yPr) / 2, zCh]
                    = world.Chunks[chunkCoord.x, chunkCoord.x].voxelMap[xCh, yPr, zCh];

            }

            if (y > yPr)
            {

                for (int y1 = (y + yPr) / 2 - 1; y1 >= yPr; --y1)
                {

                    world.Chunks[chunkCoord.x, chunkCoord.z].voxelMap[xCh, y1, zCh] = 2;

                }

            }
            else if (y < yPr)
            {

                if (world.BlocksAttributes.Blocktypes[world.Chunks[chunkCoord.x, chunkCoord.z].voxelMap[xCh, waterY, zCh]].isLiquid)
                {

                    for (int y1 = (y + yPr) / 2 + 1; y1 <= yPr; ++y1)
                    {

                        world.Chunks[chunkCoord.x, chunkCoord.z].voxelMap[xCh, y1, zCh] = 9;

                    }

                }

            }

        }
        else
        {            

            world.Chunks[chunkCoord.x, chunkCoord.z].voxelMap[xCh, y, zCh] = id;

            for (int y1 = y - 1; y1 > 0; --y1)
            {

                world.Chunks[chunkCoord.x, chunkCoord.z].voxelMap[xCh, y1, zCh] = 2;

            }

            for (int y1 = y + 1; y1 <= waterY; ++y1)
            {

                world.Chunks[chunkCoord.x, chunkCoord.z].voxelMap[xCh, y1, zCh] = 9;

            }

            world.Chunks[chunkCoord.x, chunkCoord.z].voxelMap[xCh, 0, zCh] = 1;

        }

    }

    private int ColumnCheck(World world, int x, int z)
    {

        ChunkCoord chunkCoord = world.GetChunkCoord(x, z);

        for (int y = world.WorldAttributes.ChunkHeight - 1; y > 0; --y)
        {

            int xCh = x % world.WorldAttributes.ChunkWidth;
            int zCh = z % world.WorldAttributes.ChunkWidth;

            if (world.BlocksAttributes.Blocktypes[world.Chunks[chunkCoord.x, chunkCoord.z].voxelMap[xCh, y, zCh]].isSolid)
            {

                return y;

            }

        }

        return 0;

    }

}