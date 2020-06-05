using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemSlot : MonoBehaviour
{

	private ItemSlot itemSlot;
	[SerializeField]
	private Image slotImage;
	[SerializeField]
	private Image slotIcon;
	[SerializeField]
	private TextMeshProUGUI slotAmount;

	[SerializeField]
	private World world;

	public enum Types
	{
		Default,
		Creative,
		CraftSlot,
		ResultSlot
	}

	public Types Type;

	public bool IsClicked;	

	public bool HasItem
	{

		get
		{

			if (itemSlot == null)
			{

				return false;

			}
			else
			{

				return itemSlot.HasItem;

			}

		}

	}

	public int Amount
	{

		get => !HasItem ? 0 : itemSlot.Amount;

	}

	public byte ID
	{

		get => !HasItem ? (byte)0 : itemSlot.ID;

	}

	public int Size
	{

		get => !HasItem ? 0 : itemSlot.Size;

	}    

	public Image SlotIcon { get => slotIcon; }

	public UIItemSlot()
	{

		itemSlot = new ItemSlot();

	}

	public ItemStack GetStack()
	{

		return new ItemStack(ID, Amount, Size);

	}

	public int Take(int amt)
	{

		int value = itemSlot.Take(amt);

		UpdateSlot();

		return value;

	}

	public ItemStack TakeStack()
	{

		ItemStack stack = itemSlot.TakeStack();

		UpdateSlot();

		return stack;

	}

	public int Put(int amt)
	{

		int value = itemSlot.Put(amt);

		UpdateSlot();

		return value;

	}

	public void PutStack(ItemStack _stack)
	{

		itemSlot.PutStack(_stack);

		UpdateSlot();

	}

	public void UpdateSlot()
	{

		if (itemSlot != null && itemSlot.HasItem)
		{

			SlotIcon.sprite = world.BlocksAttributes.Blocktypes[itemSlot.ID].Icon;
			slotAmount.text = itemSlot.Amount.ToString();
			SlotIcon.enabled = true;
			slotAmount.enabled = true;

		}
		else
		{

			Clear();

		}

	}

	public void Clear()
	{

		SlotIcon.sprite = null;
		slotAmount.text = "";
		SlotIcon.enabled = false;
		slotAmount.enabled = false;

	}    

	private void OnDestroy()
	{

		if (itemSlot != null)
		{            

			itemSlot = null;

		}

	}

}

public class ItemSlot
{

	private ItemStack stack = null;         

	public bool HasItem
	{

		get
		{

			if (stack != null)
			{

				return true;

			}
			else
			{

				return false;

			}

		}

	}

	public int Amount
	{

		get => !HasItem ? 0 : stack.Amount;

	}

	public byte ID
	{

		get => !HasItem ? (byte)0 : stack.ID;

	}

	public int Size
	{

		get => !HasItem ? 0 : stack.Size;

	}    

	public ItemSlot()
	{

		stack = null;

	}

	public void EmptySlot()
	{

		stack.Amount = 1;

		stack = null;

	}

	public int Take(int amt)
	{

		if (!HasItem)
		{

			return 0;

		}

		if (amt > stack.Amount)
		{

			int _amt = stack.Amount;

			EmptySlot();

			return _amt;

		}

		if (amt < stack.Amount)
		{

			stack.Amount -= amt;            

			return amt;

		}

		EmptySlot();

		return amt;

	}

	public ItemStack TakeStack()
	{

		ItemStack handOver = new ItemStack(stack.ID, stack.Amount, stack.Size);

		EmptySlot();

		return handOver;

	}

	public int Put(int amt)
	{
		if (stack.Size - stack.Amount >= amt)
		{

			stack.Amount += amt;            

			return amt;

		}
		else
		{

			int value = stack.Size - stack.Amount;

			stack.Amount += value;            

			return value;

		}

	}

	public void PutStack(ItemStack _stack)
	{

		stack = _stack;        

	}

	~ItemSlot()
	{

		stack = null;

	}

}
