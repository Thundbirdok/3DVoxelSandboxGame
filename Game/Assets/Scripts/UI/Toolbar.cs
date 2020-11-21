using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolbar : MonoBehaviour
{

	[SerializeField]
	private RectTransform highlight = null;

	[SerializeField]
	private UIItemSlot[] slots = null;

	private int slotIndex = 0;

	private void Start()
	{

	}

	private void Update()
	{

		float scroll = Input.GetAxis("Mouse ScrollWheel");

		if (scroll != 0)
		{

			if (scroll > 0)
			{

				--slotIndex;

			}
			else
			{

				++slotIndex;

			}

			if (slotIndex > slots.Length - 1)
			{

				slotIndex = 0;

			}

			if (slotIndex < 0)
			{

				slotIndex = slots.Length - 1;

			}

			highlight.position = slots[slotIndex].SlotIcon.transform.position;

		}


	}

	public byte GetSelectedItemID()
	{

		return slots[slotIndex].ID;

	}

	public bool HasItemInSlot()
	{

		return slots[slotIndex].HasItem;

	}

	public void TakeItemFromSlot(int value)
	{

		slots[slotIndex].Take(value);

	}

	public int PutTakeToSlot(int value)
	{

		return slots[slotIndex].Put(value);

	}

	public void PutStackToSlot(ItemStack stack)
	{

		slots[slotIndex].PutStack(stack);

	}

	public void PutStack(ItemStack stack)
	{

		foreach (var slot in slots)
		{

			if (slot.ID == stack.ID)
			{

				int value = slot.Put(stack.Amount);

				stack.Amount -= value;				

				if (stack.Amount == 0)
				{

					break;

				}

			}
			else if (!slot.HasItem)
			{

				slot.PutStack(stack);

				break;

			}

		}		

	}

}
