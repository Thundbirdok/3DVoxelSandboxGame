using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "TerrainGenerator/Recipe")]
public class Recipe : ScriptableObject
{

	[SerializeField]
	private bool isFormless;

	[SerializeField]
	private byte[] slots;

	[SerializeField]
	private byte result;

	[SerializeField]
	private byte resultAmount;

	public bool IsFormless { get => isFormless; }
	public byte[] Slots { get => slots; }
	public byte Result { get => result; }
	public byte ResultAmount { get => resultAmount; }
	
}
