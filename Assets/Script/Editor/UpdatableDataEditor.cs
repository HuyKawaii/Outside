using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UpdatableData), true)]
public class UpdatableDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        UpdatableData data = target as UpdatableData;
        base.OnInspectorGUI();

        if (GUILayout.Button("Update"))
        {
            data.NotifyUpdatedValue();
        }
    }
}
