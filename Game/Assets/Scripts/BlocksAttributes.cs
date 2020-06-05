using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlocksAttributes", menuName = "TerrainGenerator/Blocks Attributes")]
public class BlocksAttributes : ScriptableObject
{

    [SerializeField]
    private Material material;
    [SerializeField]
    private BlockType[] blocktypes;

    public Material Material { get => material; }

    [SerializeField]
    private int textureAtlasSizeInBlocks;

    public int TextureAtlasSizeInBlocks { get => textureAtlasSizeInBlocks; }

    public float NormalizedBlockTextureSize
    {

        get { return 1f / (float)textureAtlasSizeInBlocks; }

    }

    public BlockType[] Blocktypes { get => blocktypes; }
}

[System.Serializable]
public class BlockType
{

    [SerializeField]
    private string blockName;

    [SerializeField]
    private bool isSolid;
    [SerializeField]
    private bool isLiquid;

    [SerializeField]
    private Sprite icon;

    [Header("Texture Values")]
    [SerializeField]
    private int backFaceTexture;
    [SerializeField]
    private int frontFaceTexture;
    [SerializeField]
    private int topFaceTexture;
    [SerializeField]
    private int bottomFaceTexture;
    [SerializeField]
    private int leftFaceTexture;
    [SerializeField]
    private int rightFaceTexture;

    public string BlockName { get => blockName; }
    public bool IsSolid { get => isSolid; }
    public bool IsLiquid { get => isLiquid; }
    public Sprite Icon { get => icon; }
    public int BackFaceTexture { get => backFaceTexture; }
    public int FrontFaceTexture { get => frontFaceTexture; }
    public int TopFaceTexture { get => topFaceTexture; }
    public int BottomFaceTexture { get => bottomFaceTexture; }
    public int LeftFaceTexture { get => leftFaceTexture; }
    public int RightFaceTexture { get => rightFaceTexture; }

    // Back, Front, Top, Bottom, Left, Right
    public int GetTextureID(int faceIndex)
    {

        switch (faceIndex)
        {

            case 0:
                return BackFaceTexture;
            case 1:
                return FrontFaceTexture;
            case 2:
                return TopFaceTexture;
            case 3:
                return BottomFaceTexture;
            case 4:
                return LeftFaceTexture;
            case 5:
                return RightFaceTexture;
            default:
                Debug.Log("Error in GetTextureID; invalid face index");
                return 0;

        }

    }

}
