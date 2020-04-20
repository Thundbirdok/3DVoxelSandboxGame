using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class FullRandom : IWorldGenerator
{

    void IWorldGenerator.GenerateWorld(World world)
    {        

        for (int x = 0; x < world.WorldAttributes.WorldSizeInChunks; ++x)
        {

            for (int z = 0; z < world.WorldAttributes.WorldSizeInChunks; ++z)
            {

                world.CreateChunk(new Vector2Int(x, z));

                for (int x1 = 0; x1 < world.WorldAttributes.ChunkWidth; ++x1)
                {

                    for (int z1 = 0; z1 < world.WorldAttributes.ChunkWidth; ++z1)
                    {

                        int y = Random.Range(1, world.WorldAttributes.ChunkHeight);

                        switch (Random.Range(0, 3))
                        {
                            case 0:

                                world.Chunks[x, z].voxelMap[x1, y, z1] = 3;

                                break;

                            case 1:

                                world.Chunks[x, z].voxelMap[x1, y, z1] = 6;

                                break;

                            case 2:

                                world.Chunks[x, z].voxelMap[x1, y, z1] = 9;

                                break;
                        }

                        for (int y1 = y - 1; y1 > 0; --y1)
                        {

                            world.Chunks[x, z].voxelMap[x1, y1, z1] = 2;

                        }

                        world.Chunks[x, z].voxelMap[x1, 0, z1] = 1;

                    }

                }

                world.Chunks[x, z].Update();

            }

        }

    }

}

