using UnityEngine;

public class controls : MonoBehaviour
{
    [Tooltip("Canvas (or GameObject) to disable when switching")]
    public GameObject canvasToDisable;

    [Tooltip("Canvas (or GameObject) to enable when switching")]
    public GameObject canvasToEnable;

    // Call from a UI Button OnClick to switch canvases
    public void SwitchCanvases()
    {
        if (canvasToDisable == null && canvasToEnable == null)
        {
            Debug.LogWarning("controls: both canvasToDisable and canvasToEnable are null.");
            return;
        }

        if (canvasToEnable != null)
        {
            canvasToEnable.SetActive(true);
        }

        if (canvasToDisable != null)
        {
            canvasToDisable.SetActive(false);
        }
    }
}
