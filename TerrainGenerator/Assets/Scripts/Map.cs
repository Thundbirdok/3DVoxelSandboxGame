using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
	[SerializeField]
	private RenderTexture MapTexture;

	[SerializeField]
	private Texture2D src;

	// Start is called before the first frame update
	void Start()
	{
		//
		int WorldSizeInBlocks = 4096;
		int BlocksInAtlas = 4;
		//

		int widthOfBlockInSrc = src.width / BlocksInAtlas;
		int heightOfBlockInSrc = src.height / BlocksInAtlas;

		int pixelsInBlockInSrc = widthOfBlockInSrc * heightOfBlockInSrc;

		Color[] srcPixels = src.GetPixels();
		Color[] MapColors = new Color[BlocksInAtlas * BlocksInAtlas];

		for (int x = 0; x < BlocksInAtlas; ++x)
		{

			for (int y = 0; y < BlocksInAtlas; ++y)
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

				MapColors[x + y * BlocksInAtlas] = new Color(r / pixelsInBlockInSrc, g / pixelsInBlockInSrc, b / pixelsInBlockInSrc);

			}

		}

		Texture2D texRef = new Texture2D(MapTexture.width, MapTexture.height);

		texRef.wrapMode = TextureWrapMode.Clamp;
		texRef.filterMode = FilterMode.Point;

		int widthOfBlockInMap = MapTexture.width / WorldSizeInBlocks;//если размер мира больше чем размер карты, то необходимо брать средний цвет от пикселей по кратности
		int heightOfBlockInMap = MapTexture.height / WorldSizeInBlocks;		

		Color[] MapPixels = new Color[MapTexture.width * MapTexture.height];

		for (int x = 0; x < WorldSizeInBlocks; ++x)
		{

			for (int y = 0; y < WorldSizeInBlocks; ++y)
			{

				Color c = MapColors[x % 4 + (y % 4) * 4];

				int n = y * heightOfBlockInMap * MapTexture.width + x * widthOfBlockInMap;

				for (int k = 0; k < widthOfBlockInMap; ++k)
				{

					for (int m = 0; m < heightOfBlockInMap; ++m)
					{

						MapPixels[n + k + m * MapTexture.width] = c;

					}

				}

			}

		}

		texRef.SetPixels(MapPixels);

		texRef.Apply();

		Graphics.Blit(texRef, MapTexture);

	}

}
