using UnityEngine;
using UnityEngine.SceneManagement;

public class load_Scene : MonoBehaviour
{
    [Tooltip("Name of the scene to load")]
    public string sceneName;
    [Tooltip("Disable the parent Canvas GameObject before loading the scene")]
    public bool disableParentCanvasBeforeLoad = false;

    // Call this method from a UI Button OnClick in the Inspector.
    public void LoadScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("load_Scene: sceneName is empty. Set it in the Inspector.");
            return;
        }
        else
        {
            if (disableParentCanvasBeforeLoad)
            {
                var parentCanvas = GetComponentInParent<Canvas>();
                if (parentCanvas != null)
                {
                    parentCanvas.gameObject.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("load_Scene: No parent Canvas found to disable.");
                }
            }
            SceneManager.LoadScene(sceneName);
        }
    }
}
