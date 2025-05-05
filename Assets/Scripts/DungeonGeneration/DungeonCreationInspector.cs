#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(DungeonCreator))]
public class DungeonCreationInspector : Editor
{
    DungeonCreator converter;

    private void Awake()
    {
        converter = (DungeonCreator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Save Layout"))
        {
            converter.SaveLayout();
        }
        if (GUILayout.Button("Load Layout"))
        {
            converter.LoadLayout();
        }
    }
}
#endif