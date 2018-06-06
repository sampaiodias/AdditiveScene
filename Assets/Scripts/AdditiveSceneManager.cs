using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveSceneManager : MonoBehaviour
{
    public SceneField[] persistentScenes;
    public SceneField[] allScenes;
    public SceneField[] loadScenesOnAwake;

    private int current;

    public static AdditiveSceneManager Instance;
    private static Dictionary<string, bool> activeScenes = new Dictionary<string, bool>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
        FillActiveScenesDictionary();

        for (int i = 0; i < loadScenesOnAwake.Length; i++)
        {
            Load(loadScenesOnAwake[i], i == 0);
        }

        for (int i = 0; i < persistentScenes.Length; i++)
        {
            Load(persistentScenes[i]);
        }
    }

    public void Load(SceneField[] visibleScenes, bool unloadOthers = true)
    {
        for (int i = 0; i < visibleScenes.Length; i++)
        {
            if (IsSceneActive(visibleScenes[i]) == false)
            {
                Load(visibleScenes[i]);
            }
        }

        if (unloadOthers)
        {
            for (int i = 0; i < allScenes.Length; i++)
            {
                bool unloadThisScene = true;

                for (int j = 0; j < visibleScenes.Length; j++)
                {
                    if (allScenes[i].SceneName == visibleScenes[j].SceneName)
                        unloadThisScene = false;
                }

                if (unloadThisScene)
                {
                    Unload(allScenes[i]);
                }
            }
        }
    }

    public void Load(SceneField scene, bool setActiveScene = false)
    {
        MarkSceneActive(scene, true);
        var asyncOperation = SceneManager.LoadSceneAsync(scene.SceneName, LoadSceneMode.Additive);
        asyncOperation.allowSceneActivation = true;

        if (setActiveScene)
        {
            asyncOperation.completed += a => SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.SceneName));
        }
    }

    public void Unload(SceneField scene)
    {
        if (IsSceneActive(scene))
        {
            MarkSceneActive(scene, false);
            Unload(scene.SceneName);
        }
    }

    public void Unload(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }

    public bool IsSceneActive(SceneField scene)
    {
        return activeScenes[scene];
    }

    public void MarkSceneActive(SceneField scene, bool isActive)
    {
        activeScenes[scene] = isActive;
    }

    private void FillActiveScenesDictionary()
    {
        for (int i = 0; i < persistentScenes.Length; i++)
        {
            if (!activeScenes.ContainsKey(persistentScenes[i].SceneName))
                activeScenes.Add(persistentScenes[i].SceneName, false);
        }

        for (int i = 0; i < allScenes.Length; i++)
        {
            if (!activeScenes.ContainsKey(allScenes[i].SceneName))
                activeScenes.Add(allScenes[i].SceneName, false);
        }

        for (int i = 0; i < loadScenesOnAwake.Length; i++)
        {
            if (!activeScenes.ContainsKey(loadScenesOnAwake[i].SceneName))
                activeScenes.Add(loadScenesOnAwake[i].SceneName, false);
        }
    }
}
