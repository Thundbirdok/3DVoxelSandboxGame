using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlocksAttributes", menuName = "TerrainGenerator/Blocks Attributes")]
public class BlocksAttributes : ScriptableObject
{

    [SerializeField]
    private Material material = null;
    [SerializeField]
    private BlockType[] blocktypes = null;

    public Material Material { get => material; }

    [SerializeField]
    private int textureAtlasSizeInBlocks = 0;

    public int TextureAtlasSizeInBlocks { get => textureAtlasSizeInBlocks; }

    public float NormalizedBlockTextureSize
    {

        get { return 1f / (float)textureAtlasSizeInBlocks; }

    }

    public BlockType[] Blocktypes { get => blocktypes; }

    public BlockType GetBlock(byte ID)
    {

        foreach(var block in blocktypes)
        {            

            if (ID == block.ID)
            {

                return block;

            }

        }

        return null;

    }
}

[System.Serializable]
public class BlockType
{

    [SerializeField]
    private string blockName = null;

    [SerializeField]
    private byte id = 0;    

    [SerializeField]
    private bool isSolid = false;
    [SerializeField]
    private bool isLiquid = false;

    [SerializeField]
    private float durability = 0;

    [SerializeField]
    private ItemType.ItemClass targetTool = ItemType.ItemClass.Item;

    [SerializeField]
    private byte drop = 0;

    [SerializeField]
    private Sprite icon = null;

    [Header("Texture Values")]
    [SerializeField]
    private int backFaceTexture = 0;
    [SerializeField]
    private int frontFaceTexture = 0;
    [SerializeField]
    private int topFaceTexture = 0;
    [SerializeField]
    private int bottomFaceTexture = 0;
    [SerializeField]
    private int leftFaceTexture = 0;
    [SerializeField]
    private int rightFaceTexture = 0;

    public string BlockName { get => blockName; }
    public byte ID { get => id; }    
    public bool IsSolid { get => isSolid; }
    public bool IsLiquid { get => isLiquid; }
    public float Durability { get => durability; }
    public ItemType.ItemClass TargetTool { get => targetTool; }
    public byte Drop { get => drop; }
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
                Debug.LogError("Error in GetTextureID; invalid face index");
                return 0;

        }

    }   

}
