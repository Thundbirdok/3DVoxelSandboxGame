using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{

	[SerializeField]
	private World World = null;

	[SerializeField]
	private GameObject Menu = null;

	[SerializeField]
	private GameObject GameInterface = null;

	[SerializeField]
	private GameObject Inventory = null;

	[SerializeField]
	private GameObject Map = null;
	private Map MapScript;

	private bool isInUI;

	public bool IsInUI
	{

		get { return isInUI; }

		set
		{

			isInUI = value;

			if (isInUI)
			{

				Cursor.lockState = CursorLockMode.None;

			}
			else
			{

				Cursor.lockState = CursorLockMode.Locked;

			}

		}

	}

	public void Awake()
	{

		Menu.SetActive(false);
		Map.SetActive(false);

		MapScript = Map.GetComponent<Map>();

	}

	private void Update()
	{

		if (Input.GetKeyDown("m"))
		{

			MapButton();

		}

		if (Input.GetKeyDown("i"))
		{

			InventoryButton();

		}

		if (Input.GetButtonDown("Cancel"))
		{

			Cancel();

		}

	}

	public void ShowMenu()
	{

		IsInUI = true;
		Menu.SetActive(true);

	}

	public void ShowMap()
	{

		IsInUI = true;
		GameInterface.SetActive(false);
		Map.SetActive(true);

	}

	public void ShowInventory()
	{

		IsInUI = true;
		GameInterface.SetActive(true);
		Inventory.SetActive(true);

	}

	public void Continue()
	{

		Menu.SetActive(false);
		Map.SetActive(false);
		Inventory.SetActive(false);
		GameInterface.SetActive(true);
		IsInUI = false;

	}

	public void Exit()
	{

		Application.Quit();

	}

	public void Cancel()
	{

		if (IsInUI)
		{

			Continue();

		}
		else
		{

			ShowMenu();

		}

	}

	public void MapButton()
	{

		if (!IsInUI)
		{

			ShowMap();

		}

	}

	private void InventoryButton()
	{

		if (IsInUI)
		{

			Continue();

		}
		else
		{

			ShowInventory();

		}

	}

	public void GenerateWorld()
	{

		World.GenerateWorld();

		MapScript.CreateMap();

	}

}
