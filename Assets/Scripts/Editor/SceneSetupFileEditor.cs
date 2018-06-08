#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneSetupFile))]
public class SceneSetupFileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SceneSetupFile setup = target as SceneSetupFile;

        GUILayout.Space(15);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save Current Setup"))
        {
            setup.SaveSetup();
        }
        if (GUILayout.Button("Load"))
        {
            setup.LoadSetup();
        }
        if (GUILayout.Button("Load (Inclusive)"))
        {
            setup.LoadSetupInclusive();
        }

        GUILayout.EndHorizontal();
    }
}

#endif