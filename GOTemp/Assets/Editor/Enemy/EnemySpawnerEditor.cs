using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemySpawner))]
public class EnemySpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EnemySpawner myScript = (EnemySpawner)target;

        if (GUILayout.Button("SETUP ENEMY DATA"))
        {
            myScript.SetupEnemyData();
            EditorUtility.SetDirty(myScript);
        }
    }
}
