using UnityEditor.SceneManagement;
using UnityEngine;

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