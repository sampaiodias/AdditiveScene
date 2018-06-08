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

public class SceneSetupFile : ScriptableObject
{
    public SceneSetup[] setup;

    public void LoadSetup()
    {
        EditorSceneManager.RestoreSceneManagerSetup(setup);
    }

    public void LoadSetupInclusive()
    {
        SceneSetup[] current = EditorSceneManager.GetSceneManagerSetup();
        SceneSetup[] newSetup = new SceneSetup[current.Length + setup.Length];

        int newSetupIndex = 0;
        for (int i = 0; i < current.Length; i++)
        {
            newSetup[newSetupIndex] = current[i];
            newSetupIndex++;
        }
        for (int i = 0; i < setup.Length; i++)
        {
            newSetup[newSetupIndex] = new SceneSetup()
            {
                path = setup[i].path,
                isLoaded = setup[i].isLoaded,
                isActive = false
            };
            newSetupIndex++;
        }

        EditorSceneManager.RestoreSceneManagerSetup(newSetup);
    }

    public void SaveSetup()
    {
        setup = EditorSceneManager.GetSceneManagerSetup();
    }
}
