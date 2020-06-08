using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BiomeAttributes
{

    public string biomeName;

    [Header("Terrain")]
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

    [Header("Trees")]
    [SerializeField]
    private int treeMinHeight;
    [SerializeField]
    private int treeMaxHeight;

    [SerializeField]
    private int comaRadius;

    [SerializeField]
    private float treeZoneScale;
    [SerializeField, Range(0, 1)]
    private float treeZoneThrashold;
    [SerializeField]
    private float treePlacementScale;
    [SerializeField, Range(0, 1)]
    private float treePlacementThrashold;

    public byte MainVoxel { get => mainVoxel; set => mainVoxel = value; }
    public byte SecondVoxel { get => secondVoxel; set => secondVoxel = value; }
    public int SolidGroundHeight { get => solidGroundHeight; }
    public int BiomeHeight { get => biomeHeight; }
    public int GroundDepthMin { get => groundDepthMin; }
    public int GroundDepthMax { get => groundDepthMax; }
    public float BiomeScale { get => biomeScale; }
    public int OctavesNumber { get => octavesNumber; }

    public int TreeMinHeight { get => treeMinHeight; }
    public int TreeMaxHeight { get => treeMaxHeight; }
    public int ComaRadius { get => comaRadius; }
    public float TreeZoneScale { get => treeZoneScale; }
    public float TreeZoneThrashold { get => treeZoneThrashold; }    
    public float TreePlacementScale { get => treePlacementScale; }
    public float TreePlacementThrashold { get => treePlacementThrashold; }
    
}
