using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectSpawner))]
public class ObjectSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ObjectSpawner myScript = (ObjectSpawner)target;

        if (GUILayout.Button("Setup Object Data - Reset"))
        {
            myScript.SetupObjectData();
            EditorUtility.SetDirty(myScript);
        }
    }
}
