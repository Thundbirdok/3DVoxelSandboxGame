using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreativeInventory : MonoBehaviour
{

    [SerializeField]
    private GameObject slotPrefab;
    [SerializeField]
    private World world;

    //	List<ItemSlot> slots = new List<ItemSlot>();

    private void Start()
    {

        for (int i = 1; i < world.BlocksAttributes.Blocktypes.Length; ++i)
        {

            GameObject newSlot = Instantiate(slotPrefab, transform);

            ItemStack stack = new ItemStack((byte)i, 1, 64);

            newSlot.GetComponent<UIItemSlot>().PutStack(stack);
            newSlot.GetComponent<UIItemSlot>().IsCreative = true;

        }

    }

}
