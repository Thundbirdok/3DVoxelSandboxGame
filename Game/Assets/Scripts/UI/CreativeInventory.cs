using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreativeInventory : MonoBehaviour
{

    [SerializeField]
    private GameObject slotPrefab = null;
    [SerializeField]
    private World world = null;    

    private void Start()
    {

        foreach (var block in world.BlocksAttributes.Blocktypes)
        {

            if (block.Icon != null)
            {

                GameObject newSlot = Instantiate(slotPrefab, transform);

                ItemStack stack = new ItemStack(block.ID, 1, 64);

                newSlot.GetComponent<UIItemSlot>().PutStack(stack);
                newSlot.GetComponent<UIItemSlot>().Type = UIItemSlot.Types.Creative;

            }

        }

        foreach (var item in world.ItemsAttributes)
        {

            if (item.Icon != null)
            {

                GameObject newSlot = Instantiate(slotPrefab, transform);

                ItemStack stack = new ItemStack(item.ID, 1, 64);

                newSlot.GetComponent<UIItemSlot>().PutStack(stack);
                newSlot.GetComponent<UIItemSlot>().Type = UIItemSlot.Types.Creative;

            }

        }

    }

}
