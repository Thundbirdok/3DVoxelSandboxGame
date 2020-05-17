
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DiamondSquare
{

    private static World world;
    private static int Size;
    private static float[,] heightMap;
    private static float[,] Roughness;

    private static int Scale;

    public static float[,] GenerateHeightMap(World _world)
    {

        world = _world;

        Size = world.WorldAttributes.WorldSizeInBlocks + 1;

        InitRoughness();
        InitHeightMap();

        for (Scale = Size / 2; Scale > 0; Scale /= 2)
        {
            for (int x = 0; x < Size - 1; x += Scale)
            {

                for (int z = 0; z < Size - 1; z += Scale)
                {

                    DiamondAndSquare(x, z, x + Scale, z + Scale);

                }

            }

        }

        return heightMap;

    }

    private static void InitHeightMap()
    {

        heightMap = new float[Size, Size];

        heightMap[0, 0] = Roughness[0, 0];
        heightMap[0, Size - 1] = Roughness[0, Size - 1];
        heightMap[Size - 1, Size - 1] = Roughness[Size - 1, Size - 1];
        heightMap[Size - 1, 0] = Roughness[Size - 1, 0];

    }

    private static void InitRoughness()
    {

        Roughness = new float[Size, Size];

        for (int x = 0; x < Size; ++x)
        {

            for (int z = 0; z < Size; ++z)
            {

                if (x < world.WorldAttributes.WorldSizeInBlocks && z < world.WorldAttributes.WorldSizeInBlocks)
                {

                    Roughness[x, z] = world.WorldAttributes.BiomeAttributes[world.Bioms[x, z]].BiomeScale;

                }
                else
                {

                    int x1, z1;

                    if (x == world.WorldAttributes.WorldSizeInBlocks)
                    {

                        x1 = world.WorldAttributes.WorldSizeInBlocks - 1;

                    }
                    else
                    {

                        x1 = x;

                    }

                    if (z == world.WorldAttributes.WorldSizeInBlocks)
                    {

                        z1 = world.WorldAttributes.WorldSizeInBlocks - 1;

                    }
                    else
                    {

                        z1 = z;

                    }

                    Roughness[x, z] = world.WorldAttributes.BiomeAttributes[world.Bioms[x1, z1]].BiomeScale;

                }

            }

        }

    }

    private static void DiamondAndSquare(int lx, int lz, int rx, int rz)
    {

        float l = (float)(rx - lx) / 2;

        Square(lx, lz, rx, rz);

        Diamond(lx, Mathf.FloorToInt(lz + l), l);
        Diamond(rx, Mathf.FloorToInt(rz - l), l);
        Diamond(Mathf.FloorToInt(rx - l), rz, l);
        Diamond(Mathf.FloorToInt(lx + l), lz, l);

    }

    private static void Diamond(int tgx, int tgz, float l)
    {

        float a, b, c, d;

        if (tgz - l >= 0)
        {

            a = heightMap[tgx, Mathf.FloorToInt(tgz - l)];

        }
        else
        {

            a = heightMap[tgx, Mathf.FloorToInt(Size - l)];

        }


        if (tgx - l >= 0)
        {

            b = heightMap[Mathf.FloorToInt(tgx - l), tgz];

        }
        else
        {

            b = heightMap[Mathf.FloorToInt(Size - l), tgz];

        }


        if (tgz + l < Size)
        {

            c = heightMap[tgx, Mathf.FloorToInt(tgz + l)];

        }
        else
        {

            c = heightMap[tgx, Mathf.FloorToInt(l)];

        }

        if (tgx + l < Size)
        {

            d = heightMap[Mathf.FloorToInt(tgx + l), tgz];

        }
        else
        {

            d = heightMap[Mathf.FloorToInt(l), tgz];

        }

        GetHeight(tgx, tgz, a, b, c, d, l);

    }

    private static void Square(int lx, int lz, int rx, int rz)
    {

        float l = (float)(rx - lx) / 2;

        float a = heightMap[lx, lz];
        float b = heightMap[lx, rz];
        float c = heightMap[rx, rz];
        float d = heightMap[rx, lz];
        int cex = Mathf.FloorToInt(lx + l);
        int cez = Mathf.FloorToInt(lz + l);

        GetHeight(cex, cez, a, b, c, d, l);

    }

    private static void GetHeight(int x, int z, float a, float b, float c, float d, float l)
    {

        float border = l / Size * Roughness[x, z] * Scale;

        float height = (a + b + c + d) / 4 + Random.Range((-border), (border));

        if (height > 1)
        {

            height = 1;

        }
        else if (height < 0)
        {

            height = 0;

        }

        heightMap[x, z] = height;

    }

}

