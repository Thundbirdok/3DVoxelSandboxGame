using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
	[SerializeField]
	private RenderTexture MapTexture;

	[SerializeField]
	private Texture2D src;

	[SerializeField]
	private World world;

	private Color[] MapColors;

	// Start is called before the first frame update
	void Start()
	{

		CreateMap();

	}

	public void CreateMap()
	{

		GetMapColors();

		GetMapTexture();

	}

	private void GetMapTexture()
	{

		Texture2D texRef = new Texture2D(MapTexture.width, MapTexture.height)
		{
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Point
		};

		Color[] MapPixels = new Color[MapTexture.width * MapTexture.height];

		int widthOfBlockInMap;        

		if (world.WorldAttributes.WorldSizeInBlocks <= MapTexture.width)
		{

			widthOfBlockInMap = MapTexture.width / world.WorldAttributes.WorldSizeInBlocks;            

			for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
			{

				for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
				{

					Color c = GetTopBlockColor(new Vector2Int(x, z));

					int n = z * widthOfBlockInMap * MapTexture.width + x * widthOfBlockInMap;

					for (int k = 0; k < widthOfBlockInMap; ++k)
					{

						for (int m = 0; m < widthOfBlockInMap; ++m)
						{

							MapPixels[n + k + m * MapTexture.width] = c;

						}

					}

				}

			}

		}
		else
		{

			int widthOfPixelInMap = world.WorldAttributes.WorldSizeInBlocks / MapTexture.width;

			int blocksInPixelInMap = widthOfPixelInMap * widthOfPixelInMap;

			for (int x = 0; x < MapTexture.width; ++x)
			{

				for (int y = 0; y < MapTexture.height; ++y)
				{

					float r = 0;
					float g = 0;
					float b = 0;                    

					for (int k = 0; k < widthOfPixelInMap; ++k)
					{

						for (int m = 0; m < widthOfPixelInMap; ++m)
						{

							Color c = GetTopBlockColor(new Vector2Int(x * widthOfPixelInMap + k, y * widthOfPixelInMap + m));

							r += c.r;
							g += c.g;
							b += c.b;

						}

					}

					MapPixels[y * MapTexture.width + x] = new Color(r / blocksInPixelInMap, g / blocksInPixelInMap, b / blocksInPixelInMap);

				}

			}

		}

		texRef.SetPixels(MapPixels);

		texRef.Apply();

		Graphics.Blit(texRef, MapTexture);

	}

	private Color GetTopBlockColor(Vector2Int coord)
	{

		if (!world.IsVoxelInWorld(coord))
		{
			return Color.black;
		}

		for (int y = world.WorldAttributes.ChunkHeight - 1; y >= 0; --y)
		{

			int blockIndex = world.Chunks[coord.x / world.WorldAttributes.ChunkWidth,
				coord.y / world.WorldAttributes.ChunkWidth].Voxels[coord.x % world.WorldAttributes.ChunkWidth,
				y, coord.y % world.WorldAttributes.ChunkWidth];

			if (world.BlocksAttributes.Blocktypes[blockIndex].IsSolid)
			{

				return MapColors[world.BlocksAttributes.Blocktypes[blockIndex].TopFaceTexture];

			}

		}

		return Color.black;

	}

	private void GetMapColors()
	{

		int widthOfBlockInSrc = src.width / world.BlocksAttributes.TextureAtlasSizeInBlocks;
		int heightOfBlockInSrc = src.height / world.BlocksAttributes.TextureAtlasSizeInBlocks;

		int pixelsInBlockInSrc = widthOfBlockInSrc * heightOfBlockInSrc;

		Color[] srcPixels = src.GetPixels();

		MapColors = new Color[world.BlocksAttributes.TextureAtlasSizeInBlocks * world.BlocksAttributes.TextureAtlasSizeInBlocks];

		for (int y = world.BlocksAttributes.TextureAtlasSizeInBlocks - 1; y >= 0; --y)
		{

			for (int x = 0; x < world.BlocksAttributes.TextureAtlasSizeInBlocks; ++x)
			{

				float r = 0;
				float g = 0;
				float b = 0;

				int n = y * heightOfBlockInSrc * src.width + x * widthOfBlockInSrc;

				for (int k = 0; k < widthOfBlockInSrc; ++k)
				{

					for (int m = 0; m < heightOfBlockInSrc; ++m)
					{

						Color c = srcPixels[n + k + m * src.width];

						r += c.r;
						g += c.g;
						b += c.b;

					}

				}

				Color color = new Color(r / pixelsInBlockInSrc, g / pixelsInBlockInSrc, b / pixelsInBlockInSrc);
				MapColors[x + (world.BlocksAttributes.TextureAtlasSizeInBlocks - 1 - y)
					* world.BlocksAttributes.TextureAtlasSizeInBlocks] = color;

			}

		}

	}

}
