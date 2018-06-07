using UnityEngine;
using UnityEngine.Events;

public class AdditiveSceneCollider : MonoBehaviour
{
    public string tagToCheck = "Player";
    [Space]
    public SceneField[] visibleScenes;
    public SceneField activeScene;
    public UnityEvent onEnter;

    private bool checking;

    private void OnEnable()
    {
        checking = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (checking)
        {
            if (other.tag == tagToCheck)
            {
                checking = false;
                onEnter.Invoke();

                if (activeScene != null)
                    AdditiveSceneManager.Instance.Load(visibleScenes, activeScene: activeScene);
                else
                    AdditiveSceneManager.Instance.Load(visibleScenes);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        checking = true;
    }
}
