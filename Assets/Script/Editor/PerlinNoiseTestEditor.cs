using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PerlinNoiseTest))]
public class PerlinNoiseTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PerlinNoiseTest test = (PerlinNoiseTest)target;
        if (GUILayout.Button("Show"))
            test.UpdateValue();
    }
}
