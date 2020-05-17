using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class RandomHills : IWorldGenerator
{

    void IWorldGenerator.GenerateWorld(World world)
    {

        for (int x = 0; x < world.WorldAttributes.WorldSizeInChunks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInChunks; ++z)
            {

                world.CreateChunk(new Vector2Int(x, z));

            }

        }

        for (int i = 0; i < Mathf.Pow(world.WorldAttributes.WorldSizeInChunks, 2); ++i)
        {

            int x = Random.Range(1, world.WorldAttributes.WorldSizeInBlocks);
            int z = Random.Range(1, world.WorldAttributes.WorldSizeInBlocks);

            int y = Random.Range(1, world.WorldAttributes.ChunkHeight);

            if (y > world.WorldAttributes.ChunkHeight / 2)
            {

                AddColumn(world, new Vector3Int(x, y, z), 3);

                int r = 1;

                do
                {

                    AddRing(world, new Vector3Int(x, y - Random.Range(0, 3), z), r, 3);

                    y -= Random.Range(1, 3);

                    ++r;

                } while (y > world.WorldAttributes.ChunkHeight / 2);

            }
            else if (y < world.WorldAttributes.ChunkHeight / 3)
            {

                AddWaterColumn(world, new Vector3Int(x, y, z), 6, world.WorldAttributes.ChunkHeight / 2);

                int r = 1;

                do
                {

                    AddWaterRing(world, new Vector3Int(x, y + Random.Range(0, 3), z), r, 6, world.WorldAttributes.ChunkHeight / 2);

                    y += Random.Range(1, 3);

                    ++r;

                } while (y <= world.WorldAttributes.ChunkHeight / 2);

            }

        }

        for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
            {

                if (ColumnCheck(world, new Vector2Int(x, z)) == 0)
                {

                    AddColumn(world, new Vector3Int(x, world.WorldAttributes.ChunkHeight / 2, z), 3);

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

    private void AddRing(World world, Vector3Int pos, int r, byte id)
    {

        for (int k = -r; k <= r; ++k)
        {

            int y1 = pos.y - Random.Range(0, 3);

            if (y1 < world.WorldAttributes.ChunkHeight / 2)
            {

                y1 = world.WorldAttributes.ChunkHeight / 2;

            }

            if (pos.x + k >= 0 && pos.x + k < world.WorldAttributes.WorldSizeInBlocks
                && pos.z - r >= 0)
            {

                AddColumn(world, new Vector3Int(pos.x + k, y1, pos.z - r), id);

            }

        }

        for (int k = -r; k <= r; ++k)
        {

            int y1 = pos.y - Random.Range(0, 3);

            if (y1 < world.WorldAttributes.ChunkHeight / 2)
            {

                y1 = world.WorldAttributes.ChunkHeight / 2;

            }

            if (pos.x + k >= 0 && pos.x + k < world.WorldAttributes.WorldSizeInBlocks
                && pos.z + r < world.WorldAttributes.WorldSizeInBlocks)
            {

                AddColumn(world, new Vector3Int(pos.x + k, y1, pos.z + r), id);

            }
        }

        for (int k = -r + 1; k < r; ++k)
        {

            int y1 = pos.y - Random.Range(0, 3);

            if (y1 < world.WorldAttributes.ChunkHeight / 2)
            {

                y1 = world.WorldAttributes.ChunkHeight / 2;

            }

            if (pos.x - r >= 0 && pos.z + k >= 0 && pos.z + k < world.WorldAttributes.WorldSizeInBlocks)
            {

                AddColumn(world, new Vector3Int(pos.x - r, y1, pos.z + k), id);

            }

        }

        for (int k = -r + 1; k < r; ++k)
        {

            int y1 = pos.y - Random.Range(0, 3);

            if (y1 < world.WorldAttributes.ChunkHeight / 2)
            {

                y1 = world.WorldAttributes.ChunkHeight / 2;

            }

            if (pos.z + k >= 0 && pos.z + k < world.WorldAttributes.WorldSizeInBlocks
                && pos.x + r < world.WorldAttributes.WorldSizeInBlocks)
            {

                AddColumn(world, new Vector3Int(pos.x + r, y1, pos.z + k), id);

            }

        }

    }

    private void AddWaterRing(World world, Vector3Int pos, int r, byte id, int waterY)
    {

        for (int k = -r; k <= r; ++k)
        {

            int y1 = pos.y + Random.Range(0, 3);

            if (y1 > waterY)
            {

                y1 = waterY - 1;

            }

            if (pos.x + k >= 0 && pos.x + k < world.WorldAttributes.WorldSizeInBlocks
                && pos.z - r >= 0)
            {

                AddWaterColumn(world, new Vector3Int(pos.x + k, y1, pos.z - r), id, waterY);

            }

        }

        for (int k = -r; k <= r; ++k)
        {

            int y1 = pos.y + Random.Range(0, 3);

            if (y1 > waterY)
            {

                y1 = waterY - 1;

            }

            if (pos.x + k >= 0 && pos.x + k < world.WorldAttributes.WorldSizeInBlocks
                && pos.z + r < world.WorldAttributes.WorldSizeInBlocks)
            {

                AddWaterColumn(world, new Vector3Int(pos.x + k, y1, pos.z + r), id, waterY);

            }

        }

        for (int k = -r + 1; k < r; ++k)
        {

            int y1 = pos.y + Random.Range(0, 3);

            if (y1 > waterY)
            {

                y1 = waterY - 1;

            }

            if (pos.x - r >= 0 && pos.z + k >= 0 && pos.z + k < world.WorldAttributes.WorldSizeInBlocks)
            {

                AddWaterColumn(world, new Vector3Int(pos.x - r, y1, pos.z + k), id, waterY);

            }

        }

        for (int k = -r + 1; k < r; ++k)
        {

            int y1 = pos.y + Random.Range(0, 3);

            if (y1 > waterY)
            {

                y1 = waterY - 1;

            }

            if (pos.z + k >= 0 && pos.z + k < world.WorldAttributes.WorldSizeInBlocks
                && pos.x + r < world.WorldAttributes.WorldSizeInBlocks)
            {

                AddWaterColumn(world, new Vector3Int(pos.x + r, y1, pos.z + k), id, waterY);

            }

        }

    }

    private void AddColumn(World world, Vector3Int pos, byte id)
    {

        Vector2Int chunkCoord = world.GetChunkCoord(new Vector2(pos.x, pos.z));

        int yPr = ColumnCheck(world, new Vector2Int(pos.x, pos.z));

        int xCh = pos.x % world.WorldAttributes.ChunkWidth;
        int zCh = pos.z % world.WorldAttributes.ChunkWidth;

        if (yPr != 0 && pos.y != yPr)
        {

            if (!world.BlocksAttributes.Blocktypes[world.Chunks[chunkCoord.x, chunkCoord.y].Voxels[xCh, yPr, zCh]].isLiquid)
            {

                if (Random.Range(0, 2) > 0)
                {

                    world.Chunks[chunkCoord.x, chunkCoord.y].Voxels[xCh, (pos.y + yPr) / 2, zCh] = id;

                }
                else
                {

                    world.Chunks[chunkCoord.x, chunkCoord.y].Voxels[xCh, (pos.y + yPr) / 2, zCh]
                        = world.Chunks[chunkCoord.x, chunkCoord.x].Voxels[xCh, yPr, zCh];

                }



                if (pos.y > yPr)
                {

                    for (int y1 = (pos.y + yPr) / 2 - 1; y1 >= yPr; --y1)
                    {

                        world.Chunks[chunkCoord.x, chunkCoord.y].Voxels[xCh, y1, zCh] = 2;

                    }

                }
                else if (pos.y < yPr)
                {

                    for (int y1 = (pos.y + yPr) / 2 + 1; y1 <= yPr; ++y1)
                    {

                        world.Chunks[chunkCoord.x, chunkCoord.y].Voxels[xCh, y1, zCh] = 0;

                    }

                }

            }

        }
        else
        {

            world.Chunks[chunkCoord.x, chunkCoord.y].Voxels[xCh, pos.y, zCh] = id;

            for (int y1 = pos.y - 1; y1 > 0; --y1)
            {

                world.Chunks[chunkCoord.x, chunkCoord.y].Voxels[xCh, y1, zCh] = 2;

            }

            world.Chunks[chunkCoord.x, chunkCoord.y].Voxels[xCh, 0, zCh] = 1;

        }

    }

    private void AddWaterColumn(World world, Vector3Int pos, byte id, int waterY)
    {

        Vector2Int chunkCoord = world.GetChunkCoord(new Vector2(pos.x, pos.z));

        int yPr = ColumnCheck(world, new Vector2Int(pos.x, pos.z));

        int xCh = pos.x % world.WorldAttributes.ChunkWidth;
        int zCh = pos.z % world.WorldAttributes.ChunkWidth;

        if (yPr == 0 || pos.y == yPr)
        {

            world.Chunks[chunkCoord.x, chunkCoord.y].Voxels[xCh, pos.y, zCh] = id;

            for (int y1 = pos.y - 1; y1 > 0; --y1)
            {

                world.Chunks[chunkCoord.x, chunkCoord.y].Voxels[xCh, y1, zCh] = 2;

            }

            for (int y1 = pos.y + 1; y1 <= waterY; ++y1)
            {

                world.Chunks[chunkCoord.x, chunkCoord.y].Voxels[xCh, y1, zCh] = 9;

            }

            world.Chunks[chunkCoord.x, chunkCoord.y].Voxels[xCh, 0, zCh] = 1;

        }

    }

    private int ColumnCheck(World world, Vector2Int pos)
    {

        Vector2Int chunkCoord = world.GetChunkCoord(new Vector2(pos.x, pos.y));

        for (int y = world.WorldAttributes.ChunkHeight - 1; y > 0; --y)
        {

            int xCh = pos.x % world.WorldAttributes.ChunkWidth;
            int zCh = pos.y % world.WorldAttributes.ChunkWidth;

            if (world.BlocksAttributes.Blocktypes[world.Chunks[chunkCoord.x, chunkCoord.y].Voxels[xCh, y, zCh]].isSolid)
            {

                return y;

            }

        }

        return 0;

    }

}