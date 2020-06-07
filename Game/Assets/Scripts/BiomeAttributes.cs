using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BiomeAttributes
{

	public string biomeName;

	[SerializeField]
	private byte mainVoxel = 0;

	[SerializeField]
	private byte secondVoxel = 0;

	[SerializeField]
	private int solidGroundHeight = 0;

	[SerializeField]
	private int biomeHeight = 0;

	[SerializeField]
	private int groundDepthMin = 0;

	[SerializeField]
	private int groundDepthMax = 0;

	[SerializeField]
	private float biomeScale = 0;

	[SerializeField]
	private int octavesNumber = 0;

	public byte MainVoxel { get => mainVoxel; set => mainVoxel = value; }
	public byte SecondVoxel { get => secondVoxel; set => secondVoxel = value; }
	public int SolidGroundHeight { get => solidGroundHeight; }
	public int BiomeHeight { get => biomeHeight; }
	public int GroundDepthMin { get => groundDepthMin; }
	public int GroundDepthMax { get => groundDepthMax; }
	public float BiomeScale { get => biomeScale; }
	public int OctavesNumber { get => octavesNumber; }
	
}
