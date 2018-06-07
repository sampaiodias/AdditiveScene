using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveSceneManager : MonoBehaviour
{
    [Tooltip("Attach scenes that may be unloaded during runtime using this Manager.")]
    public SceneField[] managedScenes;
    [Tooltip("Optional feature. The first scene of the list will be set as the Active scene.")]
    public SceneField[] loadScenesOnAwake;

    private int current;

    public static AdditiveSceneManager Instance;
    private static Dictionary<string, bool> loadedScenes = new Dictionary<string, bool>();

    private void Awake()
    {
        SetThisAsSingleton();
        FillActiveScenesDictionary();

        int preloadScene = SceneManager.GetActiveScene().buildIndex;

        for (int i = 0; i < loadScenesOnAwake.Length; i++)
        {
            Load(loadScenesOnAwake[i], i == 0);
        }

        SceneManager.UnloadSceneAsync(preloadScene);
    }

    /// <summary>
    /// Loads all Scenes informed (if the Scene is already loaded, nothing happens). After it, all other Scenes are unloaded.
    /// </summary>
    /// <param name="visibleScenes">Scenes that need to be visible. Scenes that are not loaded yet will be loaded.</param>
    /// <param name="activeScene">Set the Scene as active (The active Scene is the Scene which will be used as the target for new GameObjects instantiated by scripts and from what scene the lighting settings are used).</param>
    /// <param name="unloadOthers">If true, all managed Scenes that are not contained on visibleScenes will be unloaded.</param>
    public void Load(SceneField[] visibleScenes, SceneField activeScene = null, bool unloadOthers = true)
    {
        for (int i = 0; i < visibleScenes.Length; i++)
        {
            if (IsSceneLoaded(visibleScenes[i]) == false)
            {
                Load(visibleScenes[i]);
            }
        }

        if (activeScene != null && activeScene.SceneName != "")
        {
            if (IsSceneLoaded(activeScene))
            {
                SetActiveScene(activeScene);
            }
            else
            {
                Load(activeScene, setActiveScene: true);
            }
        }

        if (unloadOthers)
        {
            UnloadAllNonVisible(visibleScenes);
        }
    }

    /// <summary>
    /// Loads the Scene.
    /// </summary>
    /// <param name="scene">Scene to be loaded</param>
    /// <param name="setActiveScene">If true, the Scene will be set as active (The active Scene is the Scene which will be used as the target for new GameObjects instantiated by scripts and from what scene the lighting settings are used).</param>
    public void Load(SceneField scene, bool setActiveScene = false)
    {
        MarkSceneAsLoaded(scene, true);
        var asyncOperation = SceneManager.LoadSceneAsync(scene.SceneName, LoadSceneMode.Additive);
        asyncOperation.allowSceneActivation = true;

        if (setActiveScene)
        {
            asyncOperation.completed += a => SetActiveScene(scene);
        }
    }

    /// <summary>
    /// Unloads the Scene.
    /// </summary>
    /// <param name="scene">Scene to be unloaded.</param>
    public void Unload(SceneField scene)
    {
        if (IsSceneLoaded(scene))
        {
            MarkSceneAsLoaded(scene, false);
            Unload(scene.SceneName);
        }
    }

    /// <summary>
    /// Unloads the Scene.
    /// </summary>
    /// <param name="sceneName">The name of the Scene file (without the '.unity' extension).</param>
    public void Unload(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }

    /// <summary>
    /// Checks if the Scene is currently loaded in the game.
    /// </summary>
    /// <param name="scene"></param>
    /// <returns></returns>
    public bool IsSceneLoaded(SceneField scene)
    {
        return loadedScenes[scene];
    }

    /// <summary>
    /// Unloads all managed Scenes that are not contained inside the visibleScenes array.
    /// </summary>
    /// <param name="visibleScenes"></param>
    private void UnloadAllNonVisible(SceneField[] visibleScenes)
    {
        for (int i = 0; i < managedScenes.Length; i++)
        {
            bool unloadThisScene = true;

            for (int j = 0; j < visibleScenes.Length; j++)
            {
                if (managedScenes[i].SceneName == visibleScenes[j].SceneName)
                    unloadThisScene = false;
            }

            if (unloadThisScene)
            {
                Unload(managedScenes[i]);
            }
        }
    }

    /// <summary>
    /// Sets the Scene as active (The active Scene is the Scene which will be used as the target for new GameObjects instantiated by scripts and from what scene the lighting settings are used).
    /// </summary>
    /// <param name="scene"></param>
    private void SetActiveScene(SceneField scene)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.SceneName));
    }

    private void MarkSceneAsLoaded(SceneField scene, bool isActive)
    {
        if (scene != null)
            loadedScenes[scene] = isActive;
    }

    private void FillActiveScenesDictionary()
    {
        for (int i = 0; i < managedScenes.Length; i++)
        {
            if (!loadedScenes.ContainsKey(managedScenes[i].SceneName))
                loadedScenes.Add(managedScenes[i].SceneName, false);
        }

        for (int i = 0; i < loadScenesOnAwake.Length; i++)
        {
            if (!loadedScenes.ContainsKey(loadScenesOnAwake[i].SceneName))
                loadedScenes.Add(loadScenesOnAwake[i].SceneName, false);
        }
    }

    private void SetThisAsSingleton()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
