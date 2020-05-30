using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{

    [SerializeField]
    private World World;

    [SerializeField]
    private GameObject Menu;

    [SerializeField]
    private GameObject Map;
    private Map MapScript;

    private bool isShow;

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

        if (Input.GetButtonDown("Cancel"))
        {

            Cancel();

        }

    }

    public void ShowMenu()
    {

        isShow = true;
        Menu.SetActive(true);        

    }

    public void ShowMap()
    {

        isShow = true;
        Map.SetActive(true);

    }

    public void Continue()
    {

        Menu.SetActive(false);
        Map.SetActive(false);
        isShow = false;

    }

    public void Exit()
    {

        Application.Quit();

    }

    public void Cancel()
    {

        if (isShow)
        {

            Cursor.lockState = CursorLockMode.Locked;

            Continue();

        }
        else
        {

            Cursor.lockState = CursorLockMode.None;

            ShowMenu();

        }

    }

    public void MapButton()
    {

        if (!isShow)
        {

            Cursor.lockState = CursorLockMode.None;

            ShowMap();

        }

    }

    public void GenerateWorld()
    {

        World.GenerateWorld();

        MapScript.CreateMap();

    }

}
