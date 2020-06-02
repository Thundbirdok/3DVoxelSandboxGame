using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ItemStack
{

	private byte id;
	private int amount;
	private int size;

	public int Size { get => size; }
	public int Amount { get => amount; set => amount = value; }
	public byte ID { get => id; }

	public ItemStack(byte _id, int _amount, int _size)
	{

		id = _id;
		amount = _amount;
		size = _size;

	}

}
