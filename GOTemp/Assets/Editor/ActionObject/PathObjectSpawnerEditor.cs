using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectPathSpawner))]
public class PathObjectSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ObjectPathSpawner myScript = (ObjectPathSpawner)target;

        if (GUILayout.Button("Setup Object Data - Reset"))
        {
            myScript.SetupObjectData();
            EditorUtility.SetDirty(myScript);
        }
    }
}
