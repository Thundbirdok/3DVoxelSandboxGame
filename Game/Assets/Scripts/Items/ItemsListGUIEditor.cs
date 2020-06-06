using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

[CustomEditor(typeof(ItemsList))]
public class ItemsListGUIEditor : Editor
{
    
    private ItemsList items;

    public void Awake()
    {

        items = (ItemsList)target;

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

        //if (GUI.changed)
        //{

        //    SetObjectDirty(itemsList.gameObject);

        //}

    }

    //public static void SetObjectDirty(GameObject obj)
    //{

    //    EditorUtility.SetDirty(obj);
    //    EditorSceneManager.MarkSceneDirty(obj.scene);

    //}

}

