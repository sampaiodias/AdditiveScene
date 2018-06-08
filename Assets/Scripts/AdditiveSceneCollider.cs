using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
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

#if UNITY_EDITOR
    public Color unselectedGizmoColor = new Color(0.1f, 1.0f, 0.1f, .3f);
    public Color selectedGizmoColor = new Color(0.1f, 1.0f, 0.1f, .6f);

    private void OnDrawGizmos()
    {
        Gizmos.color = unselectedGizmoColor;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = selectedGizmoColor;
        Gizmos.DrawCube(transform.position, GetComponent<Collider>().bounds.size);
    }
#endif
}
