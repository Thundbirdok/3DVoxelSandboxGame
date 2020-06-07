using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipes", menuName = "TerrainGenerator/Recipes")]
public class RecipesBook : ScriptableObject
{

	[SerializeField]
	private Recipe[] recipes = null;

	public Recipe[] Recipes { get => recipes; }

}
