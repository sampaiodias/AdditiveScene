#if UNITY_EDITOR
using System.IO;
using UnityEditor;
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
#endif
