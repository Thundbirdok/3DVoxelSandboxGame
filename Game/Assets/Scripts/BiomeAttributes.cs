using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BiomeAttributes
{

	public string biomeName;

	[SerializeField]
	private byte mainVoxel;

	[SerializeField]
	private byte secondVoxel;

	[SerializeField]
	private int solidGroundHeight;

	[SerializeField]
	private int biomeHeight;

	[SerializeField]
	private int groundDepthMin;

	[SerializeField]
	private int groundDepthMax;

	[SerializeField]
	private float biomeScale;

	[SerializeField]
	private int octavesNumber;

	public byte MainVoxel { get => mainVoxel; set => mainVoxel = value; }
	public byte SecondVoxel { get => secondVoxel; set => secondVoxel = value; }
	public int SolidGroundHeight { get => solidGroundHeight; }
	public int BiomeHeight { get => biomeHeight; }
	public int GroundDepthMin { get => groundDepthMin; }
	public int GroundDepthMax { get => groundDepthMax; }
	public float BiomeScale { get => biomeScale; }
	public int OctavesNumber { get => octavesNumber; }
	
}
