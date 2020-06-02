﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragAndDropHandler : MonoBehaviour
{

	[SerializeField]
	private UIItemSlot cursorSlot;

	[SerializeField]
	private GraphicRaycaster m_Raycaster = null;

	private PointerEventData m_PointerEventData;

	[SerializeField]
	private EventSystem m_EventSystem = null;

	[SerializeField]
	private UIController uicontroller;

	private void Update()
	{

		if (!uicontroller.IsInUI)
		{

			if (cursorSlot.HasItem)
			{

				//throw away*/
				cursorSlot.TakeStack();

			}

			return;

		}

		cursorSlot.transform.position = Input.mousePosition;

		if (Input.GetMouseButtonDown(0))
		{

			HandleSlotClick(CheckForSlot());

		}

	}

	private void HandleSlotClick(UIItemSlot clickedSlot)
	{

		if (clickedSlot == null)
		{

			return;

		}

		if (!cursorSlot.HasItem && !clickedSlot.HasItem)
		{

			return;

		}

		if (clickedSlot.IsCreative)
		{

			if (cursorSlot.HasItem)
			{

				if (cursorSlot.ID != clickedSlot.ID)
				{
					
					cursorSlot.PutStack(clickedSlot.GetStack());

				}
				else
				{

					cursorSlot.Put(1);

				}

			}
			else
			{

				cursorSlot.PutStack(clickedSlot.GetStack());

			}

		}
		else if (!cursorSlot.HasItem && clickedSlot.HasItem)
		{

			cursorSlot.PutStack(clickedSlot.TakeStack());

			return;

		}
		else if (cursorSlot.HasItem && !clickedSlot.HasItem)
		{

			clickedSlot.PutStack(cursorSlot.TakeStack());

			return;

		}
		else if (cursorSlot.HasItem && clickedSlot.HasItem)
		{

			if (cursorSlot.ID != clickedSlot.ID)
			{

				ItemStack oldCursorSlot = cursorSlot.TakeStack();
				ItemStack oldSlot = clickedSlot.TakeStack();

				clickedSlot.PutStack(oldCursorSlot);
				cursorSlot.PutStack(oldSlot);

			}
			else if (cursorSlot.Amount < cursorSlot.Size)
			{

				int value = cursorSlot.Put(clickedSlot.Amount);

				clickedSlot.Take(value);                

			}

		}


	}

	private UIItemSlot CheckForSlot()
	{

		m_PointerEventData = new PointerEventData(m_EventSystem);
		m_PointerEventData.position = Input.mousePosition;

		List<RaycastResult> results = new List<RaycastResult>();
		m_Raycaster.Raycast(m_PointerEventData, results);

		foreach (RaycastResult result in results)
		{

			if (result.gameObject.tag == "UIItemSlot")
				return result.gameObject.GetComponent<UIItemSlot>();

		}

		return null;

	}

}