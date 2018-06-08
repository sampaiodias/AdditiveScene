#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneSetupManager
{
    [MenuItem("Scene/Save Scene Setup")]
    public static void SaveSetup()
    {
        SceneSetupFile setupFile = CreateAsset<SceneSetupFile>();
        setupFile.SaveSetup();
    }

    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary>
    private static T CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        return asset;
    }
}

public class SceneSetupFile : ScriptableObject
{
    public SceneSetup[] setup;

    public void LoadSetup()
    {
        EditorSceneManager.RestoreSceneManagerSetup(setup);
    }

    public void SaveSetup()
    {
        setup = EditorSceneManager.GetSceneManagerSetup();
    }
}

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

        GUILayout.EndHorizontal();
    }
}

#endif