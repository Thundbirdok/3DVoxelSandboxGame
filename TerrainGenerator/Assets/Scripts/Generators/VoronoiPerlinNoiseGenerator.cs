using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class VoronoiPerlinNoiseGenerator : IWorldGenerator
{

	World world;

	int[,] heightMap;
	int[,] waterMap;

	void IWorldGenerator.GenerateWorld(World _world)
	{

		world = _world;

		Init();

		List<VoronoiDiagram.GraphEdge> Edges = SetBioms();

		SetHeightMap();

		SmoothingBorders(Edges);

		AddWater(Edges);

		BuildTerrain();

		AddSoil();

		AddTrees();

		world.UpdateChunks();

	}

	private void Init()
	{

		waterMap = new int[world.WorldAttributes.WorldSizeInBlocks, world.WorldAttributes.WorldSizeInBlocks];

		for (int x = 0; x < world.WorldAttributes.WorldSizeInChunks; ++x)
		{

			for (int z = 0; z < world.WorldAttributes.WorldSizeInChunks; ++z)
			{

				world.CreateChunk(new Vector2Int(x, z));

				waterMap[x, z] = 0;

			}

		}

	}

	private List<VoronoiDiagram.GraphEdge> SetBioms()
	{

		List<VoronoiDiagram.GraphEdge> Edges = SetVoronoiDiagram();

		PutBioms();

		HideEdges(Edges);

		return Edges;

	}

	private void HideEdges(List<VoronoiDiagram.GraphEdge> Edges)
	{

		foreach (var edge in Edges)
		{

			int biomeA, biomeB;

			GetBorderingBioms(edge, out biomeA, out biomeB);

			if (biomeA == biomeB && biomeA != -1)
			{

				DrawLine(edge, biomeA);

			}
			else
			{

				if (biomeA != -1)
				{

					DrawLine(edge, biomeA);

				}
				else if (biomeB != -1)
				{

					DrawLine(edge, biomeB);

				}
				else
				{

					DrawLine(edge, 0);

				}

			}

		}

	}

	private List<VoronoiDiagram.GraphEdge> SetVoronoiDiagram()
	{

		InitBioms(world);

		VoronoiDiagram voronoi = new VoronoiDiagram(world.WorldAttributes.SitesMinDistance);

		double[] xVal = new double[world.WorldAttributes.SitesNumber];
		double[] yVal = new double[world.WorldAttributes.SitesNumber];

		for (int i = 0; i < world.WorldAttributes.SitesNumber; ++i)
		{

			xVal[i] = Random.Range(0, world.WorldAttributes.WorldSizeInBlocks);
			yVal[i] = Random.Range(0, world.WorldAttributes.WorldSizeInBlocks);

		}

		List<VoronoiDiagram.GraphEdge> Edges = voronoi.GenerateDiagram(xVal, yVal, 0, world.WorldAttributes.WorldSizeInBlocks, 0, world.WorldAttributes.WorldSizeInBlocks);

		List<VoronoiDiagram.GraphEdge> ClearedEdges = new List<VoronoiDiagram.GraphEdge>();

		for (int i = 0; i < Edges.Count; ++i)
		{

			if (Edges[i].x1 != Edges[i].x2 || Edges[i].y1 != Edges[i].y2)
			{

				ClearedEdges.Add(Edges[i]);

			}

		}

		foreach (var edge in ClearedEdges)
		{

			DrawLine(edge, -1);

		}

		return ClearedEdges;

	}

	private void InitBioms(World world)
	{

		for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
		{

			for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
			{

				world.Bioms[x, z] = -2;

			}

		}

	}

	private void DrawLine(VoronoiDiagram.GraphEdge edge, int biome)
	{

		Vector2 A = new Vector2((float)edge.x1, (float)edge.y1);
		Vector2 B = new Vector2((float)edge.x2, (float)edge.y2);

		do
		{

			DrawPoint(Vector2Int.FloorToInt(A), world.WorldAttributes.BoarderBrushRadius, biome);

			A = Vector2.MoveTowards(A, B, 1f);

		} while (A != B);

	}

	private void DrawPoint(Vector2Int center, int radius, int biome)
	{

		foreach (var point in Brush(center, radius))
		{

			if (world.IsVoxelInWorld(point))
			{

				world.Bioms[point.x, point.y] = biome;

			}

		}

	}

	private void PutBioms()
	{

		for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
		{

			for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
			{

				if (world.Bioms[x, z] == -2)
				{

					FillBorder(new Vector2Int(x, z), Random.Range(0, world.WorldAttributes.BiomeAttributes.Length));

				}

			}

		}

	}

	private void FillBorder(Vector2Int start, int biome)
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
				stack.Push(new Vector2Int(pos.x - 1, pos.y - 1));
				stack.Push(new Vector2Int(pos.x - 1, pos.y + 1));
				stack.Push(new Vector2Int(pos.x + 1, pos.y - 1));
				stack.Push(new Vector2Int(pos.x + 1, pos.y + 1));

			}

		}

	}

	private void SetHeightMap()
	{

		heightMap = new int[world.WorldAttributes.WorldSizeInBlocks, world.WorldAttributes.WorldSizeInBlocks];

		for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
		{

			for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
			{

				heightMap[x, z] = GetTerrainHeight(new Vector2Int(x, z));

			}

		}

	}

	private int GetTerrainHeight(Vector2Int pos)
	{

		BiomeAttributes biome = world.WorldAttributes.BiomeAttributes[world.Bioms[pos.x, pos.y]];

		int terrainHeight = biome.SolidGroundHeight;

		for (int i = 0; i < biome.OctavesNumber; ++i)
		{

			terrainHeight += Mathf.FloorToInt((1 / Mathf.Pow(2, i)) * biome.BiomeHeight * PerlinNoise.Get2DPerlin(world, pos, i, biome.BiomeScale * Mathf.Pow(2, i)));

		}

		return terrainHeight;

	}

	private void SmoothingBorders(List<VoronoiDiagram.GraphEdge> Edges)
	{

		foreach (var edge in Edges)
		{

			int biomeA, biomeB;

			GetBorderingBioms(edge, out biomeA, out biomeB);

			if (biomeA != biomeB && (biomeA == 0 || biomeB == 0))
			{

				Vector2 A = new Vector2((float)edge.x1, (float)edge.y1);
				Vector2 B = new Vector2((float)edge.x2, (float)edge.y2);

				do
				{

					SmoothingPoint(Vector2Int.FloorToInt(A), world.WorldAttributes.SmoothingBrushRadius + world.WorldAttributes.BoarderBrushRadius);

					A = Vector2.MoveTowards(A, B, 1f);

				} while (A != B);

			}

		}

	}

	private void SmoothingColumn(Vector2Int pos)
	{

		if (!world.IsVoxelInWorld(pos))
		{

			return;

		}

		int avrgHeight = 0;
		int count = 0;

		foreach (var column in Brush(pos, world.WorldAttributes.SmoothingCheckBrushRadius))
		{

			avrgHeight += heightMap[column.x, column.y];
			++count;

		}

		avrgHeight = Mathf.RoundToInt((float)avrgHeight / count);

		if (waterMap[pos.x, pos.y] != 0)
		{

			waterMap[pos.x, pos.y] += heightMap[pos.x, pos.y] - avrgHeight;

			if (waterMap[pos.x, pos.y] < 0)
			{

				waterMap[pos.x, pos.y] = 0;

			}

		}
		else if (avrgHeight < heightMap[pos.x, pos.y])
		{

			if (world.IsVoxelInWorld(new Vector2Int(pos.x - 1, pos.y)) && avrgHeight < heightMap[pos.x - 1, pos.y] + waterMap[pos.x - 1, pos.y] && waterMap[pos.x - 1, pos.y] != 0)
			{

				waterMap[pos.x, pos.y] = heightMap[pos.x - 1, pos.y] + waterMap[pos.x - 1, pos.y] - avrgHeight;

				world.Bioms[pos.x, pos.y] = 0;

			}
			else if (world.IsVoxelInWorld(new Vector2Int(pos.x + 1, pos.y)) && avrgHeight < heightMap[pos.x + 1, pos.y] + waterMap[pos.x + 1, pos.y] && waterMap[pos.x + 1, pos.y] != 0)
			{

				waterMap[pos.x, pos.y] = heightMap[pos.x + 1, pos.y] + waterMap[pos.x + 1, pos.y] - avrgHeight;

				world.Bioms[pos.x, pos.y] = 0;

			}
			else if (world.IsVoxelInWorld(new Vector2Int(pos.x, pos.y - 1)) && avrgHeight < heightMap[pos.x, pos.y - 1] + waterMap[pos.x, pos.y - 1] && waterMap[pos.x, pos.y - 1] != 0)
			{

				waterMap[pos.x, pos.y] = heightMap[pos.x, pos.y - 1] + waterMap[pos.x, pos.y - 1] - avrgHeight;

				world.Bioms[pos.x, pos.y] = 0;

			}
			else if (world.IsVoxelInWorld(new Vector2Int(pos.x, pos.y + 1)) && avrgHeight < heightMap[pos.x, pos.y + 1] + waterMap[pos.x, pos.y + 1] && waterMap[pos.x, pos.y + 1] != 0)
			{

				waterMap[pos.x, pos.y] = heightMap[pos.x, pos.y + 1] + waterMap[pos.x, pos.y + 1] - avrgHeight;

				world.Bioms[pos.x, pos.y] = 0;

			}

		}

		heightMap[pos.x, pos.y] = avrgHeight;

	}

	private void SmoothingPoint(Vector2Int center, int radius)
	{

		foreach (var pos in Brush(center, radius))
		{

			SmoothingColumn(pos);

		}

	}

	private void AddWater(List<VoronoiDiagram.GraphEdge> Edges)
	{

		foreach (var edge in Edges)
		{

			int biomeA, biomeB;

			GetBorderingBioms(edge, out biomeA, out biomeB);

			if (biomeA != biomeB && biomeA != 0 && biomeB != 0)
			{

				Vector2 begin = new Vector2((float)edge.x1, (float)edge.y1);
				Vector2 end = new Vector2((float)edge.x2, (float)edge.y2);

				do
				{

					SetRiverPoint(Vector2Int.FloorToInt(begin));

					begin = Vector2.MoveTowards(begin, end, 1f);

				} while (begin != end);

			}

		}

		for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
		{

			for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
			{

				if (world.Bioms[x, z] == 0)
				{

					AddWaterColumn(new Vector2Int(x, z), world.WorldAttributes.OceanHeight);

				}
				else
				{

					Vector2Int pos = new Vector2Int(x, z);

					float riverNoise = PerlinNoise.Get2DPerlin(world, pos, 1f, world.WorldAttributes.WorldScale);

					if (0.54f < riverNoise && 0.55f > riverNoise)
					{

						SetRiverPoint(Vector2Int.FloorToInt(pos));

					}

				}

			}

		}

	}

	private void AddWaterColumn(Vector2Int start, int waterHeight)
	{

		Stack<Vector2Int> stack = new Stack<Vector2Int>();

		if (heightMap[start.x, start.y] < waterHeight)
		{

			waterMap[start.x, start.y] = waterHeight - heightMap[start.x, start.y];

		}

		stack.Push(new Vector2Int(start.x - 1, start.y));
		stack.Push(new Vector2Int(start.x + 1, start.y));
		stack.Push(new Vector2Int(start.x, start.y - 1));
		stack.Push(new Vector2Int(start.x, start.y + 1));

		while (stack.Count != 0)
		{

			Vector2Int pos = stack.Pop();

			if (world.IsVoxelInWorld(pos) && world.Bioms[pos.x, pos.y] != 0)
			{

				if (heightMap[pos.x, pos.y] < waterHeight)
				{

					world.Bioms[pos.x, pos.y] = 0;

					waterMap[pos.x, pos.y] = waterHeight - heightMap[pos.x, pos.y];

					stack.Push(new Vector2Int(pos.x - 1, pos.y));
					stack.Push(new Vector2Int(pos.x + 1, pos.y));
					stack.Push(new Vector2Int(pos.x, pos.y - 1));
					stack.Push(new Vector2Int(pos.x, pos.y + 1));

				}

			}

		}

	}

	private void SetRiverPoint(Vector2Int center)
	{

		if (!world.IsVoxelInWorld(center))
		{

			return;

		}

		int minHeight;
		Queue<Vector2Int> checkedRiverColumns;

		CheckRiverColumns(center, out checkedRiverColumns, out minHeight);

		int centerHeight = minHeight - world.WorldAttributes.RiverDepth;

		foreach (var pos in checkedRiverColumns)
		{

			float distance = Vector2Int.Distance(center, pos);

			int height = Mathf.RoundToInt(distance * distance * world.WorldAttributes.RiverBrushScale) + centerHeight;

			if (heightMap[pos.x, pos.y] > height)
			{

				heightMap[pos.x, pos.y] = height;

			}

		}

		foreach (var pos in Brush(center, world.WorldAttributes.RiverBrushRadius + world.WorldAttributes.SmoothingBrushRadius))
		{

			if (!checkedRiverColumns.Contains(pos) && world.Bioms[pos.x, pos.y] != 0)
			{

				SmoothingColumn(pos);

			}

		}

		foreach (var pos in checkedRiverColumns)
		{

			AddWaterColumn(pos, world.WorldAttributes.OceanHeight);

		}

	}

	private void CheckRiverColumns(Vector2Int center, out Queue<Vector2Int> checkedColumns, out int minHeight)
	{

		minHeight = world.WorldAttributes.ChunkHeight;
		checkedColumns = new Queue<Vector2Int>();

		foreach (var pos in Brush(center, world.WorldAttributes.RiverBrushRadius))
		{

			checkedColumns.Enqueue(pos);

			if (minHeight > heightMap[pos.x, pos.y] + waterMap[pos.x, pos.y])
			{

				minHeight = heightMap[pos.x, pos.y] + waterMap[pos.x, pos.y];

			}

		}

	}

	private void BuildTerrain()
	{

		for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
		{

			for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
			{

				Vector2Int pos = new Vector2Int(x, z);

				Vector2Int ChunkCoord = world.GetChunkCoord(pos);
				Vector2Int InChunkCoord = world.GetInChunkCoord(pos);

				for (int y = heightMap[x, z] + waterMap[x, z]; y > 0 && y > heightMap[x, z]; --y)
				{

					world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] = 9;

				}

				for (int y = heightMap[x, z]; y > 0; --y)
				{

					world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] = 2;

				}

				world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, 0, InChunkCoord.y] = 1;

			}

		}

	}

	private void AddSoil()
	{

		for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
		{

			for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
			{

				int biome = world.Bioms[x, z];

				AddSoilInColumn(new Vector2Int(x, z), world.WorldAttributes.BiomeAttributes[biome]);

			}

		}

	}

	private void AddSoilInColumn(Vector2Int pos, BiomeAttributes biome)
	{

		Vector2Int ChunkCoord = world.GetChunkCoord(pos);
		Vector2Int InChunkCoord = world.GetInChunkCoord(pos);

		int y;

		int terrainHeight = heightMap[pos.x, pos.y];

		if (terrainHeight >= world.WorldAttributes.ChunkHeight || terrainHeight < 0)
		{

			Debug.Log(pos.x + " " + pos.y + " " + heightMap[pos.x, pos.y]);

		}

		world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, terrainHeight, InChunkCoord.y] = biome.MainVoxel;

		int groundDepth = Random.Range(biome.GroundDepthMin, biome.GroundDepthMax + 1);

		for (y = heightMap[pos.x, pos.y] - 1; y > heightMap[pos.x, pos.y] - groundDepth && y > 0; --y)
		{

			world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] = biome.SecondVoxel;

		}

	}

	private IEnumerable<Vector2Int> Brush(Vector2Int center, int radius)
	{

		if (world.IsVoxelInWorld(center))
		{

			yield return center;

		}

		for (int r = 1; r <= radius; ++r)
		{

			for (int k = -r; k <= r; ++k)
			{

				Vector2Int pos = new Vector2Int(center.x + k, center.y + r);

				if (world.IsVoxelInWorld(pos))
				{

					yield return pos;

				}

				pos = new Vector2Int(center.x + k, center.y - r);

				if (world.IsVoxelInWorld(pos))
				{

					yield return pos;

				}

			}

			for (int k = -r + 1; k < r; ++k)
			{

				Vector2Int pos = new Vector2Int(center.x + r, center.y + k);

				if (world.IsVoxelInWorld(pos))
				{

					yield return pos;

				}

				pos = new Vector2Int(center.x - r, center.y + k);

				if (world.IsVoxelInWorld(pos))
				{

					yield return pos;

				}

			}

		}

	}

	private void GetBorderingBioms(VoronoiDiagram.GraphEdge edge, out int biomeA, out int biomeB)
	{

		double x = edge.x2 - edge.x1;
		double y = edge.y2 - edge.y1;

		Vector2 normal = Vector2.Perpendicular(new Vector2((float)x, (float)y)).normalized;

		Vector2 middle = new Vector2((float)((edge.x2 + edge.x1) / 2), (float)((edge.y2 + edge.y1) / 2));

		Vector2 A = middle + (normal * world.WorldAttributes.CheckBiomeDistance);
		Vector2 B = middle + (normal * -world.WorldAttributes.CheckBiomeDistance);

		if (world.IsVoxelInWorld(A))
		{

			biomeA = world.Bioms[Mathf.FloorToInt(A.x), Mathf.FloorToInt(A.y)];

		}
		else
		{

			biomeA = 0;

		}

		if (world.IsVoxelInWorld(B))
		{

			biomeB = world.Bioms[Mathf.FloorToInt(B.x), Mathf.FloorToInt(B.y)];

		}
		else
		{

			biomeB = 0;

		}

	}

	private void AddTrees()
	{

		for (int x = 0; x < world.WorldAttributes.WorldSizeInBlocks; ++x)
		{

			for (int z = 0; z < world.WorldAttributes.WorldSizeInBlocks; ++z)
			{

				Vector2Int pos = new Vector2Int(x, z);

				if (PerlinNoise.Get2DPerlin(world, pos, 0, world.WorldAttributes.BiomeAttributes[world.Bioms[x, z]].TreeZoneScale)
					> world.WorldAttributes.BiomeAttributes[world.Bioms[x, z]].TreeZoneThrashold)
				{

					if (waterMap[x, z] == 0)
					{

						if (PerlinNoise.Get2DPerlin(world, pos, 0, world.WorldAttributes.BiomeAttributes[world.Bioms[x, z]].TreePlacementScale)
							> world.WorldAttributes.BiomeAttributes[world.Bioms[x, z]].TreePlacementThrashold)
						{

							AddTree(pos);

						}

					}

				}

			}

		}

	}

	private void AddTree(Vector2Int pos)
	{

		BiomeAttributes biome = world.WorldAttributes.BiomeAttributes[world.Bioms[pos.x, pos.y]];

		int height = Random.Range(biome.TreeMinHeight, biome.TreeMaxHeight);

		Vector2Int ChunkCoord = world.GetChunkCoord(pos);
		Vector2Int InChunkCoord = world.GetInChunkCoord(pos);

		for (int y = heightMap[pos.x, pos.y] + 1; y <= heightMap[pos.x, pos.y] + height; ++y)
		{

			world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, y, InChunkCoord.y] = 7;

		}

		AddComa(new Vector3Int(pos.x, heightMap[pos.x, pos.y] + height, pos.y), Mathf.RoundToInt(biome.ComaRadius * ((float)height / biome.TreeMaxHeight)));

	}

	private void AddComa(Vector3Int center, int radius)
	{

		for (int r = 1; r <= radius; ++r)
		{

			for (int x = -r; x <= r; ++x)
			{

				for (int y = -r; y <= r; ++y)
				{

					for (int z = -r; z <= r; ++z)
					{

						Vector3Int pos = new Vector3Int(center.x + x, center.y + y, center.z + z);

						if (!world.IsVoxelInWorld(pos))
						{

							continue;

						}


						Vector2Int ChunkCoord = world.GetChunkCoord(pos);
						Vector2Int InChunkCoord = world.GetInChunkCoord(pos);

						if (Vector3Int.Distance(center, pos) <= radius && world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, pos.y, InChunkCoord.y] == 0)
						{

							world.Chunks[ChunkCoord.x, ChunkCoord.y].Voxels[InChunkCoord.x, pos.y, InChunkCoord.y] = 13;

						}

					}

				}

			}

		}

	}	

}