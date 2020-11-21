using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

[CustomEditor(typeof(ItemsAttributes))]
public class ItemsListGUIEditor : Editor
{
    
    private ItemsAttributes items;

    public void Awake()
    {

        items = (ItemsAttributes)target;

    }

    public override void OnInspectorGUI()
    {

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("RemoveAll"))
        {

            items.RemoveAll();

        }
        else if (GUILayout.Button("Remove"))
        {

            items.RemoveCurrentElement();

        }
        else if (GUILayout.Button("Add"))
        {

            items.AddElement();

        }
        else if (GUILayout.Button("<="))
        {

            items.GetPrev();

        }
        else if (GUILayout.Button("=>"))
        {

            items.GetNext();

        }

        EditorGUILayout.EndHorizontal();

        base.OnInspectorGUI();

    }

}

