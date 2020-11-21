using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "TerrainGenerator/Recipe")]
public class Recipe : ScriptableObject
{

	[SerializeField]
	private bool isFormless = false;

	[SerializeField]
	private byte[] slots = null;

	[SerializeField]
	private byte result = 0;

	[SerializeField]
	private byte resultAmount = 0;

	public bool IsFormless { get => isFormless; }
	public byte[] Slots { get => slots; }
	public byte Result { get => result; }
	public byte ResultAmount { get => resultAmount; }
	
}
