using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolbar : MonoBehaviour
{

	[SerializeField]
	private RectTransform highlight;

	[SerializeField]
	private UIItemSlot[] slots;

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

	internal byte GetSelectedItemID()
	{

		return slots[slotIndex].ID;

	}

	internal bool HasItemInSlot()
	{

		return slots[slotIndex].HasItem;

	}

	internal void TakeBlock(int value)
	{

		slots[slotIndex].Take(value);

	}

}
